
namespace SurfaceFiller.Components
{
    public class FractSlider : Slider<float>
    {
        public FractSlider() : base()
        {
            this.trackBar.Minimum = 0;
            this.trackBar.Maximum = 100;
        }

        public override float Value
        {
            get => (float)this.trackBar.Value / 100;
            set => this.trackBar.Value = (int)(value * 100);
        }

        protected override void UpdateLabelText()
        {
            this.valueLabel.Text = $"{(float)this.trackBar.Value / 100}";
        }

    }

    public class ColorSlider : Slider<int>
    {
        public ColorSlider() : base()
        {
            this.trackBar.Minimum = 0;
            this.trackBar.Maximum = 255;
        }

        public override int Value
        {
            get => this.trackBar.Value;
            set => this.trackBar.Value = value;
        }

        protected override void UpdateLabelText()
        {
            this.valueLabel.Text = $"{this.trackBar.Value}";
        }
    }

    public class PercentageSlider : Slider<float>
    {
        public PercentageSlider()
        {
            this.trackBar.Minimum = 0;
            this.trackBar.Maximum = 100;
        }

        public override float Value
        {
            get => (float)this.trackBar.Value / 100;
            set => this.trackBar.Value = (int)(value * 100);
        }

        protected override void UpdateLabelText()
        {
            this.valueLabel.Text = $"{this.trackBar.Value}%";
        }
    }

    public abstract class Slider<T> : Slider
    {
        protected Slider()
        {
            this.trackBar.ValueChanged += TrackBar_ValueChanged;
        }

        public abstract T Value { get; set; }

        public event Action<T>? ValueChanged;
        private void TrackBar_ValueChanged(object? sender, EventArgs e)
        {
            UpdateLabelText();
            this.ValueChanged?.Invoke(this.Value);
        }
    }

    public abstract class Slider : UserControl
    {
        public string LabelText
        {
            get => this.label.Text;
            set
            {
                this.label.Text = value;
            }
        }

        protected readonly TrackBar trackBar = new()
        {
            TickFrequency = 1,
            SmallChange = 1,
            LargeChange = 1,
            MaximumSize = new Size(FormConstants.SliderWidth, FormConstants.MinimumControlSize),
            AutoSize = false,
            TickStyle = TickStyle.None,
        };

        protected readonly TableLayoutPanel mainTable = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
        };

        protected readonly Label label = new()
        {
            AutoSize = false,
            Width = FormConstants.LabelWidth,
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Left,
            Dock = DockStyle.Top,
        };

        protected readonly Label valueLabel = new()
        {
            AutoSize = false,
            Width = FormConstants.LabelWidth,
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Left,
            Dock = DockStyle.Top,
        };

        protected Slider()
        {
            this.Height = FormConstants.MinimumControlSize;
            this.Margin = new Padding(0, 5, 0, 5);

            UpdateLabelText();

            mainTable.Controls.Add(label);
            mainTable.Controls.Add(trackBar);
            mainTable.Controls.Add(valueLabel);

            this.mainTable.SetColumn(label, 0);
            this.mainTable.SetColumn(trackBar, 1);
            this.mainTable.SetColumn(valueLabel, 2);

            this.Controls.Add(mainTable);
        }

        protected abstract void UpdateLabelText();
    }
}
