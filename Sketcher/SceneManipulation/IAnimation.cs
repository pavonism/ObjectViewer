using System.Numerics;

namespace SketcherControl.SceneManipulation
{
    public interface IAnimation
    {
        bool IsStopped { get; set; }

        Matrix4x4 GetTranslation();
        Matrix4x4 GetRotation();
    }
}
