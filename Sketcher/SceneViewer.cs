using SketcherControl.Filling;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl
{
    public class SceneViewer : PictureBox
    {
        private DirectBitmap sceneView;
        private SceneRenderer renderer;
        private Scene scene;
        private Timer rotationTimer;
        private Timer fpsTimer;

        private Matrix4x4 view;
        private Matrix4x4 position;

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

        private Vector3 cameraVector = new Vector3(2, 2, 2);
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
            this.scene = scene;
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
        }

        #region Event Handlers
        private void OnRenderFinished(DirectBitmap sceneRender)
        {
            this.sceneView.Dispose();
            this.sceneView = sceneRender;

            using (var g = Graphics.FromImage(sceneRender.Bitmap))
            {
                using(var font = new Font(DefaultFont.Name, 20, FontStyle.Bold))
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
            if (this.scene.IsEmpty)
                return;

            SceneRenderParameters renderParameters = new()
            {
                View = this.view,
                Position = this.position,
                ViewWidth = Width,
                ViewHeight = Height,
                LookVector = -this.cameraVector,
                ShowLines = ShowLines,
                Fill = Fill
            };

            this.renderer.Render(this.scene, renderParameters);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            UpdateMatrices();
        }

        private void UpdateMatrices()
        {
            this.view = Matrix4x4.CreateLookAt(CameraVector, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            this.position = Matrix4x4.CreatePerspectiveFieldOfView(fov, (float)Width / Height, 1, 50);
        }
        #endregion
    }
}