﻿using SketcherControl.Filling;
using SketcherControl.SceneManipulation;
using SurfaceFiller;
using System.Numerics;

namespace ObjectViewer.Scenes
{
    internal class SampleAnimation : IAnimation
    {
        private float speed = 0.01f;
        private float rotation = 0.03f;
        private float move;

        public Matrix4x4 GetRotation()
        {
            return Matrix4x4.CreateRotationY(rotation) * Matrix4x4.CreateRotationZ(rotation);
        }

        public Matrix4x4 GetTranslation()
        {
            move += speed;

            if (move >= 3 && speed > 0 || move <= -3 && speed < 0)
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
            var lightSource = new LightSource();
            sphere.Rotate(Matrix4x4.CreateRotationX((float)Math.PI));
            sphere.SetScale(0.1f);
            lightSource.Shape = sphere;
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
            animatedTorus.Animation = new SampleAnimation();
            scene.MovingObject = animatedTorus;
            scene.AddObject(animatedTorus);
            
            return scene;
        }
    }
}