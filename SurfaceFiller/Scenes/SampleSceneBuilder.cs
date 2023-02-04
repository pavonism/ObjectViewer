using SketcherControl.Filling;
using SketcherControl.SceneManipulation;
using SurfaceFiller;
using System.Numerics;

namespace ObjectViewer.Scenes
{
    internal class SampleAnimation : IAnimation
    {
        private float speed = 0.02f;
        private float move;
        public bool IsStopped { get; set; }

        public Matrix4x4 GetRotation()
        {
            return Matrix4x4.Identity;
        }

        public Matrix4x4 GetTranslation()
        {
            if(!IsStopped)
                move += speed;

            if (move >= 0.05 && speed > 0 || move <= -0.05 && speed < 0)
                speed = -speed;

            return Matrix4x4.CreateTranslation(0, 0, speed);
        }
    }

    internal class SampleSceneBuilder : SceneBuilder
    {
        private const int fixedObjectsCount = 6;
        private const float radius = 3;

        public override Scene Build()
        {
            Scene scene = new();
            ObjectLoader loader = new();

            var sphere = loader.LoadObjectFromFile(Resources.SphereFile);
            var reflector = new Reflector();
            reflector.ColorVector = new Vector4(1, 1, 1, 0);
            sphere.Rotate(Matrix4x4.CreateRotationX((float)Math.PI));
            sphere.SetScale(0.1f);
            reflector.Shape = sphere;
            scene.AddLightSource(reflector);

            var lightSource = new Light();
            lightSource.SceneLocation = new Vector4(-3, 2, 3, 0);
            lightSource.Color = Color.Red;
            sphere.Rotate(Matrix4x4.CreateRotationX((float)Math.PI));
            lightSource.Shape = loader.MakeCopy(sphere);
            lightSource.Shape.SetScale(0.1f);
            scene.AddLightSource(lightSource);

            var torusData = loader.LoadObjectFromFile(Resources.TorusFile);

            for (int i = 0; i < fixedObjectsCount; i++)
            {
                float X = radius * (float)Math.Cos(i * 2 * Math.PI / fixedObjectsCount);
                float Y = radius * (float)Math.Sin(i * 2 * Math.PI / fixedObjectsCount);
                var torus = loader.MakeCopy(torusData);
                torus.MoveTo(X, Y, 0);
                scene.AddObject(torus);
            }

            var animatedTorus = torusData;
            animatedTorus.RotateX((float)Math.PI / 2);
            reflector.SetParent(animatedTorus, new Vector3(0,0,0), new Vector3(0, 0, 1));
            animatedTorus.Animation = new SampleAnimation() { IsStopped = true };
            scene.MovingObject = animatedTorus;
            scene.Reflector = reflector;
            scene.AddObject(animatedTorus);
            
            return scene;
        }
    }
}
