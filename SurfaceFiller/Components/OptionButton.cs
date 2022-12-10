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
