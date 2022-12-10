using SketcherControl.Filling;
using SketcherControl.Shapes;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl
{
    public class Sketcher : PictureBox, IRenderer
    {
        DirectBitmap canvas;
        private readonly List<Triangle> triangles = new();
        private Timer resizeTimer;
        /// <summary>
        /// Blokuje renderowanie sceny
        /// </summary>
        private bool freeze;
        /// <summary>
        /// Czy animacja była włączona przed przesunięciem źródła światła / przed zmianą rozmiaru okna
        /// </summary>
        private bool wasAnimationTurnedOn;
        /// <summary>
        /// Czy użytkownik przesuwa myszką źródło światłą?
        /// </summary>
        private bool lightIsMoving;
        private float objectScale;

        public int RenderThreads { get; set; }
        public LightSource LightSource { get; }
        public ColorPicker ColorPicker { get; }
        public bool Fill { get; set; } = true;
        public SizeF ObjectSize{ get; set; }

        public Size BitmpapSize
        {
            get => new(this.canvas.Width, this.canvas.Height);
            set
            {
                this.canvas.Dispose();
                this.canvas = new DirectBitmap(value.Width, value.Height);
                this.Image = this.canvas.Bitmap;
                Refresh();
            }
        }

        public bool ShowLines { get; set; } = false;

        public Sketcher()
        {
            LightSource = new(this);
            ColorPicker = new(LightSource);
            this.canvas = new DirectBitmap(this.Width, this.Height);
            this.Dock = DockStyle.Fill;

            this.Image = this.canvas.Bitmap;
            this.resizeTimer = new();
            this.resizeTimer.Interval = 100;
            this.resizeTimer.Tick += ResizeTimerHandler;
            LightSource.LightSourceChanged += ParametersChangedHandler;
            ColorPicker.ParametersChanged += ParametersChangedHandler;
        }

        #region Loading Object
        public void LoadObjectFromFile(string fileName)
        {
            string fileContent;

            using (StreamReader reader = new StreamReader(fileName))
            {
                fileContent = reader.ReadToEnd();
            }

            LoadObject(fileContent);
        }


        public void LoadObject(string shapeObject)
        {
            this.triangles.Clear();
            PointF minPoint = new(float.MaxValue, float.MaxValue);
            PointF maxPoint = new(float.MinValue, float.MinValue);
            LightSource.MinZ = float.MinValue;
            List<Vertex> vertices = new List<Vertex>();
            List<Vector4> normalVectors = new List<Vector4>();

            string[] lines = shapeObject.Split("\n");

            foreach (var line in lines.Where((line) => line.StartsWith("v ")))
            {
                var values = line.Split(" ");
                var x = float.Parse(values[1]);
                var y = float.Parse(values[2]);
                var z = float.Parse(values[3]);
                vertices.Add(new Vertex(x, y, z));

                if (z > LightSource.MinZ)
                    LightSource.MinZ = z;

                if(x > maxPoint.X)
                    maxPoint.X = x;
                if (x < minPoint.X)
                    minPoint.X = x;
                if (y > maxPoint.Y)
                    maxPoint.Y = y;
                if (y < minPoint.Y)
                    minPoint.Y = y;
            }

            foreach (var line in lines.Where((line) => line.StartsWith("vn")))
            {
                var values = line.Split(" ");
                normalVectors.Add(new Vector4(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]), 0));
            }

            Triangle triangle = new();

            foreach (var line in lines.Where((line) => line.StartsWith("f")))
            {
                var faces = line.TrimStart('f', ' ').Split(" ");

                foreach (var face in faces)
                {
                    var vertexIndex = face.Split("//");
                    triangle.AddVertex(vertices[int.Parse(vertexIndex[0]) - 1], normalVectors[int.Parse(vertexIndex[1]) - 1]);
                }

                this.triangles.Add(triangle);
                triangle = new();
            }

            ObjectSize = new(Math.Abs(maxPoint.X - minPoint.X), Math.Abs(maxPoint.Y - minPoint.Y));
            CalculateObjectScale();
            SetRenderScale();
            Refresh();
        }
        #endregion

        #region Rendering
        private void ParametersChangedHandler()
        {
            Refresh();
        }

        private Task FillAsync(int start, int step)
        {
            return Task.Run(
                () =>
                {
                    for (int j = start; j < start + step && j < this.triangles.Count; j++)
                    {
                        ScanLine.Fill(this.triangles[j], canvas, ColorPicker);

                    }
                });
        }

        public override void Refresh()
        {
            if (freeze)
                return;

            using (var g = Graphics.FromImage(this.canvas.Bitmap))
            {
                g.Clear(Color.White);
            }

            if(RenderThreads == 1 && Fill)
            {
                foreach (var triangle in this.triangles)
                {
                    ScanLine.Fill(triangle, this.canvas, ColorPicker);
                }
            }
            else if (Fill)
            {
                var trianglesPerThread = (int)Math.Ceiling((float)this.triangles.Count / RenderThreads);
                List<Task> tasks = new();

                for (int i = 0; i < RenderThreads; i++)
                {
                    tasks.Add(FillAsync(i * trianglesPerThread, trianglesPerThread));
                }

                Task.WaitAll(tasks.ToArray());
            }

            if (ShowLines)
                foreach (var triangle in triangles)
                {
                    triangle.Render(this.canvas);
                }

            LightSource.Render(this.canvas);

            base.Refresh();
        }

        private void ResizeTimerHandler(object? sender, EventArgs e)
        {
            this.resizeTimer.Stop();
            BitmpapSize = new Size(this.Width, this.Height);
            CalculateObjectScale();
            SetRenderScale();
            this.LightSource.LightAnimation = this.wasAnimationTurnedOn;
            this.freeze = false;
            LightSource.RecalculateLightCoordinates();
            Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!this.freeze)
            {
                this.freeze = true;
                wasAnimationTurnedOn = this.LightSource.LightAnimation;
                this.LightSource.LightAnimation = false;
            }
            this.resizeTimer.Stop();
            this.resizeTimer.Start();
        }

        private void CalculateObjectScale()
        {
            this.objectScale = 0.8f * Math.Min(canvas.Width, canvas.Height) / Math.Max(ObjectSize.Width, ObjectSize.Height);
        }

        private void SetRenderScale()
        {
            foreach (var triangle in triangles)
            {
                triangle.SetRenderScale(this.objectScale, (float)canvas.Width / 2, (float)canvas.Height / 2);
            }
        }

        public Vector4 Unscale(float x, float y, float z)
        {
            return new Vector4()
            {
                X = (x - (float)canvas.Width / 2) / this.objectScale,
                Y = (y - (float)canvas.Height / 2) / this.objectScale,
                Z = z,
            };
        }
        #endregion

        #region Overrides
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (LightSource.HitTest(e.X, e.Y))
            {
                this.lightIsMoving = true;
                wasAnimationTurnedOn = LightSource.LightAnimation;
                LightSource.LightAnimation = false;
            }
            else
            {
                this.lightIsMoving = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.lightIsMoving)
            {
                freeze = true;
                LightSource.MoveTo(e.X, e.Y);
                freeze = false;
                Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.lightIsMoving)
            {
                this.lightIsMoving = false;
                LightSource.LightAnimation = wasAnimationTurnedOn;
                wasAnimationTurnedOn = false;
            }
        }
        #endregion
    }
}