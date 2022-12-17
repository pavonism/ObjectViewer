
using SketcherControl.Filling;
using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Object3
    {
        private int objectIndx;
        private int RenderThreads = 50;
        private readonly List<Triangle> triangles = new();
        private float rotationX;
        private float rotationY;

        private Matrix4x4 model;
        private Matrix4x4 view;
        private Matrix4x4 position;

        public SizeF ObjectSize { get; private set; }

        public Object3(List<Triangle> triangles, SizeF objectSize, int objectIndx)
        {
            this.triangles = triangles;
            ObjectSize = objectSize;
            this.objectIndx = objectIndx;
        }

        public void Render(DirectBitmap bitmap, bool showLines = true, ColorPicker? colorPicker = null)
        {

            if(colorPicker != null)
            {
                var colorPickerWithScale = new ColorPickerWithScale(colorPicker, model * view * position, bitmap.Width, bitmap.Height);

                if (RenderThreads == 1)
                {
                    foreach (var triangle in this.triangles)
                    {
                        ScanLine.Fill(triangle, bitmap, colorPickerWithScale);
                    }
                }
                else
                {
                    var trianglesPerThread = (int)Math.Ceiling((float)this.triangles.Count / RenderThreads);
                    List<Task> tasks = new();

                    for (int i = 0; i < RenderThreads; i++)
                    {
                        tasks.Add(FillAsync(bitmap, colorPickerWithScale, i * trianglesPerThread, trianglesPerThread));
                    }

                    Task.WaitAll(tasks.ToArray());
                }
            }

            if (showLines)
            {
                foreach (var triangle in triangles)
                {
                    triangle.Render(bitmap);
                }
            }
        }

        private Task FillAsync(DirectBitmap bitmap, ColorPicker colorPicker, int start, int step)
        {
            return Task.Run(
                () =>
                {
                    for (int j = start; j < start + step && j < this.triangles.Count; j++)
                    {
                        if (this.triangles[j].IsVisible(bitmap.Width, bitmap.Height))
                            ScanLine.Fill(this.triangles[j], bitmap, colorPicker);
                    }
                });
        }

        public void SetRenderScale(int width, int height, Vector3 cameraPosition, float fov, float angleX, float angleY)
        {
            if (this.objectIndx % 2 == 0)
                this.rotationX -= 2 * (float)Math.PI / height;
            else
                this.rotationX += 2 * (float)Math.PI / width;

            var rotationX = Matrix4x4.CreateRotationX(this.rotationX);
            var rotationY = Matrix4x4.CreateRotationY(this.rotationY);
            this.model = rotationX * rotationY;
            this.view = Matrix4x4.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            this.position = Matrix4x4.CreatePerspectiveFieldOfView(fov, (float)width / height, 1, 50);

            foreach (var triangle in triangles)
            {
                triangle.SetRenderScale(width, height, this.model, this.view, this.position);
            }
        }
    }
}
