
using Microsoft.VisualBasic.Devices;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace SurfaceFiller.Components
{
    /// <summary>
    /// Umożliwia tworzenie paska z opcjami i guzikami
    /// </summary>
    public class Toolbar : FlowLayoutPanel
    {
        private TableLayoutPanel? currentRadioBox;

        public Toolbar()
        {
            Dock = DockStyle.Fill;
            Padding = Padding.Empty;
            Height = FormConstants.MinimumControlSize;
        }

        public void AddLabel(string text)
        {
            var label = new Label()
            {
                Font = new Font(DefaultFont, FontStyle.Bold),
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = FormConstants.MinimumControlSize,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
            };

            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = label.Height,
            };

            table.Controls.Add(label);
            Controls.Add(table);
        }

        public void AddDivider()
        {
            Controls.Add(new Divider());
        }

        public void AddSpacing()
        {
            Controls.Add(new Spacing(5));
        }

        private T AddSlider<T>(string labelText) where T : Slider, new()
        {
            var slider = new T()
            {
                Dock = DockStyle.Top,
                Width = this.Width,
                LabelText = labelText,
            };

            Controls.Add(slider);
            return slider;
        }

        private void AddTooltip(Control control, string? hint = null)
        {
            if (hint == null)
                return;

            var tooltip = new ToolTip();
            tooltip.SetToolTip(control, hint);
        }

        public void AddColorSlider(Action<int> handler, string labelText, int defaultValue = 0)
        {
            var slider = AddSlider<ColorSlider>(labelText);
            slider.ValueChanged += handler;
            slider.Value = defaultValue;
        }


        public void AddFractSlider(Action<float> handler, string labelText, float defaultValue = 0)
        {
            var slider = AddSlider<FractSlider>(labelText);
            slider.ValueChanged += handler;
            slider.Value = defaultValue;
        }

        public void AddSlider(Action<float> handler, string labelText, float defaultValue = 0)
        {
            var slider = AddSlider<PercentageSlider>(labelText);
            slider.ValueChanged += handler;
            slider.Value = defaultValue;
        }

        public void AddPlayPouse(Action<bool> handler, bool defaultState, string? hint = null)
        {
            var button = new PlayPouseButton();
            button.Lock = defaultState;
            AddTooltip(button, hint);
            button.OnOptionChanged += handler;
            Controls.Add(button);
        }

        public void AddProcessButton(Action handler, string glyph, string? hint = null)
        {
            var button = new ProcessButton(handler) { Text = glyph };
            AddTooltip(button, hint);
            Controls.Add(button);
        }

        public Button AddButton(EventHandler handler, string glyph, string? hint = null)
        {
            var button = new OptionButton()
            {
                Text = glyph,
                Margin = new Padding(2, 2, 2, 2),
            };

            AddTooltip(button, hint);
            button.Click += handler;
            Controls.Add(button);

            return button;
        }

        public CheckButton AddTool(Action<bool> handler, string glyph, string? hint = null)
        {
            var button = new CheckButton()
            {
                Width = FormConstants.MinimumControlSize,
                Height = FormConstants.MinimumControlSize,
                Margin = new Padding(2, 2, 2, 2),
                Text = glyph,
            };

            button.OnOptionChanged += handler;

            AddTooltip(button, hint);
            Controls.Add(button);
            return button;
        }

        public CheckBox AddOption(EventHandler onOptionChanged, string text, string? hint = null)
        {
            var checkBox = new CheckBox()
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = text,
            };

            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = checkBox.Height,
            };
            table.Controls.Add(checkBox);

            AddTooltip(checkBox, hint);

            checkBox.CheckedChanged += onOptionChanged;
            Controls.Add(table);
            return checkBox;
        }

        public ComboPickerWithImage<T> AddComboImagePicker<T>(Action<T> valuePickedHandler) where T : IComboItem
        {
            var combo = new ComboPickerWithImage<T>()
            {
                Width = this.Width - (int)(2.5 * FormConstants.MinimumControlSize),
            };

            combo.ValuePicked += valuePickedHandler;

            Controls.Add(combo);
            return combo;
        }

        public ComboPicker<T> AddComboPicker<T>(Action<T> valuePickedHandler)
        {
            var combo = new ComboPicker<T>()
            {
                Width = this.Width - (int)(2.5 * FormConstants.MinimumControlSize),
            };

            combo.ValuePicked += valuePickedHandler;

            Controls.Add(combo);
            return combo;
        }

        public void CreateNewRadioBox()
        {
            this.currentRadioBox = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
            };

            Controls.Add(this.currentRadioBox);
        }

        public void AddRadioOption(EventHandler clickHandler, string label, string? hint = null, bool? selected = null)
        {
            if (this.currentRadioBox == null)
                throw new Exception("RadioBox not found! Try to use CreateNewRadioBox()");

            var radio = new RadioButton()
            {
                Text = label,
                Width = this.Width,
            };

            if (selected.HasValue)
                radio.Checked = selected.Value;

            radio.Click += clickHandler;

            AddTooltip(radio, hint);
            this.currentRadioBox.Controls.Add(radio);
            this.currentRadioBox.Height = this.currentRadioBox.Controls.Count * FormConstants.MinimumControlSize;
        }
    }
}
