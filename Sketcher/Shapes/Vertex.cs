using SketcherControl.Geometrics;
using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Vertex
    {
        public Vector4 Location;
        public Vector4 NormalVector { get; set; }
        public Vector4 GlobalNormalVector;
        public Vector4 RenderLocation;
        public Vector4 GlobalLocation;
        public Vector4 Color { get; set; }

        public Vertex(float x, float y, float z)
        {
            Location.X = x;
            Location.Y = y;
            Location.Z = z;

            RenderLocation.X = x;
            RenderLocation.Y = y;
        }

        public Vertex(Vertex vertex)
        {
            Location = vertex.Location;
            NormalVector = vertex.NormalVector;
            GlobalNormalVector = vertex.GlobalNormalVector;
            RenderLocation = vertex.RenderLocation;
            GlobalLocation = vertex.GlobalLocation;
            Color = vertex.Color;
        }

        public bool Transform(int width, int height, Matrix4x4 model, Matrix4x4 view, Matrix4x4 position)
        {
            Location.W = 1;
            var globalNormal = Vector3.TransformNormal(new Vector3(NormalVector.X, NormalVector.Y, NormalVector.Z), model);
            GlobalNormalVector = new Vector4(globalNormal, 0);
            GlobalLocation = Vector4.Transform(Location, model);
            RenderLocation = Vector4.Transform(GlobalLocation, view * position);
            RenderLocation /= RenderLocation.W;

            if(Math.Abs(RenderLocation.X) > 1 || Math.Abs(RenderLocation.Y) > 1 || Math.Abs(RenderLocation.Z) > 1)
                return false;

            RenderLocation.X *= width / 2;
            RenderLocation.Y *= height / 2;
            RenderLocation.X += width / 2;
            RenderLocation.Y += height / 2;
            Location.W = 0;
            GlobalLocation.W = 0;

            return true;
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
