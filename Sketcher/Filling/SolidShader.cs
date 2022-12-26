using SketcherControl.Shapes;

namespace SketcherControl.Filling
{
    public class SolidShader : Shader
    {
        public override Color GetColor(Polygon polygon, int x, int y)
        {
            return this.target.Color;
        }

        public override void StartFillingTriangle(Polygon polygon)
        {
        }
    }
}
