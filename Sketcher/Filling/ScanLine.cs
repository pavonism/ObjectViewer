using SketcherControl.Shapes;

namespace SketcherControl.Filling
{
    public class ScanLine
    {
        private static List<Edge>[] BucketSort(Polygon polygon, int minY, int maxY)
        {
            List<Edge>[] sortedEdges = new List<Edge>[maxY - minY + 1];

            foreach (var edge in polygon.Edges)
            {
                edge.DrawingX = edge.From.RenderY == edge.YMin ? edge.From.RenderX : edge.To.RenderX;
                var index = (int)edge.YMin - minY;

                if (sortedEdges[index] == null)
                    sortedEdges[index] = new();

                sortedEdges[index].Add(edge);
            }

            return sortedEdges;
        }

        public static void Fill(Polygon polygon, DirectBitmap canvas, ColorPicker colorPicker)
        {
            colorPicker.StartFillingTriangle(polygon.Vertices);
            polygon.GetMaxPoints(out var maxPoint, out var minPoint);

            var ET = BucketSort(polygon, minPoint.Y, maxPoint.Y);
            int ETCount = polygon.EdgesCount;
            int y = 0;
            List<Edge> AET = new();

            while (ETCount > 0 || AET.Count > 0)
            {
                if (ET[y] != null)
                {
                    AET.AddRange(ET[y]);
                    ETCount -= ET[y].Count;
                }

                AET.Sort((e1, e2) => e1.DrawingX.CompareTo(e2.DrawingX));

                AET.RemoveAll((edge) => (int)edge.YMax <= y + minPoint.Y);

                for (int i = 1; i < AET.Count; i += 2)
                {
                    for (int xi = (int)AET[i - 1].DrawingX; xi <= AET[i].DrawingX; xi++)
                    {
                        var color = colorPicker.GetColor(polygon, xi, y + minPoint.Y);
                        canvas.SetPixel(xi, y + minPoint.Y, color);
                    }
                }

                y++;

                foreach (var edge in AET)
                {
                    edge.DrawingX += edge.Slope;
                }
            }
        }
    }
}
