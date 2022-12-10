
namespace SurfaceFiller.Samples
{
    internal class ColorSample : Sample
    {
        public Color Color { get; set; }

        public ColorSample(Color color, string name) : base(name)
        {
            Color = color;
        }

        public override Image GetThumbnail(int width, int height)
        {
            var thumbnail = new Bitmap(width, height);

            using (Graphics gfx = Graphics.FromImage(thumbnail))
            using (SolidBrush brush = new SolidBrush(Color))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
            }

            return thumbnail;
        }
    }
}
