using SketcherControl.SceneManipulation;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl
{
    public class SceneViewer : PictureBox
    {
        private DirectBitmap sceneView;
        private SceneRenderer renderer;
        private Timer rotationTimer;
        private Timer fpsTimer;

        public Scene Scene { get; private set; }
        public Matrix4x4 View { get; set; }
        public Matrix4x4 Position { get; set; }

        private ICamera camera;
        public ICamera Camera
        {
            get => this.camera;
            set
            {
                if (this.camera != value)
                {
                    this.camera = value;
                    RefreshView();
                }
            }
        }

        private int frames;
        private int fps;

        private float fov = (float)Math.PI / 6;
        public float FOV
        {
            get => this.fov;
            set
            {
                if (this.fov != value)
                {
                    this.fov = value;
                    UpdateView();
                }
            }
        }

        private Vector3 cameraVector = new Vector3(4f, 4f, 2f);
        public Vector3 CameraVector
        {
            get => this.cameraVector;
            set
            {
                if (this.cameraVector != value)
                {
                    this.cameraVector = value;
                    UpdateView();
                }
            }
        }

        public int RenderThreads { get; set; }

        public bool Fill { get; set; } = false;

        public bool ShowLines { get; set; } = false;

        public SceneViewer(Scene scene)
        {
            this.Scene = scene;
            this.sceneView = new(this.Width, this.Height);
            this.renderer = new();
            this.renderer.RenderFinished += OnRenderFinished;
            this.Dock = DockStyle.Fill;

            this.Image = this.sceneView.Bitmap;

            this.rotationTimer = new();
            this.rotationTimer.Interval = 1;
            this.rotationTimer.Tick += RotationTimer_Tick;
            this.rotationTimer.Start();

            this.fpsTimer = new();
            this.fpsTimer.Interval = 1000;
            this.fpsTimer.Tick += FpsTimer_Tick;
            this.fpsTimer.Start();

            var camera = new BaseCamera();
            camera.Apply(this);
        }

        #region Event Handlers
        private void OnRenderFinished(DirectBitmap sceneRender)
        {
            this.sceneView.Dispose();
            this.sceneView = sceneRender;

            using (var g = Graphics.FromImage(sceneRender.Bitmap))
            {
                using (var font = new Font(DefaultFont.Name, 20, FontStyle.Bold))
                {
                    g.DrawString(this.fps.ToString(), font, Brushes.Lime, sceneRender.Width - 40, 2);
                }
            }

            Image = this.sceneView.Bitmap;
            this.frames++;
        }

        private void FpsTimer_Tick(object? sender, EventArgs e)
        {
            this.fps = this.frames;
            this.frames = 0;
        }

        private void RotationTimer_Tick(object? sender, EventArgs e)
        {
            RefreshView();
        }
        #endregion Event Handlers

        #region Rendering
        private void UpdateView()
        {
            UpdateMatrices();
            RefreshView();
        }

        public void RefreshView()
        {
            if (Scene.IsEmpty)
                return;


            SceneRenderParameters renderParameters = new()
            {
                View = Camera.GetViewMatrix(),
                Position = Camera.GetPositionMatrix(),
                ViewWidth = Width,
                ViewHeight = Height,
                LookVector = Camera.GetLookVector(),
                ShowLines = ShowLines,
                Fill = Fill
            };

            this.renderer.Render(Scene, renderParameters);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            Camera.CameraScreenChanged(Width, Height);
            UpdateMatrices();
        }

        private void UpdateMatrices()
        {
            this.View = Matrix4x4.CreateLookAt(CameraVector, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            this.Position = Matrix4x4.CreatePerspectiveFieldOfView(fov, (float)Width / Height, 1, 50);
        }

        public void Freeze()
        {
            this.rotationTimer.Stop();
        }

        public void Thaw()
        {
            this.rotationTimer.Start();
        }

        private float speed = 0.1f;

        public void MoveCamera(int x, int y)
        {
            this.cameraVector.X += x * speed;
            this.cameraVector.Y += y * speed;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Focus();
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.W:
                    MoveCamera(0, 1);
                    break;
                case Keys.S:
                    MoveCamera(0, -1);
                    break;
                case Keys.D:
                    MoveCamera(-1, 0);
                    break;
                case Keys.A:
                    MoveCamera(1, 0);
                    break;
            }
        }
        #endregion
    }
}