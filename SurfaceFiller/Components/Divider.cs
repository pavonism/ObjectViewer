
namespace SurfaceFiller.Components
{

    /// <summary>
    /// Implementuje obiekt rozdzielający opcje dostępne na pasku z nadzędziami
    /// </summary>
    internal class Divider : Label
    {
        private const int DividerHeight = 2;
        private const int HorizontalPadding = 8;

        public Divider()
        {
            Text = string.Empty;
            BorderStyle = BorderStyle.Fixed3D;
            AutoSize = false;
            Height = DividerHeight;
            Width = FormConstants.ToolbarWidth;
        }
    }
}
