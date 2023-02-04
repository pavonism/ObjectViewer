using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SketcherControl.Geometrics
{
    public static class GeometricExtensions
    {
        public static Vector3 ToVector3(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

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
            vector.Cut();
            return Color.FromArgb((int)(vector.X * 255), (int)(vector.Y * 255), (int)(vector.Z * 255));
        }

        public static Vector4 Cut(this ref Vector4 vector)
        {
            if (vector.X < 0 || float.IsNaN(vector.X)) vector.X = 0;
            if (vector.X > 1) vector.X = 1;
            if (vector.Y < 0 || float.IsNaN(vector.Y)) vector.Y = 0;
            if (vector.Y > 1) vector.Y = 1;
            if (vector.Z < 0 || float.IsNaN(vector.Z)) vector.Z = 0;
            if (vector.Z > 1) vector.Z = 1;

            vector.W = 0;
            return vector;
        }

        public static Vector3 Cut(this ref Vector3 vector)
        {
            if (vector.X < 0) vector.X = 0;
            if (vector.X > 1) vector.X = 1;
            if (vector.Y < 0) vector.Y = 0;
            if (vector.Y > 1) vector.Y = 1;
            if (vector.Z < 0) vector.Z = 0;
            if (vector.Z > 1) vector.Z = 1;
            return vector;
        }

        public static Color ToColor(this Vector3 vector)
        {
            vector.Cut();
            return Color.FromArgb((int)(vector.X * 255), (int)(vector.Y * 255), (int)(vector.Z * 255));
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
