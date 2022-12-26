using SketcherControl.Filling;
using SketcherControl.Geometrics;
using SketcherControl.SceneManipulation;
using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Object3
    {
        private int objectIndx;
        private int RenderThreads = 20;
        private readonly List<Triangle> triangles = new();
        public Vector3 Location { get; private set; }

        public Matrix4x4 Model { get; private set; }
        public Matrix4x4 Rotation { get; protected set; } = Matrix4x4.Identity;
        public Matrix4x4 Translation { get; protected set; } = Matrix4x4.Identity;
        public Matrix4x4 Scale { get; protected set; } = Matrix4x4.Identity;
        public IEnumerable<Triangle> Triangles => this.triangles;

        public Color Color { get; set; }
        public virtual DirectBitmap? Texture { get; set; }
        public virtual DirectBitmap? NormalMap { get; set; }

        public IAnimation? Animation { get; set; }
        public Vector3 ObjectSize { get; private set; }

        public Object3(List<Triangle> triangles, Vector3 objectSize, int objectIndx)
        {
            this.triangles = triangles;
            ObjectSize = objectSize;
            this.objectIndx = objectIndx;

            if (this.objectIndx % 2 == 0)
                this.Color = SketcherConstants.ThemeColor;
            else
                this.Color = Color.MediumPurple;
        }

        public Object3()
        {
        }

        public void UpdateTrianglesVisibility(Vector3 lookVector)
        {
            foreach (var triangle in this.triangles)
            {
                triangle.UpdateVisibility(lookVector);
            }
        }

        public void Render(DirectBitmap bitmap, Vector3 cameraVector, bool showLines = true, IPixelProcessor? pixelProcessor = null)
        {
            if (pixelProcessor != null)
            {

                if (RenderThreads == 1)
                {
                    foreach (var triangle in this.triangles)
                    {
                        ScanLine.Run(triangle, pixelProcessor);
                    }
                }
                else
                {
                    var trianglesPerThread = (int)Math.Ceiling((float)this.triangles.Count / RenderThreads);
                    List<Task> tasks = new();

                    for (int i = 0; i < RenderThreads; i++)
                    {
                        tasks.Add(FillAsync(pixelProcessor, i * trianglesPerThread, trianglesPerThread));
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

        private Task FillAsync(IPixelProcessor painter, int start, int step)
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
            if(Animation != null)
            {
                this.Rotation *= Animation.GetRotation();
                this.Translation *= Animation.GetTranslation();
                UpdateModel();
            }

            foreach (var triangle in this.triangles)
            {
                triangle.SetRenderScale(width, height, Model, view, position);
            }
        }

        private void UpdateModel()
        {
            Model = Rotation * Scale * Translation;
        }

        public void MoveTo(float x, float y, float z)
        {
            this.Location = new Vector3(x, y, z);
            this.Translation = Matrix4x4.CreateTranslation(Location);
            UpdateModel();
        }

        public void SetScale(float scale)
        {
            this.Scale = Matrix4x4.CreateScale(scale);
            UpdateModel();
        }

        public void Rotate(Matrix4x4 rotation)
        {
            this.Rotation *= rotation;
            UpdateModel();
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
