using SketcherControl.SceneManipulation;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl
{
    internal class SceneRenderParameters
    {
        public Matrix4x4 View { get; set; }
        public Matrix4x4 Position { get; set; }
        public int ViewWidth { get; set; }
        public int ViewHeight { get; set; }
        public Vector3 LookVector { get; set; }
        public bool ShowLines { get; set; }
        public bool Fill { get; set; }
    }

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
            return bitmap;
        }

        public void PaintScene(DirectBitmap bitmap, Scene scene, SceneRenderParameters parameters)
        {
            foreach (var obj in scene.Objects)
            {
                obj.Render(bitmap, parameters.ShowLines, parameters.Fill ? scene.ColorPicker : null);
            }
        }
    }
}
