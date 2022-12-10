
namespace SketcherControl.Shapes
{
    public class Edge
    {
        public Vertex From;
        public Vertex To;

        public float YMax => Math.Max(From.RenderLocation.Y, To.RenderLocation.Y);
        public float YMin => Math.Min(From.RenderLocation.Y, To.RenderLocation.Y);
        public float XMax => Math.Max(From.RenderLocation.X, To.RenderLocation.X);
        public float XMin => Math.Min(From.RenderLocation.X, To.RenderLocation.X);
        public float Slope { get; private set; }
        public float DrawingX { get; set; }
        public float Length => (float)Math.Sqrt(Math.Pow(From.RenderLocation.X - To.RenderLocation.X, 2) + Math.Pow(From.RenderLocation.Y - To.RenderLocation.Y, 2));

        public Edge(Vertex from, Vertex to)
        {
            From = from;
            To = to;

            UpdateSlope();
        }

        public void Render(DirectBitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap.Bitmap))
            {
                g.DrawLine(Pens.Black, From.RenderLocation.X, bitmap.Height - From.RenderLocation.Y, To.RenderLocation.X, bitmap.Height - To.RenderLocation.Y);
            }
        }

        public void UpdateSlope()
        {
            var lowerVertex = From.Location.Y < To.Location.Y ? From : To;
            var higherVertex = From != lowerVertex ? From : To;
            Slope = (higherVertex.RenderLocation.X - lowerVertex.RenderLocation.X) / (higherVertex.RenderLocation.Y - lowerVertex.RenderLocation.Y);
        }
    }
}
