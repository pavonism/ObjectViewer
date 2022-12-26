using System.Numerics;

namespace SketcherControl.SceneManipulation
{
    public interface IAnimation
    {
        Matrix4x4 GetTranslation();
        Matrix4x4 GetRotation();
    }
}
