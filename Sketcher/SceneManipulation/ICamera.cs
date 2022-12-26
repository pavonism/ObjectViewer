using System.Numerics;

namespace SketcherControl.SceneManipulation
{
    public interface ICamera
    {
        void Apply(SceneViewer viewer);

        Matrix4x4 GetViewMatrix();
        Matrix4x4 GetPositionMatrix();
        Vector3 GetCameraVector();
        Vector3 GetLookVector();
        void CameraScreenChanged(int newWidth, int newHeight);
    }
}
