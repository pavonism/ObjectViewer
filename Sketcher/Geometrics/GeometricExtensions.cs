using System.Numerics;

namespace SketcherControl.Geometrics
{
    public static class GeometricExtensions
    {
        public static Vector4 Cross(this Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X, 0);
        }

        public static Vector4 ToVector(this Color color)
        {
            return new Vector4((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, 0);
        }

        public static Vector4 ToNormalMapVector(this Color color)
        {
            var normalMapVector = color.ToVector();
            normalMapVector.X = normalMapVector.X * 2 - 1;
            normalMapVector.Y = normalMapVector.Y * 2 - 1;
            return normalMapVector;
        }

        public static Color ToColor(this Vector4 vector)
        {
            var r = float.IsNaN(vector.X) || float.IsInfinity(vector.X) ? 255 : (int)Math.Min(Math.Max(0, vector.X) * 255, 255);
            var g = float.IsNaN(vector.Y) || float.IsInfinity(vector.Y) ? 255 : (int)Math.Min(Math.Max(0, vector.Y) * 255, 255);
            var b = float.IsNaN(vector.Z) || float.IsInfinity(vector.Z) ? 255 : (int)Math.Min(Math.Max(0, vector.Z) * 255, 255);

            return Color.FromArgb(r, g, b);
        }

        public static int CenterX(this Rectangle rect)
        {
            return rect.Location.X + rect.Width / 2;
        }

        public static int CenterY(this Rectangle rect)
        {
            return rect.Location.Y + rect.Height / 2;
        }
    }
}
