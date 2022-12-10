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

        public void SetRenderSize(float scale, float offsetX, float offsetY, float angleX = 0, float angleY = 0)
        {
            Location.W = 1;
            var translation = Matrix4x4.CreateTranslation(offsetX, offsetY, 0);
            var scaleMatrix = Matrix4x4.CreateScale(scale);
            var rotationX = Matrix4x4.CreateRotationX(angleX);
            var rotationY = Matrix4x4.CreateRotationY(angleY);

            RenderLocation = Vector4.Transform(Location, rotationY * rotationX * scaleMatrix * translation);
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
