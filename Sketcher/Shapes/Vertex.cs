using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Vertex
    {
        public Vector4 Location;
        public Vector4 NormalVector { get; set; }
        public Vector4 RenderLocation;

        public Color Color { get; set; }

        public event Action? RenderCoordinatesChanged;

        public Vertex(float x, float y, float z)
        {
            Location.X = x;
            Location.Y = y;
            Location.Z = z;

            RenderLocation.X = x;
            RenderLocation.Y = y;
        }

        public void Transform(int width, int height, Matrix4x4 model, Matrix4x4 view, Matrix4x4 position)
        {
            Location.W = 1;
            RenderLocation = Vector4.Transform(Location, model * view * position);
            RenderLocation /= RenderLocation.W;
            RenderLocation.X *= width / 2;
            RenderLocation.Y *= height / 2;
            RenderLocation.X += width / 2;
            RenderLocation.Y += height / 2;
            Location.W = 0;

            RenderCoordinatesChanged?.Invoke();
        }

        public void Render(DirectBitmap canvas)
        {
            RectangleF rectangle = new RectangleF(RenderLocation.X - SketcherConstants.VertexPointRadius, canvas.Height - RenderLocation.Y - SketcherConstants.VertexPointRadius,
                2 * SketcherConstants.VertexPointRadius, 2 * SketcherConstants.VertexPointRadius);

            using (var g = Graphics.FromImage(canvas.Bitmap))
            {
                g.FillEllipse(Brushes.Black, rectangle);
                g.DrawEllipse(Pens.Black, rectangle);
            }
        }
    }
}
