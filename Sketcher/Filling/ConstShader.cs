using SketcherControl.Geometrics;
using SketcherControl.Shapes;

namespace SketcherControl.Filling
{
    public class ConstShader : Shader
    {
        public override void StartFillingTriangle(Polygon polygon)
        {
            var meanCoordinates = polygon.GetMeanCoordinates();
            var meanNormal = polygon.GetMeanNormal();
            var meanRender = polygon.GetMeanRenderCoordinates();
            var color = GetTextureColor((int)meanRender.X, (int)meanRender.Y);
            polygon.ConstShaderColor = CalculateColorInPoint(meanCoordinates, meanNormal, color).ToColor();
        }

        public override Color GetColor(Polygon polygon, int x, int y)
        {
            return polygon.ConstShaderColor;
        }
    }
}
