
using SketcherControl.Filling;
using SketcherControl.Geometrics;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;

namespace SketcherControl.Shapes
{
    public class Object3
    {
        private int objectIndx;
        private int RenderThreads = 20;
        private readonly List<Triangle> triangles = new();
        private float rotationX;
        private float rotationY;
        private float circleRotation;
        private Vector3 radius = new Vector3(0, 1.5f, 0);

        private Matrix4x4 model;
        private Color color;

        public SizeF ObjectSize { get; private set; }
        public DirectBitmap Layer { get; set; } = new DirectBitmap(1, 1);

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
        
        public void UpdateTrianglesVisibility(int viewWidth, int viewHeight, Vector3 lookVector)
        {
            foreach (var triangle in this.triangles)
            {
                triangle.UpdateVisibility(viewWidth, viewHeight, lookVector);
            }
        }

        public void Render(DirectBitmap bitmap, bool showLines = true, ColorPicker? colorPicker = null)
        {
            if (colorPicker != null)
            {
                var colorPickerWithTargetColor = new TargetColorColorPickerDecorator(colorPicker, this.color);
                PixelPainter pixelPainter = new(bitmap, colorPickerWithTargetColor);

                if (RenderThreads == 1)
                {
                    foreach (var triangle in this.triangles)
                    {
                        ScanLine.Run(triangle, pixelPainter);
                    }
                }
                else
                {
                    var trianglesPerThread = (int)Math.Ceiling((float)this.triangles.Count / RenderThreads);
                    List<Task> tasks = new();

                    for (int i = 0; i < RenderThreads; i++)
                    {
                        tasks.Add(FillAsync(pixelPainter, i * trianglesPerThread, trianglesPerThread));
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

        private Task FillAsync(PixelPainter painter, int start, int step)
        {
            return Task.Run(
                () =>
                {
                    for (int j = start; j < start + step && j < this.triangles.Count; j++)
                    {
                        if (this.triangles[j].IsVisible)
                            ScanLine.Run(this.triangles[j], painter);
                    }
                });
        }

        public void Transform(int width, int height, Matrix4x4 view, Matrix4x4 position)
        {
            if (this.objectIndx % 2 == 0)
                this.rotationX += 2 * 2 * (float)Math.PI / height;
            else
                this.rotationY += 2 * 2 * (float)Math.PI / width;

            this.circleRotation += (float)Math.PI / Math.Min(height, width);

            var rotationX = Matrix4x4.CreateRotationX(this.rotationX);
            var rotationY = Matrix4x4.CreateRotationY(this.rotationY);
            var translate = Matrix4x4.CreateTranslation(radius);
            var rotation = Matrix4x4.CreateRotationZ(circleRotation);

            this.model = /*rotationX * rotationY **/ translate * rotation;

            foreach (var triangle in this.triangles)
            {
                triangle.SetRenderScale(width, height, this.model, view, position);
            }
        }

        internal void UpdateBarycentricCache()
        {
            foreach (var triangle in this.triangles)
            {
                Interpolator.CalculateBarycentricCache(triangle);
            }
        }
    }
}
