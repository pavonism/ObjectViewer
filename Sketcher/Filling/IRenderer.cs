using System.Numerics;

namespace SketcherControl.Filling
{
    public interface IRenderer
    {
        Size Size { get; }
        void Refresh();
        Vector4 Unscale(float x, float y, float z);
    }
}