
using SketcherControl.Filling;
using SketcherControl.Geometrics;
using System.Drawing;

namespace SketcherControl.Shapes
{
    public class Object3
    {
        private int RenderThreads = 50;
        private float scale;
        private float offsetX;
        private float offsetY;
        private readonly List<Triangle> triangles = new();

        public SizeF ObjectSize { get; private set; }

        public Object3(List<Triangle> triangles, SizeF objectSize)
        {
            this.triangles = triangles;
            ObjectSize = objectSize;
        }

        public void Render(DirectBitmap bitmap, bool showLines = true, ColorPicker? colorPicker = null)
        {

            if(colorPicker != null)
            {
                var colorPickerWithScale = new ColorPickerWithScale(colorPicker, scale, offsetX, offsetY);

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
                        ScanLine.Fill(this.triangles[j], bitmap, colorPicker);

                    }
                });
        }

        public void SetRenderScale(float scale, float offsetX, float offsetY, float angleX, float angleY)
        {
            this.scale = scale;
            this.offsetX = offsetX;
            this.offsetY = offsetY;

            foreach (var triangle in triangles)
            {
                triangle.SetRenderScale(scale, offsetX, offsetY, angleX, angleY);
            }
        }
    }
}
