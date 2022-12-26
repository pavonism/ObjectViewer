using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public class GouraudShader : Shader
    {
        public override void StartFillingTriangle(Polygon polygon)
        {
            foreach (var vertex in polygon.Vertices)
            {
                var textureColor = GetTextureColor((int)vertex.RenderLocation.X, (int)vertex.RenderLocation.Y);
                var normalVector = target.NormalMap != null ? GetNormalVectorFromNormalMap((int)vertex.RenderLocation.X, (int)vertex.RenderLocation.Y, vertex.NormalVector) : vertex.GlobalNormalVector;
                vertex.Color = CalculateColorInPoint(vertex.GlobalLocation, normalVector, textureColor);
                polygon.UpdateColorMatrix();
            }
        }

        public override Color GetColor(Polygon polygon, int x, int y)
        {
            return GetColorWithColorInterpolation(polygon, x, y);
        }

        private Color GetColorWithColorInterpolation(Polygon polygon, int x, int y)
        {
            var coefficients = Interpolator.GetBarycentricCoefficients(polygon, x, y);
            var color = Vector3.Transform(coefficients, polygon.colors);
            return color.ToColor();
        }
    }
}
