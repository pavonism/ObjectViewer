
using System.Numerics;

namespace SketcherControl.Shapes
{
    public class Triangle : Polygon
    {
        public Triangle()
        {
            this.Vertices = new Vertex[3];
        }

        public bool IsVisible { get; private set; }
        private bool isInCube;

        public Triangle(Triangle triangle)
        {
            Vertices = new Vertex[3];

            for (int i = 0; i < Vertices.Length; i++)
            {
                AddVertex(new Vertex(triangle.Vertices[i]));
            }
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
        }

        public void SetRenderScale(int width, int height, Matrix4x4 model, Matrix4x4 view, Matrix4x4 position)
        {
            isInCube = true;

            foreach (var vertex in Vertices)
            {
                if (!vertex.Transform(width, height, model, view, position))
                    isInCube = false;
            }

            foreach (var edge in Edges)
            {
                edge.UpdateSlope();
            }

            CoefficientsCache.Clear();
            NormalVectorsCache.Clear();
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

        public void UpdateVisibility(Vector3 lookVector)
        {
            if (!isInCube)
            {
                IsVisible = false;
                return;
            }

            bool visible = false;
            foreach (var vertex in Vertices)
            {
                if (Vector4.Dot(vertex.GlobalNormalVector, new Vector4(lookVector, 0)) < 0)
                {
                    visible = true;
                    break;
                }
            }

            IsVisible = visible;
        }
    }
}
