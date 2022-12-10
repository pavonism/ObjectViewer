
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace SurfaceFiller.Components
{
    public interface IComboItem
    {
        Image GetThumbnail(int width, int height);
    }

    public class ComboPickerWithImage<T> : ComboPicker<T> where T : IComboItem
    {
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if(e.Index >= 0)
            {
                Rectangle bounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width / 3, e.Bounds.Height);
                e.Graphics.DrawImage(((T)this.Items[e.Index]).GetThumbnail(e.Bounds.Width / 3, e.Bounds.Height), bounds);
                RectangleF textBounds = new Rectangle(e.Bounds.Left + e.Bounds.Width / 3, e.Bounds.Top, 2 * e.Bounds.Width / 3, e.Bounds.Height);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(this.Items[e.Index].ToString(), new Font(e.Font.Name, 7), new SolidBrush(e.ForeColor), textBounds, stringFormat);
            }
        }
    }

    public class ComboPicker<T> : ComboBox
    {
        private T? defaultValue;

        public T? DefaultValue
        {
            get => this.defaultValue;
            set
            {
                this.defaultValue = value;
                this.SelectedItem = value;
            }
        }

        public void AddOptions(IEnumerable<T> options)
        {
            Items.AddRange(options.Cast<object>().ToArray());
        }

        public void AddOption(T newOption)
        {
            Items.Add(newOption);
        }

        public void Select(T option)
        {
            this.SelectedIndex = Items.IndexOf(option);
        }

        public void AddAndSelect(T newOption)
        {
            Items.Add(newOption);
            Select(newOption);
        }

        public event Action<T>? ValuePicked;

        public ComboPicker()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            ItemHeight = FormConstants.MinimumControlSize;
            Height = FormConstants.MinimumControlSize;
            BackColor = Color.White;
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            this.ValuePicked?.Invoke((T)SelectedItem);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            SelectionLength = 0;
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                RectangleF textBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(this.Items[e.Index].ToString(), new Font(e.Font.Name, 7), new SolidBrush(e.ForeColor), textBounds, stringFormat);
            }

            base.OnDrawItem(e);
        }
    }
}
