
using SketcherControl.Filling;
using SketcherControl.Geometrics;
using System.Drawing;
using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Object3
    {
        private int objectIndx;
        private int RenderThreads = 50;
        private float scale;
        private float offsetX;
        private float offsetY;
        private readonly List<Triangle> triangles = new();
        private float rotationX;
        private float rotationY;

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

        public void SetRenderScale(int width, int height, Vector3 cameraPosition, float fov, float angleX, float angleY)
        {

            //if(this.objectIndx % 2 == 0)
            //    this.rotationX -= 2 * (float)Math.PI / height;
            //else
            //    this.rotationX += 2 * (float)Math.PI / width;

            foreach (var triangle in triangles)
            {
                triangle.SetRenderScale(width, height, cameraPosition, fov, this.rotationX, this.rotationY);
            }
        }
    }
}
