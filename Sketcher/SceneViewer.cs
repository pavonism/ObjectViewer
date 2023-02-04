using SketcherControl.Geometrics;
using SketcherControl.SceneManipulation;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl
{
    public class SceneViewer : PictureBox
    {
        private DirectBitmap sceneView;
        private SceneRenderer renderer;
        private Timer renderTimer;
        private Timer fpsTimer;
        private Timer dayNightTimer;

        private Vector3 backgroundColor = new Vector3(1, 1, 1);
        private SwitchState nightModeCycle;

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

        public float FogDistance { get; set; } = SceneConstants.MaximumViewDistance;
        public float FogIntensity { get; set; }
        public bool Fog { get; set; }

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

        private bool nightMode;
        public bool NightMode
        {
            get => this.nightMode;
            set
            {
                if (this.nightMode != value)
                {
                    this.nightMode = value;
                    nightModeCycle = value ? SwitchState.TurningOn : SwitchState.TurningOff;
                    this.dayNightTimer.Start();
                    UpdateBackground();
                    UpdateView();
                }
            }
        }

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

            this.renderTimer = new();
            this.renderTimer.Interval = 1;
            this.renderTimer.Tick += RenderTimer_Tick;
            this.renderTimer.Start();

            this.fpsTimer = new();
            this.fpsTimer.Interval = 1000;
            this.fpsTimer.Tick += FpsTimer_Tick;
            this.fpsTimer.Start();

            this.dayNightTimer = new();
            this.dayNightTimer.Interval = 40;
            this.dayNightTimer.Tick += DayNightTimer_Tick;

            var camera = new BaseCamera();
            camera.Apply(this);
        }

        #region Event Handlers
        private void DayNightTimer_Tick(object? sender, EventArgs e)
        {
            UpdateBackground();
        }

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

        private void RenderTimer_Tick(object? sender, EventArgs e)
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
                CameraVector = Camera.GetCameraVector(),
                ShowLines = ShowLines,
                Fill = Fill,
                Background = backgroundColor.ToColor(),
                Fog = Fog,
                ViewDistance = FogDistance,
                FogIntensity  = FogIntensity,
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
            this.renderTimer.Stop();
        }

        public void Thaw()
        {
            this.renderTimer.Start();
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
            if(e.Modifiers == Keys.Shift)
                Scene.MovingObject.Move(0, 0, -0.05f);

            switch (e.KeyCode)
            {
                case Keys.W:
                    Scene.MovingObject.Move(0, 0.05f, 0);
                    break;
                case Keys.S:
                    Scene.MovingObject.Move(0, -0.05f, 0);
                    break;
                case Keys.D:
                    Scene.MovingObject.Move(0.05f, 0,  0);
                    break;
                case Keys.A:
                    Scene.MovingObject.Move(-0.05f, 0, 0);
                    break;
                case Keys.Space:
                    Scene.MovingObject.Move(0, 0, 0.05f);
                    break;
                case Keys.Up:
                    Scene.MovingObject.RotateX((float)Math.PI / 20);
                    break;
                case Keys.Down:
                    Scene.MovingObject.RotateX(-(float)Math.PI / 20);
                    break;
                case Keys.Left:
                    Scene.MovingObject.RotateZ((float)Math.PI / 20);
                    break;
                case Keys.Right:
                    Scene.MovingObject.RotateZ(-(float)Math.PI / 20);
                    break;
                case Keys.K:
                    Scene.Reflector.TurnLeft();
                    break;
                case Keys.L:
                    Scene.Reflector.TurnRight();
                    break;
            }

            base.OnPreviewKeyDown(e);
        }
        #endregion

        #region Scene manipulation 
        private void UpdateBackground()
        {
            switch (nightModeCycle)
            {
                case SwitchState.TurningOff:
                    if (backgroundColor.X == 0)
                        backgroundColor = new Vector3(0.01f, 0.01f, 0.01f);
                    if (backgroundColor.X >= 1)
                    {
                        nightModeCycle = SwitchState.Off;
                        this.dayNightTimer.Stop();
                    }
                    backgroundColor *= 1.05f;
                    break;
                case SwitchState.TurningOn:
                    backgroundColor /= 1.05f;
                    if (backgroundColor.X < 0.01f)
                    {
                        backgroundColor = new Vector3(0, 0, 0);
                        nightModeCycle = SwitchState.On;
                        this.dayNightTimer.Stop();
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        private enum SwitchState
        {
            On,
            Off,
            TurningOn,
            TurningOff
        }
    }
}