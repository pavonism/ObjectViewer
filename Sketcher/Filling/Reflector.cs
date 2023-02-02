using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public class Reflector : Light
    {
        private Vector4 target;
        private Vector3 offset;
        private Object3? parent;

        public override Vector4 SceneLocation
        {
            get
            {
                return this.parent != null ? new Vector4(this.parent.Location + this.offset, 0) : base.SceneLocation;
            }
            set
            {
            }
        }

        public Vector4 Target
        {
            get => this.parent != null ? Vector4.Transform(new Vector3(0,0,1), this.parent.Rotation) : this.target;
            set
            {
                this.target = Vector4.Normalize(value - SceneLocation);
            }
        }

        public override Vector4 AdjustColorFromShader(Vector4 shaderColor, Vector4 pointLocation)
        {
            var lightLine = Vector4.Normalize(pointLocation - SceneLocation);
            var cos = Vector4.Dot(Target, lightLine);

            return cos * shaderColor;
        }

        public void SetParent(Object3 parent, Vector3 offset)
        {
            this.parent = parent;
            this.offset = offset;
        }

        public override void RenderShape(DirectBitmap bitmap, Vector3 cameraVector, bool showLines, IPixelProcessor? pixelProcessor)
        {
            if (this.parent != null && Shape != null)
            {
                this.Shape.Translation = this.parent.Translation;
                this.Shape.Rotation = this.parent.Rotation;
                Shape.UpdateModel();
            }
            base.RenderShape(bitmap, cameraVector, showLines, pixelProcessor);
        }
    }
}
