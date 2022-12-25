using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Geometrics
{
    internal static class Interpolator
    {
        public static void CalculateBarycentricCache(Polygon polygon) 
        {
            Vector2 v0 = new()
            {
                X = polygon.Vertices[1].RenderLocation.X - polygon.Vertices[0].RenderLocation.X,
                Y = polygon.Vertices[1].RenderLocation.Y - polygon.Vertices[0].RenderLocation.Y,
            };

            Vector2 v1 = new()
            {
                X = polygon.Vertices[2].RenderLocation.X - polygon.Vertices[0].RenderLocation.X,
                Y = polygon.Vertices[2].RenderLocation.Y - polygon.Vertices[0].RenderLocation.Y,
            };

            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);

            float denom = d00 * d11 - d01 * d01;

            polygon.BarycentricCache = new()
            {
                v0 = v0,
                v1 = v1,
                d00 = d00,
                d01 = d01,
                d11 = d11,
                denom = denom
            };
        }

        public static Vector3 CalculateCoefficients(Polygon polygon, int x, int y)
        {
            if (polygon.BarycentricCache.denom < 1)
                return new Vector3(1, 1, 1);

            Vector2 v2 = new()
            {
                X = x - polygon.Vertices[0].RenderLocation.X,
                Y = y - polygon.Vertices[0].RenderLocation.Y,
            };

            float d20 = Vector2.Dot(v2, polygon.BarycentricCache.v0);
            float d21 = Vector2.Dot(v2, polygon.BarycentricCache.v1);

            var v = (polygon.BarycentricCache.d11 * d20 - polygon.BarycentricCache.d01 * d21) / polygon.BarycentricCache.denom;
            var w = (polygon.BarycentricCache.d00 * d21 - polygon.BarycentricCache.d01 * d20) / polygon.BarycentricCache.denom;
            var u = 1.0f - v - w;

            return new Vector3(w, u, v);
        }
    }
}
