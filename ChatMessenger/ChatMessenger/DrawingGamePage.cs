using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatMessenger
{
    public partial class DrawingGamePage : Form
    {
        private PictureBox canvas;
        private Bitmap drawingBitmap;   
        private Graphics g;
        private bool isDrawing = false;
        private Point lastPoint;
        private Color brushColor = Color.Black;
        private int brushSize = 4;

        private readonly JsonWebSocketClient wsClient;
        private readonly string myName;

        // small buffer of local strokes (optional, not required)
        private readonly List<DrawingStroke> localStrokes = new List<DrawingStroke>();

        public DrawingGamePage(JsonWebSocketClient existingWsClient, User currentUser)
        {
            if (existingWsClient == null) throw new ArgumentNullException(nameof(existingWsClient));
            wsClient = existingWsClient;

            // Use Username property from your User class
            myName = currentUser?.Username ?? "Guest";

            InitializeUI();

            // Subscribe to websocket messages (page will handle draw & draw_load)
            wsClient.OnMessage += WsClient_OnMessage;

            // Send join if client has already connected -- if not, it's fine, server will reply when it connects
            Task.Run(async () =>
            {
                await Task.Delay(150);
                try
                {
                    await wsClient.SendAsync(new WsMessage { Type = "join", Name = myName });
                }
                catch { /* ignore */ }
            });
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // canvas
            canvas = new PictureBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(canvas);

            // initial bitmap (resizable)
            drawingBitmap = new Bitmap(Math.Max(800, Math.Max(1, canvas.Width)), Math.Max(600, Math.Max(1, canvas.Height)));
            g = Graphics.FromImage(drawingBitmap);
            g.Clear(Color.White);
            canvas.Image = drawingBitmap;

            // Load previous strokes after g exists
            LoadLocalDrawing();

            // toolbar
            FlowLayoutPanel toolbar = new FlowLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(6),
                FlowDirection = FlowDirection.LeftToRight
            };
            this.Controls.Add(toolbar);

            Button btnColor = new Button() { Text = "Color", Height = 32 };
            btnColor.Click += (s, e) =>
            {
                using (var dlg = new ColorDialog())
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                        brushColor = dlg.Color;
                }
            };
            toolbar.Controls.Add(btnColor);

            Label lblSize = new Label() { Text = "Size:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            toolbar.Controls.Add(lblSize);

            NumericUpDown sizePicker = new NumericUpDown() { Minimum = 1, Maximum = 40, Value = 4, Width = 60 };
            sizePicker.ValueChanged += (s, e) => brushSize = (int)sizePicker.Value;
            toolbar.Controls.Add(sizePicker);

            Button btnClear = new Button() { Text = "Clear (local)", Height = 32 };
            btnClear.Click += (s, e) =>
            {
                g.Clear(Color.White);
                canvas.Invalidate();
                localStrokes.Clear();

                // Delete saved JSON file
                if (System.IO.File.Exists("drawing.json"))
                    System.IO.File.Delete("drawing.json");
            };
            toolbar.Controls.Add(btnClear);

            Button btnSave = new Button() { Text = "Save (server)", Height = 32 };
            btnSave.Click += async (s, e) =>
            {
                try
                {
                    await wsClient.SendAsync(new WsMessage { Type = "draw_save", Name = myName });
                    MessageBox.Show("Save requested (server will handle persistence).");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Save request failed: " + ex.Message);
                }
            };
            toolbar.Controls.Add(btnSave);

            // mouse events
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            this.Resize += (s, e) => EnsureCanvasSize();
        }


        private void LoadLocalDrawing()
        {
            try
            {
                if (System.IO.File.Exists("drawing.json"))
                {
                    var json = System.IO.File.ReadAllText("drawing.json");
                    var strokes = JsonSerializer.Deserialize<List<DrawingStroke>>(json);

                    if (strokes != null && g != null)
                    {
                        localStrokes.AddRange(strokes);

                        foreach (var s in strokes)
                        {
                            using (var pen = new Pen(Color.FromArgb(s.ColorArgb), s.Size))
                            {
                                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                                g.DrawLine(pen, new Point(s.X1, s.Y1), new Point(s.X2, s.Y2));
                            }
                        }

                        canvas.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load previous drawing: " + ex.Message);
            }
        }


        private void EnsureCanvasSize()
        {
            if (canvas.Width <= 0 || canvas.Height <= 0) return;

            if (drawingBitmap.Width < canvas.Width || drawingBitmap.Height < canvas.Height)
            {
                int newW = Math.Max(drawingBitmap.Width, canvas.Width);
                int newH = Math.Max(drawingBitmap.Height, canvas.Height);
                var newBmp = new Bitmap(newW, newH);
                using (var ng = Graphics.FromImage(newBmp))
                {
                    ng.Clear(Color.White);
                    ng.DrawImageUnscaled(drawingBitmap, 0, 0);
                }
                drawingBitmap.Dispose();
                drawingBitmap = newBmp;
                g = Graphics.FromImage(drawingBitmap);
                canvas.Image = drawingBitmap;
                canvas.Invalidate();
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            var p2 = e.Location;
            using (var pen = new Pen(brushColor, brushSize))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(pen, lastPoint, p2);
            }

            var stroke = new DrawingStroke
            {
                X1 = lastPoint.X,
                Y1 = lastPoint.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                ColorArgb = brushColor.ToArgb(),
                Size = brushSize,
                User = myName,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            localStrokes.Add(stroke);

            // send stroke
            _ = SendStrokeAsync(stroke);

            // save to json immediately
            try
            {
                var json = JsonSerializer.Serialize(localStrokes);
                System.IO.File.WriteAllText("drawing.json", json);
            }
            catch { }

            lastPoint = p2;
            canvas.Invalidate();
        }


        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isDrawing = false;
        }

        private async Task SendStrokeAsync(DrawingStroke stroke)
        {
            if (wsClient == null) return;

            try
            {
                await wsClient.SendAsync(new WsMessage { Type = "draw", Data = stroke, Name = myName });
            }
            catch
            {
                // ignore send errors
            }
        }

        // When messages arrive from server, they come here.
        private void WsClient_OnMessage(WsMessage msg)
        {
            try
            {
                if (msg == null) return;

                if (msg.Type == "draw")
                {
                    // single stroke
                    DrawingStroke stroke = null;
                    if (msg.Data is JsonElement je && je.ValueKind != JsonValueKind.Undefined)
                    {
                        stroke = je.Deserialize<DrawingStroke>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        // fallback
                        var json = JsonSerializer.Serialize(msg.Data);
                        stroke = JsonSerializer.Deserialize<DrawingStroke>(json);
                    }

                    if (stroke != null)
                    {
                        // draw on UI thread
                        this.Invoke(() =>
                        {
                            using (var pen = new Pen(Color.FromArgb(stroke.ColorArgb), stroke.Size))
                            {
                                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                                g.DrawLine(pen, new Point(stroke.X1, stroke.Y1), new Point(stroke.X2, stroke.Y2));
                            }
                            canvas.Invalidate();
                        });
                    }
                }
                else if (msg.Type == "draw_load")
                {
                    // load array of strokes
                    List<DrawingStroke> strokes = null;
                    if (msg.Data is JsonElement je && je.ValueKind != JsonValueKind.Undefined)
                    {
                        strokes = je.Deserialize<List<DrawingStroke>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        var json = JsonSerializer.Serialize(msg.Data);
                        strokes = JsonSerializer.Deserialize<List<DrawingStroke>>(json);
                    }

                    if (strokes != null)
                    {
                        this.Invoke(() =>
                        {
                            foreach (var s in strokes)
                            {
                                using (var pen = new Pen(Color.FromArgb(s.ColorArgb), s.Size))
                                {
                                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                                    g.DrawLine(pen, new Point(s.X1, s.Y1), new Point(s.X2, s.Y2));
                                }
                            }
                            canvas.Invalidate();
                        });
                    }
                }
                // optionally handle draw_saved etc.
            }
            catch
            {
                // ignore parse errors
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Unsubscribe from websocket messages
            try
            {
                wsClient.OnMessage -= WsClient_OnMessage;
            }
            catch { }

            // Save local strokes to JSON
            try
            {
                var json = JsonSerializer.Serialize(localStrokes);
                System.IO.File.WriteAllText("drawing.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save drawing: " + ex.Message);
            }
        }
    }
}
