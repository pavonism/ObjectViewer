using SketcherControl.SceneManipulation;
using SurfaceFiller.Samples;

namespace ObjectViewer.Samples
{
    public class CameraSample : BasicSample
    {
        public CameraSample(string name, ICamera camera) : base(name)
        {
            Camera = camera;
        }

        public ICamera Camera { get; }
    }
}
