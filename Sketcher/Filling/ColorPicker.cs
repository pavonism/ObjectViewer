using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public class ColorPicker : IColorPicker
    {
        #region Fields and Events
        public event Action? ParametersChanged;

        private Interpolation interpolationMode;
        private Color targetColor = SketcherConstants.ThemeColor;
        private float kD = 0.5f;
        private float kS = 0.5f;
        private int m = 4;
        private LightSource lightSource;
        private Vector4 v = new(0, 0, 1, 0);
        private DirectBitmap? texture;
        private DirectBitmap? normalMap;
        #endregion

        #region Properties
        public virtual Interpolation InterpolationMode
        {
            get => this.interpolationMode;
            set
            {
                if (this.interpolationMode == value)
                    return;

                this.interpolationMode = value;
                ParametersChanged?.Invoke();
            }
        }

        public virtual float KD
        {
            get => this.kD;
            set
            {
                if (this.kD == value)
                    return;

                this.kD = value;
                ParametersChanged?.Invoke();
            }
        }

        public virtual float KS
        {
            get => this.kS;
            set
            {
                if (this.kS == value)
                    return;

                this.kS = value;
                ParametersChanged?.Invoke();
            }
        }

        public virtual int M
        {
            get => this.m;
            set
            {
                if (this.m == value)
                    return;

                this.m = value;
                ParametersChanged?.Invoke();
            }
        }

        public virtual Color TargetColor
        {
            get => this.targetColor;
            set
            {
                if (this.targetColor == value)
                    return;

                texture?.Dispose();
                texture = null;
                this.targetColor = value;
                ParametersChanged?.Invoke();
            }
        }

        public virtual DirectBitmap? Pattern
        {
            get => this.texture;
            set
            {
                if(this.texture == value) 
                    return;
                
                this.texture = value;
                ParametersChanged?.Invoke();
            }
        }

        public virtual DirectBitmap? NormalMap
        {
            get => this.normalMap;
            set
            {
                if (this.normalMap == value)
                    return;

                this.normalMap = value;
                ParametersChanged?.Invoke();
            }
        }

        public virtual LightSource LightSource
        {
            get => this.lightSource;
            set
            {
                if (this.lightSource != value)
                    this.lightSource = value;

                ParametersChanged?.Invoke();
            }
        }
        #endregion

        #region Initialization
        public ColorPicker()
        {
        }

        public ColorPicker(LightSource lightSource)
        {
            this.lightSource = lightSource;
        }
        #endregion

        #region Logic
        public virtual Vector4 Scale(Vector4 vector)
        {
            return vector;
        }

        public virtual Vector4 ScaleBack(Vector4 vector)
        {
            return vector;
        }

        public void StartFillingTriangle(IEnumerable<Vertex> vertices)
        {

            switch (InterpolationMode)
            {
                case Interpolation.Color:
                    foreach (var vertex in vertices)
                    {
                        var textureColor = Pattern?.GetPixel(((int)vertex.RenderLocation.X + Pattern.Width / 2) % Pattern.Width, ((int)vertex.RenderLocation.Y + Pattern.Height / 2) % Pattern.Height).ToVector();
                        var normalVector = NormalMap != null ? GetNormalVectorFromNormalMap((int)vertex.RenderLocation.X, (int)vertex.RenderLocation.Y, vertex.NormalVector) : vertex.NormalVector;
                        vertex.Color = CalculateColorInPoint(vertex.Location, normalVector, textureColor);
                    }
                    break;
                case Interpolation.NormalVector:
                    break;
            }
        }

        public Color GetColor(Polygon polygon, int x, int y)
        {
            if (polygon.VertexCount < 3)
                return Color.Empty;

            switch (InterpolationMode)
            {
                case Interpolation.Color:
                    return GetColorWithColorInterpolation(polygon, x, y);
                case Interpolation.NormalVector:
                    return GetColorWithVectorInterpolation(polygon, x, y);
            }

            return Color.Empty;
        }

        private Color GetColorWithColorInterpolation(Polygon polygon, int x, int y)
        {
            var coefficients = GetBarycentricCoefficients(polygon, x, y);
            var rc = polygon.Vertices[0].Color.R * coefficients[1] / 255 + polygon.Vertices[1].Color.R * coefficients[2] / 255 + polygon.Vertices[2].Color.R * coefficients[0] / 255;
            var gc = polygon.Vertices[0].Color.G * coefficients[1] / 255 + polygon.Vertices[1].Color.G * coefficients[2] / 255 + polygon.Vertices[2].Color.G * coefficients[0] / 255;
            var bc = polygon.Vertices[0].Color.B * coefficients[1] / 255 + polygon.Vertices[1].Color.B * coefficients[2] / 255 + polygon.Vertices[2].Color.B * coefficients[0] / 255;

            if (rc < 0) rc = 0;
            if (rc > 1) rc = 1;
            if (gc < 0) gc = 0;
            if (gc > 1) gc = 1;
            if (bc < 0) bc = 0;
            if (bc > 1) bc = 1;

            return Color.FromArgb((int)(rc * 255), (int)(gc * 255), (int)(bc * 255));
        }

        private Color GetColorWithVectorInterpolation(Polygon polygon, int x, int y)
        {
            var coefficients = GetBarycentricCoefficients(polygon, x, y);
            var normalVector = GetNormalVector(polygon, x, y, coefficients);
            var textureColor = GetTextureColor(x, y);
            var z = InterpolateZ(polygon, coefficients);
            var renderLocation = new Vector4(x, y, z, 0);
            return CalculateColorInPoint(ScaleBack(renderLocation), normalVector, textureColor);
        }

        private float[] GetBarycentricCoefficients(Polygon polygon, int x, int y)
        {
            float[]? coefficients;

            if (!polygon.CoefficientsCache.TryGetValue((x, y), out coefficients))
            {
                coefficients = CalculateCoefficients(polygon, x, y);
            }

            return coefficients;
        }

        private Vector4 GetNormalVector(Polygon polygon, int x, int y, float[] coefficients)
        {
            var normalVector = GetNormalVectorFromInterpolation(polygon, x, y, coefficients);

            if (NormalMap != null)
                normalVector = GetNormalVectorFromNormalMap(x, y, normalVector);

            return normalVector;
        }

        private Vector4? GetTextureColor(int x, int y)
        {
            return Pattern?.GetPixel(x % Pattern.Width, y % Pattern.Height).ToVector();
        }

        private float InterpolateZ(Polygon polygon, float[] coefficients)
        {
            return polygon.Vertices[0].Location.Z * coefficients[1] + polygon.Vertices[1].Location.Z * coefficients[2] + polygon.Vertices[2].Location.Z * coefficients[0];
        }

        private Color CalculateColorInPoint(Vector4 location, Vector4 normalVector, Vector4? textureColor = null)
        {
            Vector4 IL = LightSource.LightSourceVector;
            Vector4 IO = textureColor ?? TargetColor.ToVector();
            Vector4 L = Vector4.Normalize(ScaleBack(LightSource.RenderLocation) - location);
            Vector4 R = 2 * Vector4.Dot(normalVector, L) * normalVector - L;

            var angleNL = Vector4.Dot(normalVector, L);
            if (angleNL < 0) angleNL = 0;

            var angleVR = Vector4.Dot(v, R);
            if (angleVR < 0) angleVR = 0;

            return ((KD * IL * IO * angleNL) + (KS * IL * IO * (float)Math.Pow(angleVR, m))).ToColor();
        }

        private Vector4 GetNormalVectorFromNormalMap(int x, int y, Vector4 NSurface)
        {
            var textureColor = NormalMap!.GetPixel(x % NormalMap.Width, y % NormalMap.Height);
            var NTexture = textureColor.ToNormalMapVector();
            var B = Vector4.Normalize(NSurface.Cross(new Vector4(0, 0, 1, 0)));
            var T = Vector4.Normalize(B.Cross(NSurface));

            return new Vector4()
            {
                X = T.X * NTexture.X + B.X * NTexture.Y + NSurface.X * NTexture.Z,
                Y = T.Y * NTexture.X + B.Y * NTexture.Y + NSurface.Y * NTexture.Z,
                Z = T.Z * NTexture.X + B.Z * NTexture.Y + NSurface.Z * NTexture.Z,
            };
        }

        private Vector4 GetNormalVectorFromInterpolation(Polygon polygon, int x, int y, float[] coefficients)
        {
            Vector4 normalVector;

            if (!polygon.NormalVectorsCache.TryGetValue((x, y), out normalVector))
            {
                normalVector = InterpolateNormalVector(polygon, coefficients);
                polygon.NormalVectorsCache.Add((x, y), normalVector);
            }

            return normalVector;
        }

        private Vector4 InterpolateNormalVector(Polygon polygon, float[] coefficients)
        {
            var xn = polygon.Vertices[0].NormalVector.X * coefficients[1] + polygon.Vertices[1].NormalVector.X * coefficients[2] + polygon.Vertices[2].NormalVector.X * coefficients[0];
            var yn = polygon.Vertices[0].NormalVector.Y * coefficients[1] + polygon.Vertices[1].NormalVector.Y * coefficients[2] + polygon.Vertices[2].NormalVector.Y * coefficients[0];
            var zn = polygon.Vertices[0].NormalVector.Z * coefficients[1] + polygon.Vertices[1].NormalVector.Z * coefficients[2] + polygon.Vertices[2].NormalVector.Z * coefficients[0];

            return Vector4.Normalize(new Vector4(xn, yn, zn, 0));
        }

        private float[] CalculateCoefficients(Polygon polygon, int x, int y)
        {
            var pixelLocation = new Vector4(x, y, 0, 0);
            var coefficients = new float[polygon.VertexCount];
            var areas = new float[polygon.VertexCount];
            float sum = 0;

            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                var xBA = polygon.Vertices[i].RenderLocation.X - polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation.X;
                var yCA = pixelLocation.Y - polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation.Y;
                var yBA = polygon.Vertices[i].RenderLocation.Y - polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation.Y;
                var xCA = pixelLocation.X - polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation.X;
                var area = 0.5f * Math.Abs(xBA * yCA - yBA * xCA);

                sum += area;
                areas[i] = area;
            }

            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                coefficients[i] = areas[i] / sum;
            }

            polygon.CoefficientsCache.Add((x, y), coefficients);
            return coefficients;
        }
        #endregion Logic
    }
}
