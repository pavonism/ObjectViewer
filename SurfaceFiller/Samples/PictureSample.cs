using SketcherControl;

namespace SurfaceFiller.Samples
{
    internal class PictureSample : Sample
    {
        public DirectBitmap? Image { get; private set; }

        public PictureSample(Bitmap? image, string name) : base(name)
        {
            if(image != null)
                Image = new(image);
        }

        public override Image GetThumbnail(int width, int height)
        {
            if (Image == null)
                return new Bitmap(1, 1);

            var thumbnail = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var color = Image.GetPixel(i % Image.Width, j % Image.Height);
                    thumbnail.SetPixel(i, j, color);
                }
            }

            return thumbnail;
        }

    }
}
