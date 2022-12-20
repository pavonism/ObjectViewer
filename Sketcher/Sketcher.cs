using SketcherControl.Filling;
using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl
{
    public class Sketcher : PictureBox, IRenderer
    {
        DirectBitmap canvas;
        private List<Object3> objects = new();
        private Timer resizeTimer;
        private Timer rotationTimer;
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
        /// <summary>
        /// Czy użytkownik obraca widok?
        /// </summary>
        private bool isRotating;

        private float angleX;
        private float angleY;
        private Point lastMousePosition;

        private float objectScale;
        private int rows;
        private int columns;
        private int columnWidth;
        private int rowHeight;

        private Matrix4x4 view;
        private Matrix4x4 position;

        private float fov = (float)Math.PI / 6;
        public float FOV
        {
            get => this.fov;
            set
            {
                if (this.fov != value)
                {
                    this.fov = value;
                    TransformAndRefresh();
                }
            }
        }

        private Vector3 cameraVector = new Vector3(2, 2, 2);
        public Vector3 CameraVector
        {
            get => this.cameraVector;
            set
            {
                if(this.cameraVector != value) 
                {
                    this.LightSource.SceneLocation = new Vector4(value, 0);
                    this.cameraVector = value;
                    TransformAndRefresh();
                }
            }
        }

        public int RenderThreads { get; set; }
        public LightSource LightSource { get; }
        public ColorPicker ColorPicker { get; }
        public bool Fill { get; set; } = false;
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

            this.rotationTimer = new();
            this.rotationTimer.Interval = 50;
            this.rotationTimer.Tick += RotationTimer_Tick;
            this.rotationTimer.Start();
        }

        private void RotationTimer_Tick(object? sender, EventArgs e)
        {
            TransformAndRefresh();
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
            List<Triangle> triangles = new List<Triangle>();
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

                triangles.Add(triangle);
                triangle = new();
            }

            var objectSize = new SizeF(Math.Abs(maxPoint.X - minPoint.X), Math.Abs(maxPoint.Y - minPoint.Y));
            this.objects.Add(new Object3(triangles, objectSize, this.objects.Count - 1));
            ObjectSize = new(Math.Max(ObjectSize.Width, objectSize.Width), Math.Max(ObjectSize.Height, objectSize.Height));
            RecalculateRenderScale();
            SetRenderScales();
            Refresh();
        }
        #endregion

        #region Rendering
        private void ParametersChangedHandler()
        {
            Refresh();
        }

        public override void Refresh()
        {
            if (freeze)
                return;

            using (var g = Graphics.FromImage(this.canvas.Bitmap))
            {
                g.Clear(Color.White);
            }

            if(Fill)
                this.canvas.ClearZBuffer();

            foreach (var obj in this.objects)
            {
                obj.Render(this.canvas, this.view, ShowLines, Fill ? ColorPicker : null);
            }

            LightSource.Render(this.canvas);

            base.Refresh();
        }

        private void ResizeTimerHandler(object? sender, EventArgs e)
        {
            this.resizeTimer.Stop();
            BitmpapSize = new Size(this.Width, this.Height);
            RecalculateRenderScale();
            SetRenderScales();
            this.LightSource.LightAnimation = this.wasAnimationTurnedOn;
            this.freeze = false;
            LightSource.RecalculateLightCoordinates();
            Refresh();
        }

        private void RecalculateRenderScale()
        {
            if (this.objects.Count == 0)
                return;

            this.rows = (int)Math.Sqrt(this.objects.Count);
            this.columns = (int)Math.Ceiling((float)this.objects.Count / rows);

            this.columnWidth = this.canvas.Width / columns;
            this.rowHeight = this.canvas.Height / rows;
            this.objectScale = 0.8f * Math.Min(this.columnWidth, this.rowHeight) / Math.Max(ObjectSize.Width, ObjectSize.Height);
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

        private void TransformAndRefresh()
        {
            SetRenderScales();
            Refresh();
        }

        private void SetRenderScales()
        {
            this.view = Matrix4x4.CreateLookAt(CameraVector, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            this.position = Matrix4x4.CreatePerspectiveFieldOfView(fov, (float)this.canvas.Width / this.canvas.Height, 1, 50);

            for (int i = 0; i < this.objects.Count; i++)
            {
                //var rect = new Rectangle(i % this.columns * this.columnWidth, i / this.columns * this.rowHeight, this.columnWidth, this.rowHeight);
                this.objects[i].SetRenderScale(this.canvas.Width, this.canvas.Height, this.view, this.position);
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

        public void Clear()
        {
            this.objects.Clear();
            Refresh();
            this.angleX = 0;
            this.angleY = 0;
            ObjectSize = new Size(0, 0);
        }
        #endregion

        #region Overrides
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.lightIsMoving = false;
            this.isRotating = false;

            if (LightSource.HitTest(e.X, e.Y))
            {
                this.lightIsMoving = true;
                wasAnimationTurnedOn = LightSource.LightAnimation;
                LightSource.LightAnimation = false;
            }
            else
            {
                this.isRotating = true;
                this.lastMousePosition = e.Location;
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
            
            if(this.isRotating)
            {
                var dx = e.Location.X - this.lastMousePosition.X;
                var dy = e.Location.Y - this.lastMousePosition.Y;

                this.angleX += (float)Math.PI / this.canvas.Height * dy;
                this.angleY += (float)Math.PI / this.canvas.Width * dx;

                this.angleX %= 2 * (float)Math.PI;
                this.angleY %= 2 * (float)Math.PI;

                SetRenderScales();
                Refresh();
            }
         
            this.lastMousePosition = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.lightIsMoving = false;
            this.isRotating = false;

            if (this.lightIsMoving)
            {
                LightSource.LightAnimation = wasAnimationTurnedOn;
                wasAnimationTurnedOn = false;
            }
        }
        #endregion
    }
}