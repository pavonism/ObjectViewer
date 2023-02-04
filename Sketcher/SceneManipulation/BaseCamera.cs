using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.SceneManipulation
{
    public class BaseCamera : ICamera
    {
        protected Vector3 cameraVector = new Vector3(4f, 4f, 2f);
        protected float fov = (float)Math.PI * 2 / 3 * 0.8f;

        protected Matrix4x4 view = Matrix4x4.Identity;
        protected Matrix4x4 position = Matrix4x4.Identity;

        public virtual void Apply(SceneViewer viewer)
        {
            UpdateViewMatrix();
            UpdatePositionMatrix(viewer.Width, viewer.Height);
            viewer.Camera = this;
        }

        public virtual void UpdateViewMatrix()
        {
            this.view = Matrix4x4.CreateLookAt(this.cameraVector, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        }

        public virtual void UpdatePositionMatrix(int width, int height)
        {
            this.position = Matrix4x4.CreatePerspectiveFieldOfView(this.fov, (float)width / height, 1, 50);
        }

        public virtual void CameraScreenChanged(int newWidth, int newHeight)
        {
            UpdatePositionMatrix(newWidth, newHeight);
        }

        public virtual Vector3 GetCameraVector()
        {
            return cameraVector;
        }

        public virtual Matrix4x4 GetPositionMatrix()
        {
            return position;
        }

        public virtual Matrix4x4 GetViewMatrix()
        {
            return view;
        }

        public virtual Vector3 GetLookVector()
        {
            return -this.cameraVector;
        }
    }

    public class FollowingCamera : BaseCamera
    {
        protected Object3 movingObject;

        public override void Apply(SceneViewer viewer)
        {
            this.movingObject = viewer.Scene.MovingObject;
            base.Apply(viewer);
        }

        public override Matrix4x4 GetViewMatrix()
        {
            UpdateViewMatrix();
            return view;
        }

        public override void UpdateViewMatrix()
        {
            this.view = Matrix4x4.CreateLookAt(cameraVector, this.movingObject.Translation.Translation, new Vector3(0, 0, 1));
        }

        public override Vector3 GetLookVector()
        {
            return this.movingObject.Location;
        }
    }

    public class FirstPersonCamera : FollowingCamera
    {
        protected Vector3 lookVector;
        protected Vector3 cameraStartLook = new Vector3(0, 0, 1);

        public override void Apply(SceneViewer viewer)
        {
            base.Apply(viewer);
            this.fov = (float)Math.PI * 2 / 3;
            this.movingObject.ObjectMoved += MovingObject_ObjectMoved;
        }

        private void MovingObject_ObjectMoved(Matrix4x4 newPosition, Matrix4x4 newRotation)
        {
            UpdateViewMatrix();
        }

        public override Vector3 GetLookVector()
        {
            return this.lookVector - this.cameraVector;
        }

        public override void UpdateViewMatrix()
        {
            this.cameraVector = this.movingObject.Location;
            lookVector = Vector3.Transform(cameraStartLook, this.movingObject.Rotation * this.movingObject.Translation);
            this.view = Matrix4x4.CreateLookAt(this.cameraVector, lookVector, new Vector3(0, 0, 1));
        }
    }

    public class ThirdPersonCamera : FirstPersonCamera
    {
        public override void UpdateViewMatrix()
        {
            this.cameraVector = this.movingObject.Translation.Translation;
            lookVector = Vector3.Transform(cameraStartLook + this.cameraVector, this.movingObject.Rotation);
            var diff = this.lookVector - this.cameraVector;
            this.cameraVector -= Vector3.Normalize(diff) * 2;
            this.view = Matrix4x4.CreateLookAt(this.cameraVector, lookVector, new Vector3(0, 0, 1));
        }
    }

    public class FreeCamera : BaseCamera
    {
        private SceneViewer sceneViewer;

        public override void Apply(SceneViewer viewer)
        {
            this.sceneViewer = viewer;
            UpdateViewMatrix();
            UpdatePositionMatrix(viewer.Width, viewer.Height);
            viewer.Camera = this;
        }

        public override void UpdateViewMatrix()
        {
            this.view = Matrix4x4.CreateLookAt(this.sceneViewer.CameraVector, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        }

        public override void UpdatePositionMatrix(int width, int height)
        {
            this.position = Matrix4x4.CreatePerspectiveFieldOfView(this.sceneViewer.FOV, (float)width / height, 1, 50);
        }

        public override Matrix4x4 GetPositionMatrix()
        {
            UpdatePositionMatrix(sceneViewer.Width, sceneViewer.Height);
            return position;
        }

        public override Matrix4x4 GetViewMatrix()
        {
            UpdateViewMatrix();
            return view;
        }

        public override Vector3 GetLookVector()
        {
            return -sceneViewer.CameraVector;
        }

        public override Vector3 GetCameraVector()
        {
            return sceneViewer.CameraVector;
        }
    }
}
