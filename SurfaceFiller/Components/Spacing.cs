
namespace SurfaceFiller.Components
{
    internal class Spacing : Divider
    {
        public Spacing(int space)
        {
            Margin = new Padding(0, space, 0, 0);
            Height = 0;
        }
    }
}
