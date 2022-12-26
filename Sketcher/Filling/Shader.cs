using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public abstract class Shader : IShader
    {
        #region Fields and Events
        public event Action? ParametersChanged;
        private Vector4 v = new(0, 0, 1, 0);

        private List<LightSource> lights = new();
        public ShaderParameters Parameters { get; set; } = new();
        protected Object3 target;
        #endregion

        #region Initialization
        public void Initialize(Object3 obj, IEnumerable<LightSource> lightSources)
        {
            this.target = obj;
            this.lights.Clear();
            this.lights.AddRange(lightSources);
        }
        #endregion

        #region Logic
        public abstract void StartFillingTriangle(Polygon polygon);

        public abstract Color GetColor(Polygon polygon, int x, int y);

        protected Vector4 GetNormalVector(Polygon polygon, int x, int y, Vector3 coefficients)
        {
            var normalVector = GetNormalVectorFromInterpolation(polygon, x, y, coefficients);

            if (this.target.NormalMap != null)
            {
                normalVector = GetNormalVectorFromNormalMap(x, y, normalVector);
            }

            return normalVector;
        }

        protected Vector4? GetTextureColor(int x, int y)
        {
            return this.target.Texture?.GetPixel(x % this.target.Texture.Width, y % this.target.Texture.Height).ToVector();
        }

        protected Vector4 CalculateColorInPoint(Vector4 location, Vector4 normalVector, Vector4? textureColor = null)
        {
            Vector4 result = new();

            foreach (var light in lights)
            {
                Vector4 IL = light.ColorVector;
                Vector4 IO = textureColor ?? target.Color.ToVector();
                Vector4 L = Vector4.Normalize(light.SceneLocation - location);
                Vector4 R = 2 * Vector4.Dot(normalVector, L) * normalVector - L;

                var angleNL = Vector4.Dot(normalVector, L);
                if (angleNL < 0) angleNL = 0;

                var angleVR = Vector4.Dot(v, R);
                if (angleVR < 0) angleVR = 0;
                result += (Parameters.KD * IL * IO * angleNL) + (Parameters.KS * IL * IO * (float)Math.Pow(angleVR, Parameters.M));
            }
            return result.Cut();
        }

        protected Vector4 GetNormalVectorFromNormalMap(int x, int y, Vector4 NSurface)
        {
            var textureColor = target.NormalMap!.GetPixel(x % target.NormalMap.Width, y % target.NormalMap.Height);
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

        protected Vector4 GetNormalVectorFromInterpolation(Polygon polygon, int x, int y, Vector3 coefficients)
        {
            Vector4 normalVector;

            if (!polygon.NormalVectorsCache.TryGetValue((x, y), out normalVector))
            {
                normalVector = InterpolateNormalVector(polygon, coefficients);
                polygon.NormalVectorsCache.Add((x, y), normalVector);
            }

            return normalVector;
        }

        protected Vector4 InterpolateNormalVector(Polygon polygon, Vector3 coefficients)
        {
            var xn = polygon.Vertices[0].GlobalNormalVector.X * coefficients.Y + polygon.Vertices[1].GlobalNormalVector.X * coefficients.Z + polygon.Vertices[2].GlobalNormalVector.X * coefficients.X;
            var yn = polygon.Vertices[0].GlobalNormalVector.Y * coefficients.Y + polygon.Vertices[1].GlobalNormalVector.Y * coefficients.Z + polygon.Vertices[2].GlobalNormalVector.Y * coefficients.X;
            var zn = polygon.Vertices[0].GlobalNormalVector.Z * coefficients.Y + polygon.Vertices[1].GlobalNormalVector.Z * coefficients.Z + polygon.Vertices[2].GlobalNormalVector.Z * coefficients.X;

            return Vector4.Normalize(new Vector4(xn, yn, zn, 0));
        }
        #endregion Logic
    }
}
