
using Microsoft.VisualBasic.Devices;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using ToolbarControls;

namespace SurfaceFiller.Components
{
    /// <summary>
    /// Umożliwia tworzenie paska z opcjami i guzikami
    /// </summary>
    public class Toolbar : FlowLayoutPanel
    {
        private Stack<FlowLayoutPanel> sections = new();

        public Toolbar()
        {
            Dock = DockStyle.Fill;
            Padding = Padding.Empty;
        }

        private void AddControl(Control control)
        {
            if (sections.Count == 0)
                Controls.Add(control);
            else
            {
                sections.Peek().Controls.Add(control);
                sections.Peek().Height += control.Height;
            }
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
            AddControl(table);
        }

        public void AddDivider()
        {
            AddControl(new Divider());
        }

        public void AddSpacing()
        {
            AddControl(new Spacing(5));
        }

        private T AddSlider<T>(string labelText) where T : Slider, new()
        {
            var slider = new T()
            {
                Dock = DockStyle.Top,
                Width = this.Width,
                LabelText = labelText,
            };

            AddControl(slider);
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

        public void AddRationSlider(Action<float> handler, string labelText1, string labelText2, float defaultValue = 0)
        {
            var slider = new RatioSlider(labelText1, labelText2)
            {
                Dock = DockStyle.Top,
                Width = this.Width,
            };

            AddControl(slider);
            slider.ValueChanged += handler;
            slider.Value = defaultValue;
        }

        public void AddPlayPouse(Action<bool> handler, bool defaultState, string? hint = null)
        {
            var button = new PlayPouseButton();
            button.Lock = defaultState;
            AddTooltip(button, hint);
            button.OnOptionChanged += handler;
            AddControl(button);
        }

        public void AddProcessButton(Action handler, string glyph, string? hint = null)
        {
            var button = new ProcessButton(handler) { Text = glyph };
            AddTooltip(button, hint);
            AddControl(button);
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
            AddControl(button);

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
            AddControl(button);
            return button;
        }

        public CheckBox AddOption(Action<bool> onOptionChanged, string text, string? hint = null, bool defaultValue = false)
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

            checkBox.CheckedChanged += (s, e) => onOptionChanged(checkBox.Checked);
            checkBox.Checked = defaultValue;
            AddControl(table);
            return checkBox;
        }

        public ComboPickerWithImage<T> AddComboImagePicker<T>(Action<T> valuePickedHandler) where T : IComboItem
        {
            var combo = new ComboPickerWithImage<T>()
            {
                Width = this.Width - (int)(2.5 * FormConstants.MinimumControlSize),
            };

            combo.ValuePicked += valuePickedHandler;

            AddControl(combo);
            return combo;
        }

        public ComboPicker<T> AddComboPicker<T>(Action<T> valuePickedHandler, T[] values, T defaul)
        {
            var combo = new ComboPicker<T>()
            {
                Width = this.Width - Margin.Left - Margin.Right,
            };

            combo.ValuePicked += valuePickedHandler;
            combo.AddOptions(values);
            combo.Select(defaul);

            AddControl(combo);
            return combo;
        }

        public ICombo<T> AddComboApply<T>(Action<T> applyHandler, string? buttonLabel = null, string? hint = null)
        {
            var comboApply = new ComboApply<ComboPicker<T>, T>();
            comboApply.Width = Width;
            comboApply.Height = FormConstants.MinimumControlSize + 10;
            comboApply.Apply += applyHandler;
            comboApply.Hint = hint;
            comboApply.ButtonLabel = buttonLabel;

            comboApply.Margin = new Padding(0, 5, 0, 5);
            AddControl(comboApply);
            return comboApply;
        }

        public FlowLayoutPanel StartSection()
        {
            var newSection = new FlowLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = FormConstants.MinimumControlSize,
                Width = this.Width
            };

            this.sections.Push(newSection);
            Controls.Add(newSection);
            return newSection;
        }

        public void EndSection()
        {
            this.sections.Pop();
        }

        public void AddRadioOption(EventHandler clickHandler, string label, string? hint = null, bool? selected = null)
        {
            var radio = new RadioButton()
            {
                Text = label,
                Width = this.Width - Margin.Left - Margin.Right,
            };

            if (selected.HasValue)
                radio.Checked = selected.Value;

            radio.Click += clickHandler;

            AddTooltip(radio, hint);
            AddControl(radio);
        }
    }
}
