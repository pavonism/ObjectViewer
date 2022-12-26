using System.Numerics;

namespace SketcherControl.Shapes
{
    public class BarycentricCache
    {
        public Vector2 v0 { get; set; }
        public Vector2 v1 { get; set; }
        public float d00 { get; set; }
        public float d01 { get; set; }
        public float d11 { get; set; }
        public float denom { get; set; }
    }

    public abstract class Polygon
    {
        public Vertex[] Vertices { get; protected set; } = new Vertex[0];
        protected readonly List<Edge> edges = new();

        public int VertexCount { get; protected set; }
        public int EdgesCount => this.edges.Count;
        public IEnumerable<Edge> Edges => this.edges;

        public readonly Dictionary<(int, int), Vector3> CoefficientsCache = new();
        public readonly Dictionary<(int, int), Vector4> NormalVectorsCache = new();

        public BarycentricCache BarycentricCache { get; set; }
        public Matrix4x4 colors { get; set; }  
        public Color ConstShaderColor { get; set; }

        public Vector4 GetMeanNormal()
        {
            Vector4 mean = new();

            foreach (var vertex in Vertices)
            {
                mean += vertex.GlobalNormalVector;
            }

            return mean / this.Vertices.Length;
        }

        public Vector4 GetMeanCoordinates()
        {
            Vector4 mean = new();

            foreach (var vertex in Vertices)
            {
                mean += vertex.GlobalLocation;
            }

            return mean / this.Vertices.Length;
        }

        public virtual void GetMaxPoints(out Point max, out Point min)
        {
            var maxPoint = new PointF(float.MinValue, float.MinValue);
            var minPoint = new PointF(float.MaxValue, float.MaxValue);

            foreach (var vertex in Vertices)
            {
                maxPoint.X = Math.Max(maxPoint.X, vertex.RenderLocation.X);
                maxPoint.Y = Math.Max(maxPoint.Y, vertex.RenderLocation.Y);
                minPoint.X = Math.Min(minPoint.X, vertex.RenderLocation.X);
                minPoint.Y = Math.Min(minPoint.Y, vertex.RenderLocation.Y);
            }

            max = new Point((int)Math.Ceiling(maxPoint.X), (int)Math.Ceiling(maxPoint.Y));
            min = new Point((int)minPoint.X, (int)minPoint.Y);
        }

        internal void UpdateColorMatrix()
        {
            this.colors = new()
            {
                M11 = this.Vertices[2].Color.X,
                M21 = this.Vertices[0].Color.X,
                M31 = this.Vertices[1].Color.X,
                M12 = this.Vertices[2].Color.Y,
                M22 = this.Vertices[0].Color.Y,
                M32 = this.Vertices[1].Color.Y,
                M13 = this.Vertices[2].Color.Z,
                M23 = this.Vertices[0].Color.Z,
                M33 = this.Vertices[1].Color.Z,
            };
        }
    }
}
