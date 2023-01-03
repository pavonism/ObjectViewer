using SketcherControl.Filling;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.SceneManipulation
{
    internal class SceneRenderer
    {
        public event Action<DirectBitmap>? RenderFinished;
        private bool isBusy;

        public void Render(Scene scene, SceneRenderParameters parameters)
        {
            if (!isBusy)
            {
                isBusy = true;
                RunRenderAsync(scene, parameters);
            }
        }

        public async void RunRenderAsync(Scene scene, SceneRenderParameters parameters)
        {
            var renderedScene = await RenderAsync(scene, parameters);
            RenderFinished?.Invoke(renderedScene);
            isBusy = false;
        }

        private Task<DirectBitmap> RenderAsync(Scene scene, SceneRenderParameters parameters)
        {
            return Task.Run(() =>
            {
                var directBitmap = PrepareBitmap(parameters.ViewWidth, parameters.ViewHeight);
                PrepareScene(scene, parameters);
                PaintScene(directBitmap, scene, parameters);
                return directBitmap;
            });
        }

        private void PrepareScene(Scene scene, SceneRenderParameters parameters)
        {
            List<Task> tasks = new List<Task>();

            foreach (var obj in scene.Objects)
            {
                tasks.Add(PrepareObjectAsync(obj, parameters));
            }

            foreach (var light in scene.Lights)
            {
                if (light.Shape != null)
                    tasks.Add(PrepareObjectAsync(light.Shape, parameters));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private Task PrepareObjectAsync(Object3 obj, SceneRenderParameters parameters)
        {
            return Task.Run(() =>
            {
                obj.Transform(parameters.ViewWidth, parameters.ViewHeight, parameters.View, parameters.Position);
                obj.UpdateTrianglesVisibility(parameters.LookVector);
                obj.UpdateBarycentricCache();
            });
        }

        private DirectBitmap PrepareBitmap(int width, int height)
        {
            var bitmap = new DirectBitmap(width, height);

            using(var g = Graphics.FromImage(bitmap.Bitmap))
            {
                g.Clear(Color.Black);
            }

            return bitmap;
        }

        public void PaintScene(DirectBitmap bitmap, Scene scene, SceneRenderParameters parameters)
        {
            IPixelProcessor? processor = null;
            Shader shader = scene.Shader;

            if(parameters.Fill)
            {
                if (parameters.Fog)
                    processor = new PixelPainterWithFog(bitmap, scene.Shader, parameters.CameraVector);
                else
                    processor = new PixelPainter(bitmap, scene.Shader);
            }

            foreach (var obj in scene.Objects)
            {
                scene.Shader?.Initialize(obj, scene.Lights);
                obj.Render(bitmap, parameters.CameraVector, parameters.ShowLines, processor);
            }

            shader = new SolidShader();
            if (processor != null)
            {
                if (parameters.Fog)
                    processor = new PixelPainterWithFog(bitmap, shader, parameters.CameraVector);
                else
                    processor = new PixelPainter(bitmap, shader);
            }

            foreach (var light in scene.Lights)
            {
                if(light.Shape != null)
                {
                    shader.Initialize(light.Shape, Enumerable.Empty<Light>());
                    light.Shape.Render(bitmap, parameters.CameraVector, parameters.ShowLines, processor);
                }
            }
        }
    }
}
