using SketcherControl.Shapes;
namespace SketcherControl.Filling
{
    public interface IShader
    {
        void StartFillingTriangle(Polygon polugon);
        public Color GetColor(Polygon polygon, int x, int y);
    }
}
