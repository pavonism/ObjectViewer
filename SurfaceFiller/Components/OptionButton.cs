using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace SurfaceFiller.Components
{
    public class PlayPouseButton : CheckButton
    {
        public override bool Lock
        {
            get => this.ticked;
            set
            {
                this.ticked = value;
                this.Text = value ? Glyphs.Pause : Glyphs.Play;

            }
        }
    }

    public class ProcessButton : OptionButton
    {
        private Timer _timer = new Timer();
        private Action processAction;

        public ProcessButton(Action processAction)
        {
            _timer.Interval = 32;
            _timer.Tick += _timer_Tick;
            this.processAction = processAction;
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            this.processAction?.Invoke();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _timer.Start();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _timer.Stop();
        }
    }

    /// <summary>
    /// Implementuje okrągły przycisk w formie checkboxa
    /// </summary>
    public class CheckButton : OptionButton
    {
        #region Fields and Properties
        protected bool ticked;

        public virtual bool Lock
        {
            get => ticked;
            set
            {
                ticked = value;
                BackColor = value ? Color.FromArgb(50, 0, 120, 215) : Color.Transparent;
                FlatAppearance.BorderSize = value ? 1 : 0;
            }
        }
        #endregion

        #region Events
        public event Action<bool>? OnOptionChanged;

        private void OptionChanged(object? sender, EventArgs e)
        {
            Lock = !Lock;
            OnOptionChanged?.Invoke(ticked);
        }
        #endregion

        public CheckButton()
        {
            this.Click += OptionChanged;
        }
    }

    public class RoundedButton : OptionButton
    {
        private bool mouseIsHovering;

        public RoundedButton()
        {
            this.Height = int.MinValue;
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mouseIsHovering = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.mouseIsHovering = true;
            base.OnMouseEnter(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(new SolidBrush(Resources.ThemeColor), RoundedRect(e.ClipRectangle, (int)(0.1 * e.ClipRectangle.Width)));

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(Text, Font, new SolidBrush(mouseIsHovering ? Color.LightGray : ForeColor), e.ClipRectangle, stringFormat);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Implementuje okrągły przycisk
    /// </summary>
    public class OptionButton : Button
    {
        public OptionButton()
        {
            Width = FormConstants.MinimumControlSize;
            Height = FormConstants.MinimumControlSize;
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            FlatStyle = FlatStyle.Flat;
            TextAlign = ContentAlignment.MiddleCenter;
            Font = new Font("Arial", 14);
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
        }
    }
}
