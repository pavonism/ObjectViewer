using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceFiller.Samples
{
    internal class PictureSample : Sample
    {
        public Bitmap? Image { get; private set; }

        public PictureSample(Bitmap? image, string name) : base(name)
        {
            Image = image;
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
