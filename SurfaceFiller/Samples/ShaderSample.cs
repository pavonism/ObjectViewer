using SketcherControl.Filling;
using SurfaceFiller.Samples;

namespace ObjectViewer.Samples
{
    internal class ShaderSample : BasicSample
    {
        public Shader Shader { get; }

        public ShaderSample(string name, Shader shader) : base(name) 
        {
            this.Shader = shader;
        }
    }
}
