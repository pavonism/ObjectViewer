
using SketcherControl.Filling;
using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Object3
    {
        private int objectIndx;
        private int RenderThreads = 100;
        private readonly List<Triangle> triangles = new();
        private float rotationX;
        private float rotationY;

        private Matrix4x4 model;

        private Color color;

        public SizeF ObjectSize { get; private set; }

        public Object3(List<Triangle> triangles, SizeF objectSize, int objectIndx)
        {
            this.triangles = triangles;
            ObjectSize = objectSize;
            this.objectIndx = objectIndx;

            if (this.objectIndx % 2 == 0)
                this.color = SketcherConstants.ThemeColor;
            else
                this.color = Color.MediumPurple;
        }

        public void Render(DirectBitmap bitmap, Matrix4x4 position, Vector3 cameraPosition, bool showLines = true, ColorPicker? colorPicker = null)
        {
            Vector3 lookVector = -cameraPosition;

            if(colorPicker != null)
            {
                var colorPickerWithScale = new ColorPickerWithScale(colorPicker, position, bitmap.Width, bitmap.Height);
                colorPickerWithScale.TargetColor = this.color;

                if (RenderThreads == 1)
                {
                    foreach (var triangle in this.triangles)
                    {
                        ScanLine.Fill(triangle, bitmap, colorPickerWithScale, this.color);
                    }
                }
                else
                {
                    var trianglesPerThread = (int)Math.Ceiling((float)this.triangles.Count / RenderThreads);
                    List<Task> tasks = new();

                    for (int i = 0; i < RenderThreads; i++)
                    {
                        tasks.Add(FillAsync(bitmap, colorPickerWithScale, i * trianglesPerThread, trianglesPerThread, lookVector));
                    }

                    Task.WaitAll(tasks.ToArray());
                }
            }

            if (showLines)
            {
                foreach (var triangle in triangles)
                {
                    triangle.Render(bitmap, lookVector);
                }
            }
        }

        private Task FillAsync(DirectBitmap bitmap, ColorPicker colorPicker, int start, int step, Vector3 lookVector)
        {
            return Task.Run(
                () =>
                {
                    for (int j = start; j < start + step && j < this.triangles.Count; j++)
                    {
                        if (this.triangles[j].IsVisible(bitmap.Width, bitmap.Height, lookVector))
                            ScanLine.Fill(this.triangles[j], bitmap, colorPicker, this.color);
                    }
                });
        }

        public void SetRenderScale(int width, int height, Matrix4x4 view, Matrix4x4 position)
        {
            if (this.objectIndx % 2 == 0)
                this.rotationX += 2 * 2 * (float)Math.PI / height;
            else
                this.rotationY += 2 * 2 * (float)Math.PI / width;

            var rotationX = Matrix4x4.CreateRotationX(this.rotationX);
            var rotationY = Matrix4x4.CreateRotationY(this.rotationY);
            this.model = rotationX * rotationY;

            foreach (var triangle in triangles)
            {
                triangle.SetRenderScale(width, height, this.model, view, position);
            }
        }
    }
}
