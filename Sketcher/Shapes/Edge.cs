
namespace SketcherControl.Shapes
{
    public class Edge
    {
        public Vertex From;
        public Vertex To;

        public float YMax => Math.Max(From.RenderY, To.RenderY);
        public float YMin => Math.Min(From.RenderY, To.RenderY);
        public float XMax => Math.Max(From.RenderX, To.RenderX);
        public float XMin => Math.Min(From.RenderX, To.RenderX);
        public float Slope { get; private set; }
        public float DrawingX { get; set; }
        public float Length => (float)Math.Sqrt(Math.Pow(From.RenderX - To.RenderX, 2) + Math.Pow(From.RenderY - To.RenderY, 2));

        public Edge(Vertex from, Vertex to)
        {
            From = from;
            To = to;

            var lowerVertex = From.Location.Y < To.Location.Y ? From : To;
            var higherVertex = From != lowerVertex ? From : To;
            Slope = (higherVertex.Location.X - lowerVertex.Location.X) / (higherVertex.Location.Y - lowerVertex.Location.Y);
        }

        public void Render(DirectBitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap.Bitmap))
            {
                g.DrawLine(Pens.Black, From.RenderX, bitmap.Height - From.RenderY, To.RenderX, bitmap.Height - To.RenderY);
            }
        }
    }
}
