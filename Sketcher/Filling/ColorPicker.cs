using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Drawing;
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
        public float InterpolateZ(Polygon polygon, int x, int y)
        {
            var coefficients = GetBarycentricCoefficients(polygon, x, y);
            return InterpolateZ(polygon, coefficients);
        }

        public void StartFillingTriangle(Polygon polygon)
        {
            switch (InterpolationMode)
            {
                case Interpolation.Color:
                    foreach (var vertex in polygon.Vertices)
                    {
                        var textureColor = Pattern?.GetPixel(((int)vertex.RenderLocation.X + Pattern.Width / 2) % Pattern.Width, ((int)vertex.RenderLocation.Y + Pattern.Height / 2) % Pattern.Height).ToVector();
                        var normalVector = NormalMap != null ? GetNormalVectorFromNormalMap((int)vertex.RenderLocation.X, (int)vertex.RenderLocation.Y, vertex.NormalVector) : vertex.GlobalNormalVector;
                        vertex.Color = CalculateColorInPoint(vertex.GlobalLocation, normalVector, textureColor);
                        polygon.UpdateColorMatrix();
                    }
                    break;
                case Interpolation.NormalVector:
                    break;
            }
        }

        public Color GetColor(Polygon polygon, int x, int y)
        {
            if(InterpolationMode == Interpolation.Color)
            {
                return GetColorWithColorInterpolation(polygon, x, y);
            }
            else
            {
                return GetColorWithVectorInterpolation(polygon, x, y);
            }
        }

        private Color GetColorWithColorInterpolation(Polygon polygon, int x, int y)
        {
            var coefficients = GetBarycentricCoefficients(polygon, x, y);
            var color = Vector3.Transform(coefficients, polygon.colors);
            return color.ToColor();
        }

        private Color GetColorWithVectorInterpolation(Polygon polygon, int x, int y)
        {
            var coefficients = GetBarycentricCoefficients(polygon, x, y);
            var normalVector = GetNormalVector(polygon, x, y, coefficients);
            var textureColor = GetTextureColor(x, y);
            var xi = InterpolateX(polygon, coefficients);
            var yi = InterpolateY(polygon, coefficients);
            var zi = InterpolateZ(polygon, coefficients);
            var renderLocation = new Vector4(xi, yi, zi, 0);
            return CalculateColorInPoint(renderLocation, normalVector, textureColor).ToColor();
        }

        private Vector3 GetBarycentricCoefficients(Polygon polygon, int x, int y)
        {
            Vector3 coefficients;

            if (!polygon.CoefficientsCache.TryGetValue((x, y), out coefficients))
            {
                coefficients = Interpolator.CalculateCoefficients(polygon, x, y);
            }

            return coefficients;
        }

        private Vector4 GetNormalVector(Polygon polygon, int x, int y, Vector3 coefficients)
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

        private float InterpolateZ(Polygon polygon, Vector3 coefficients)
        {
            return polygon.Vertices[0].RenderLocation.Z * coefficients.Y + polygon.Vertices[1].RenderLocation.Z * coefficients.Z + polygon.Vertices[2].RenderLocation.Z * coefficients.X;
        }

        private float InterpolateX(Polygon polygon, Vector3 coefficients)
        {
            return polygon.Vertices[0].GlobalLocation.X * coefficients.Y + polygon.Vertices[1].GlobalLocation.X * coefficients.Z + polygon.Vertices[2].GlobalLocation.X * coefficients.X;
        }

        private float InterpolateY(Polygon polygon, Vector3 coefficients)
        {
            return polygon.Vertices[0].GlobalLocation.Y * coefficients.Y + polygon.Vertices[1].GlobalLocation.Y * coefficients.Z + polygon.Vertices[2].GlobalLocation.Y * coefficients.X;
        }

        private Vector4 CalculateColorInPoint(Vector4 location, Vector4 normalVector, Vector4? textureColor = null)
        {
            Vector4 IL = LightSource.LightSourceVector;
            Vector4 IO = textureColor ?? TargetColor.ToVector();
            Vector4 L = Vector4.Normalize(LightSource.SceneLocation - location);
            Vector4 R = 2 * Vector4.Dot(normalVector, L) * normalVector - L;

            var angleNL = Vector4.Dot(normalVector, L);
            if (angleNL < 0) angleNL = 0;

            var angleVR = Vector4.Dot(v, R);
            if (angleVR < 0) angleVR = 0;
            var result = (KD * IL * IO * angleNL) + (KS * IL * IO * (float)Math.Pow(angleVR, m));
            return result.Cut();
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

        private Vector4 GetNormalVectorFromInterpolation(Polygon polygon, int x, int y, Vector3 coefficients)
        {
            Vector4 normalVector;

            if (!polygon.NormalVectorsCache.TryGetValue((x, y), out normalVector))
            {
                normalVector = InterpolateNormalVector(polygon, coefficients);
                polygon.NormalVectorsCache.Add((x, y), normalVector);
            }

            return normalVector;
        }

        private Vector4 InterpolateNormalVector(Polygon polygon, Vector3 coefficients)
        {
            var xn = polygon.Vertices[0].GlobalNormalVector.X * coefficients.Y + polygon.Vertices[1].GlobalNormalVector.X * coefficients.Z + polygon.Vertices[2].GlobalNormalVector.X * coefficients.X;
            var yn = polygon.Vertices[0].GlobalNormalVector.Y * coefficients.Y + polygon.Vertices[1].GlobalNormalVector.Y * coefficients.Z + polygon.Vertices[2].GlobalNormalVector.Y * coefficients.X;
            var zn = polygon.Vertices[0].GlobalNormalVector.Z * coefficients.Y + polygon.Vertices[1].GlobalNormalVector.Z * coefficients.Z + polygon.Vertices[2].GlobalNormalVector.Z * coefficients.X;

            return Vector4.Normalize(new Vector4(xn, yn, zn, 0));
        }
        #endregion Logic
    }
}
