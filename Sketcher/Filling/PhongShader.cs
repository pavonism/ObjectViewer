using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public class PhongShader : Shader
    {
        public override void StartFillingTriangle(Polygon polygon)
        {

        }

        public override Color GetColor(Polygon polygon, int x, int y)
        {
            return GetColorWithVectorInterpolation(polygon, x, y);
        }

        private Color GetColorWithVectorInterpolation(Polygon polygon, int x, int y)
        {
            var coefficients = Interpolator.GetBarycentricCoefficients(polygon, x, y);
            var normalVector = GetNormalVector(polygon, x, y, coefficients);
            var textureColor = GetTextureColor(x, y);
            var xi = Interpolator.InterpolateX(polygon, coefficients);
            var yi = Interpolator.InterpolateY(polygon, coefficients);
            var zi = Interpolator.InterpolateZ(polygon, coefficients);
            var renderLocation = new Vector4(xi, yi, zi, 0);
            return CalculateColorInPoint(renderLocation, normalVector, textureColor).ToColor();
        }
    }
}
