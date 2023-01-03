using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public interface IPixelProcessor
    {
        void StartProcessing(Polygon polygon);
        void Process(Polygon polygon, int x, int y);
    }

    public class BarycentricProcessor : IPixelProcessor
    {
        public void Process(Polygon polygon, int x, int y)
        {
            Interpolator.CalculateCoefficients(polygon, x, y);
        }

        public void StartProcessing(Polygon polygon)
        {
        }
    }

    public class PixelPainter : IPixelProcessor
    {
        DirectBitmap bitmap;
        IShader shader;

        public PixelPainter(DirectBitmap bitmap, Shader shader)
        {
            this.bitmap = bitmap;
            this.shader = shader;
        }

        public void StartProcessing(Polygon polygon)
        {
            shader.StartFillingTriangle(polygon);
        }

        public void Process(Polygon polygon, int x, int y)
        {
            var interpolatedZ = Interpolator.InterpolateZ(polygon, x, y);
            if (interpolatedZ > bitmap.GetZ(x, y))
                return;

            var lightColor = this.shader.GetColor(polygon, x, y);
            bitmap.SetZ(x, y, interpolatedZ);
            bitmap.SetPixel(x, y, lightColor);
        }
    }

    public class PixelPainterWithFog : IPixelProcessor
    {
        DirectBitmap bitmap;
        IShader colorPicker;
        Vector4 backgroundColor = new Vector4(1, 1, 1, 0);
        Vector3 cameraVector;
        float fogDEnd = 10;
        float fogDStart = 0;

        public PixelPainterWithFog(DirectBitmap bitmap, Shader colorPicker, Vector3 cameraVector, Vector4 backgroundColor)
        {
            this.bitmap = bitmap;
            this.colorPicker = colorPicker;
            this.cameraVector = cameraVector;
            this.backgroundColor = backgroundColor;
        }

        public void StartProcessing(Polygon polygon)
        {
            colorPicker.StartFillingTriangle(polygon);
        }

        public void Process(Polygon polygon, int x, int y)
        {
            var interpolatedZ = Interpolator.InterpolateZ(polygon, x, y);
            if (interpolatedZ > bitmap.GetZ(x, y))
                return;

            var interpolatedX = Interpolator.InterpolateX(polygon, x, y);
            var interpolatedY = Interpolator.InterpolateY(polygon, x, y);

            Vector3 location = new()
            {
                X = interpolatedX,
                Y = interpolatedY,
                Z = interpolatedZ
            };

            var distance = (location - cameraVector).Length();

            if (distance > fogDEnd)
                return;

            var lightColor = this.colorPicker.GetColor(polygon, x, y);
            var fogCoefficient = CalculateDistanceCoefficient(distance);
            var colorWithFog = (fogCoefficient * lightColor.ToVector() + (1 - fogCoefficient) * backgroundColor).ToColor();

            bitmap.SetZ(x, y, interpolatedZ);
            bitmap.SetPixel(x, y, colorWithFog);
        }


        private float CalculateDistanceCoefficient(float d)
        {
            return (fogDEnd - d) / (fogDEnd - fogDStart);
        }
    }

    public class ScanLine
    {
        private static List<Edge>[] BucketSort(Polygon polygon, int minY, int maxY)
        {
            List<Edge>[] sortedEdges = new List<Edge>[maxY - minY + 1];

            foreach (var edge in polygon.Edges)
            {
                edge.DrawingX = edge.From.RenderLocation.Y == edge.YMin ? edge.From.RenderLocation.X : edge.To.RenderLocation.X;
                var index = (int)edge.YMin - minY;

                if (sortedEdges[index] == null)
                    sortedEdges[index] = new();

                sortedEdges[index].Add(edge);
            }

            return sortedEdges;
        }

        public static void Run(Polygon polygon, IPixelProcessor processor)
        {
            processor.StartProcessing(polygon);
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
                        var currentX = xi;
                        var currentY = y + minPoint.Y;

                        processor.Process(polygon, currentX, currentY);
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
