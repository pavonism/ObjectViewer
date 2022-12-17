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

        public void SetRenderSize(int width, int height, Vector3 cameraPosition, float fov, float angleX = 0, float angleY = 0)
        {
            Location.W = 1;
            var rotationX = Matrix4x4.CreateRotationX(angleX);
            var rotationY = Matrix4x4.CreateRotationY(angleY);

            var modelMatrix = rotationX * rotationY;
            var viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            var positionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fov, (float)width / height, 1, 50);

            RenderLocation = Vector4.Transform(Location, modelMatrix * viewMatrix * positionMatrix);
            RenderLocation /= RenderLocation.W;
            RenderLocation.X *= width / 2;
            RenderLocation.Y *= height / 2;
            RenderLocation.X += width / 2;
            RenderLocation.Y += height / 2;
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
