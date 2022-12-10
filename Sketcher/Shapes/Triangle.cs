
using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Triangle : Polygon
    {
        public Triangle()
        {
            this.Vertices = new Vertex[3];
        }

        public void AddVertex(Vertex vertex, Vector4? normalVector = null)
        {
            if (normalVector.HasValue)
                vertex.NormalVector = Vector4.Normalize(normalVector.Value);

            if (VertexCount < 3)
            {
                if(VertexCount > 0)
                {
                    this.edges.Add(new Edge(Vertices[VertexCount - 1], vertex));
                }
                if(VertexCount == 2)
                {
                    this.edges.Add(new Edge(vertex, Vertices[0]));
                }
                Vertices[VertexCount] = vertex;
            }

            VertexCount++;
            vertex.RenderCoordinatesChanged += RenderCoordinatesChangedHandler;
        }

        private void RenderCoordinatesChangedHandler()
        {
            CoefficientsCache.Clear();
            NormalVectorsCache.Clear();
        }

        public void SetRenderScale(float scale, float offsetX, float ofssetY)
        {
            foreach (var vertex in Vertices)
            {
                vertex.SetRenderSize(scale, offsetX, ofssetY);
            }
        }

        public void Render(DirectBitmap canvas)
        {
            foreach (var vertex in Vertices)
            {
                vertex.Render(canvas);
            }

            foreach (var edge in edges)
            {
                edge.Render(canvas);
            }
        }
    }
}
