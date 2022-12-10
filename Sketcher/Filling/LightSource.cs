using SketcherControl.Geometrics;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl.Filling
{
    public class LightSource
    {
        #region Fields and Events
        public IRenderer Renderer { get; private set; }

        public event Action? LightSourceChanged;

        private Vector4 lightSourceColor = new Vector4(1, 1, 1, 0);

        /// <summary>
        /// Współrzędne względem rozmiaru bitmapy
        /// </summary>
        private float lightLocationX = 0.5f, lightLocationY = 0.5f, lightLocationZ = 0.5f;

        float lightAngle = 0;
        int xSun;
        int ySun;
        private bool showTrack;
        private MoveDirection moveDirection = MoveDirection.Forward;
        private Timer timer = new();
        #endregion

        #region Properties
        public float MinZ { get; set; } = 1f;
        public Vector4 Location => Renderer.Unscale(xSun, Renderer.Size.Height - ySun, MinZ + 3 * MinZ * lightLocationZ);

        public bool ShowTrack
        {
            get => this.showTrack;
            set
            {
                if (this.showTrack == value)
                    return;

                this.showTrack = value;
                this.LightSourceChanged?.Invoke();
            }
        }

        public Color LightSourceColor
        {
            get => this.lightSourceColor.ToColor();
            set
            {
                if (this.lightSourceColor.ToColor() == value)
                    return;

                this.lightSourceColor = value.ToVector();
                this.LightSourceChanged?.Invoke();
            }
        }


        public bool LightAnimation
        {
            get => timer.Enabled;
            set
            {
                if (value)
                    timer.Start();
                else
                    timer.Stop();
            }
        }

        private float lightSpeed = 0.5f;
        public float LightSpeed
        {
            get => lightSpeed;
            set
            {
                this.lightSpeed = value;
            }
        }

        public float LightLocationX
        {
            get => this.lightLocationX;
            set
            {
                if (this.lightLocationX == value)
                    return;

                this.lightLocationX = value;
                RecalculateLightCoordinates();
                this.LightSourceChanged?.Invoke();
            }
        }

        public float LightLocationY
        {
            get => this.lightLocationY;
            set
            {
                if (this.lightLocationY == 1 - value)
                    return;

                this.lightLocationY = 1 - value;
                RecalculateLightCoordinates();
                this.LightSourceChanged?.Invoke();
            }
        }

        public float LightLocationZ
        {
            get => this.lightLocationZ;
            set
            {
                if (this.lightLocationZ == value)
                    return;

                this.lightLocationZ = value;
                this.LightSourceChanged?.Invoke();
            }
        }

        public Vector4 LightSourceVector => this.lightSourceColor;
        public Vector4 CanvasCoordinates => new Vector4(xSun, ySun, LightLocationZ, 0);
        #endregion Properties

        #region Initialization
        public LightSource(IRenderer renderer)
        {
            this.Renderer = renderer;
            this.xSun = Renderer.Size.Width;
            this.ySun = Renderer.Size.Height;

            this.timer.Interval = 32;
            this.timer.Tick += Timer_Tick;
            timer.Start();
        }
        #endregion

        #region Rendering
        public void Render(DirectBitmap canvas)
        {
            if (Renderer.Size.Width > xSun && xSun > 0 && ySun > 0 && ySun < Renderer.Size.Height)
                using (var g = Graphics.FromImage(canvas.Bitmap))
                {
                    var size = TextRenderer.MeasureText(SketcherConstants.LightSource, new Font(Control.DefaultFont.Name, 20, FontStyle.Bold));
                    var brush = Brushes.Gold;
                    g.DrawString(SketcherConstants.LightSource, new Font(Control.DefaultFont.Name, 20, FontStyle.Bold), brush, xSun - size.Width / 2, ySun - size.Height / 2);
                }

            if (ShowTrack)
            {
                float currentAngle = 0f;
                int currentXLight = (int)(LightLocationX * Renderer.Size.Width), currentYLight = (int)(Renderer.Size.Height * LightLocationY);
                while (currentAngle < this.lightAngle)
                {
                    var omega = SketcherConstants.LightSourceSpeedCoefficient * LightSpeed / (float)(2 * Math.PI * DistanceFromStart(xSun, ySun));
                    currentAngle += Math.Max(omega, SketcherConstants.MinLightAngleIncrease) * this.timer.Interval / 1000;
                    currentXLight = (int)(Math.Cos(currentAngle) * currentAngle) + (int)(Renderer.Size.Width * LightLocationX);
                    currentYLight = (int)(Math.Sin(currentAngle) * currentAngle) + (int)(Renderer.Size.Height * LightLocationY);

                    if (Renderer.Size.Width <= currentXLight || currentXLight <= 0 || currentYLight <= 0 || currentYLight >= Renderer.Size.Height)
                        continue;
                    canvas.SetPixel(currentXLight, canvas.Height - currentYLight, LightSourceColor == Color.White ? Color.Gold : LightSourceColor);
                }
            }
        }

        public bool HitTest(int x, int y)
        {
            return Math.Pow(Math.Abs(x - xSun), 2) + Math.Pow(Math.Abs(y - ySun), 2) <= Math.Pow(SketcherConstants.LightHitTestRadius, 2);
        }

        public void MoveTo(int x, int y)
        {
            Reset();
            LightLocationX = (float)x / Renderer.Size.Width;
            LightLocationY = 1 - (float)y / Renderer.Size.Height;
        }
        #endregion

        #region Animation
        private void Timer_Tick(object? sender, EventArgs e)
        {
            MoveLight(moveDirection == MoveDirection.Backward);
            this.LightSourceChanged?.Invoke();
        }

        public void MoveLight(bool backwards = false)
        {
            var dist = DistanceFromStart(xSun, ySun);

            if (dist < 1)
            {
                if (backwards)
                    this.lightAngle = Math.Max(0, this.lightAngle - 0.2f);
                else
                    this.lightAngle += 0.2f;
            }
            else
            {
                var omega = SketcherConstants.LightSourceSpeedCoefficient * Math.Min(Renderer.Size.Width, Renderer.Size.Height) * LightSpeed / (float)(2 * Math.PI * dist);
                if (backwards)
                    this.lightAngle = Math.Max(0, this.lightAngle - omega * this.timer.Interval / 1000);
                else
                    this.lightAngle += omega * this.timer.Interval / 1000;
            }
            RecalculateLightCoordinates();
        }

        public void RecalculateLightCoordinates()
        {
            xSun =  (int)(Math.Cos(lightAngle) * this.lightAngle) + (int)(Renderer.Size.Width * LightLocationX);
            ySun = (int)(Math.Sin(lightAngle) * this.lightAngle) + (int)(Renderer.Size.Height * LightLocationY);

            if (xSun <= 0 || xSun >= Renderer.Size.Width || ySun <= 0 || ySun >= Renderer.Size.Height)
            {
                this.moveDirection = MoveDirection.Backward;
            }
            else if (DistanceFromStart(xSun, ySun) < 1)
            {
                this.moveDirection = MoveDirection.Forward;
            }
        }

        private float DistanceFromStart(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(x - Renderer.Size.Width * LightLocationX, 2) + Math.Pow(y - Renderer.Size.Height * LightLocationY, 2));
        }

        public void Reset()
        {
            xSun = (int)(LightLocationX * Renderer.Size.Width);
            ySun = (int)(LightLocationY * Renderer.Size.Height);

            this.lightAngle = 0f;
            this.LightSourceChanged?.Invoke();
        }
        #endregion
    }

    internal enum MoveDirection
    {
        Forward,
        Backward,
    }
}
