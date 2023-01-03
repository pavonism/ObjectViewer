using System.Numerics;

namespace SketcherControl.Filling
{
    public class Reflector : Light
    {
        private Vector4 target;
        public Vector4 Target
        {
            get => this.target;
            set
            {
                this.target = Vector4.Normalize(value - SceneLocation);
            }
        }

        public override Vector4 AdjustColorFromShader(Vector4 shaderColor, Vector4 pointLocation)
        {
            var lightLine = Vector4.Normalize(pointLocation - SceneLocation);
            var cos = Vector4.Dot(target, lightLine);

            return cos * shaderColor;
        }
    }
}
