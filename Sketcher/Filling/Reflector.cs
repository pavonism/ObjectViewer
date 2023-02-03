using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public class Reflector : Light
    {
        private Vector3 target;
        private Vector3 offset;
        private Object3? parent;
        private float targetRotation;
        private Matrix4x4 currentTargetRotation;

        public override Vector4 SceneLocation { get; set; }
        public Vector4 CurrentTarget { get; set; }


        public override Vector4 AdjustColorFromShader(Vector4 shaderColor, Vector4 pointLocation)
        {
            var lightLine = Vector4.Normalize(pointLocation - SceneLocation);
            var cos = Vector4.Dot(CurrentTarget, lightLine);

            return cos * shaderColor;
        }

        public void SetParent(Object3 parent, Vector3 offset, Vector3 target)
        {
            this.parent = parent;
            this.offset = offset;
            this.target = target;
            UpdateTarget(true);
            this.parent.ObjectMoved += Parent_ObjectMoved;
        }

        private void Parent_ObjectMoved(Matrix4x4 newPosition, Matrix4x4 newRotation)
        {
            UpdateTarget();
            SceneLocation = new Vector4(newPosition.Translation + offset, 0);
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

        public void TurnRight()
        {
            targetRotation += (float)Math.PI / 100000;
            UpdateTarget(true);
        }

        public void TurnLeft()
        {
            targetRotation -= (float)Math.PI / 100000;
            UpdateTarget(true);
        }

        private void UpdateTarget(bool recalculateMatrix = false)
        {
            if (recalculateMatrix)
            {
                this.currentTargetRotation = Matrix4x4.CreateRotationX(this.targetRotation);
            }

            CurrentTarget = Vector4.Transform(this.target, this.currentTargetRotation);

            if (this.parent != null)
            {
                CurrentTarget = Vector4.Transform(CurrentTarget, parent.Rotation * parent.Translation);
            }
        }
    }
}
