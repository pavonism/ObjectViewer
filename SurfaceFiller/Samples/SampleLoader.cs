using SketcherControl;

namespace SurfaceFiller.Samples
{
    internal class SampleGenerator
    {
        public static IEnumerable<Sample> GetTextures(string directory, out ColorSample colorSample)
        {
            List<Sample> samples = new();
            colorSample = CreateColorSample();
            samples.Add(colorSample);
            samples.AddRange(GetImageSamples(directory));
            return samples;
        }

        public static IEnumerable<Sample> GetNormalMaps(string directory)
        {
            List<Sample> samples = new();
            samples.Add(new PictureSample(null, "None"));
            samples.AddRange(GetImageSamples(directory));
            return samples;
        }

        public static IEnumerable<Sample> GetImageSamples(string directory)
        {
            List<Sample> samples = new();

            foreach (string imageFileName in Directory.GetFiles(directory))
            {
                samples.Add(GetSample(imageFileName));
            }

            return samples;
        }

        public static IEnumerable<BasicSample> GetObjectSamples(string directory)
        {
            List<BasicSample> samples = new();

            foreach (string fileName in Directory.GetFiles(directory, "*.obj"))
            {
                var sample = new ObjectSample(fileName, Path.GetFileNameWithoutExtension(fileName));
                if(sample.Name == Resources.DefaultObject)
                    samples.Insert(0, sample);
                else 
                    samples.Add(sample);
            }

            return samples;
        }

        public static PictureSample GetSample(string path)
        {
            Bitmap bmp = new Bitmap(Image.FromFile(path));
            return new PictureSample(bmp, Path.GetFileNameWithoutExtension(path));
        }

        public static ObjectSample CreateObjectSample(string path)
        {
            return new ObjectSample(path, Path.GetFileNameWithoutExtension(path));
        }

        public static ColorSample CreateColorSample()
        {
            return new ColorSample(SketcherConstants.ThemeColor, "Kolor");
        }
    }
}
