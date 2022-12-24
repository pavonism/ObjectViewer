using SketcherControl.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SketcherControl.Geometrics
{
    internal class Interpolator
    {
        public Vector3 CalculateCoefficients(Polygon polygon, int x, int y)
        {
            Vector2 v0 = new()
            {
                X = polygon.Vertices[1].RenderLocation.X - polygon.Vertices[0].RenderLocation.X,
                Y = polygon.Vertices[1].RenderLocation.Y - polygon.Vertices[0].RenderLocation.Y,
            };

            Vector2 v1= new()
            {
                X = polygon.Vertices[2].RenderLocation.X - polygon.Vertices[0].RenderLocation.X,
                Y = polygon.Vertices[2].RenderLocation.Y - polygon.Vertices[0].RenderLocation.Y,
            };

            Vector2 v2 = new()
            {
                X = x - polygon.Vertices[0].RenderLocation.X,
                Y = y - polygon.Vertices[0].RenderLocation.Y,
            };

            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);
            float d20 = Vector2.Dot(v2, v0);
            float d21 = Vector2.Dot(v2, v1);

            float denom = d00 * d11 - d01 * d01;
            if (denom < 1)
                return new Vector3(1, 1, 1);

            var v = (d11 * d20 - d01 * d21) / denom;
            var w = (d00 * d21 - d01 * d20) / denom;
            var u = 1.0f - v - w;

            return new Vector3(w, u, v);
        }
    }
}
