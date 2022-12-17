﻿
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

        public void SetRenderScale(int width, int height, Matrix4x4 model, Matrix4x4 view, Matrix4x4 position)
        {
            foreach (var vertex in Vertices)
            {
                vertex.Transform(width, height, model, view, position);
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
            if (!IsVisible(canvas.Width, canvas.Height))
                return;

            foreach (var vertex in Vertices)
            {
                vertex.Render(canvas);
            }

            foreach (var edge in edges)
            {
                edge.Render(canvas);
            }
        }

        public bool IsVisible(int width, int height)
        {
            GetMaxPoints(out var max, out var min);
            return max.X < width && max.Y < height && min.X >= 0 && min.Y >= 0;
        }
    }
}
