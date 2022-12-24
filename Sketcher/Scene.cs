using SketcherControl.Filling;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl
{
    public class Scene
    {
        #region Fields and Properties
        private List<Object3> objects;

        public LightSource LightSource { get; }
        public ColorPicker ColorPicker { get; }

        public IEnumerable<Object3> Objects => this.objects.ToList();
        public bool IsEmpty => !this.objects.Any();
        #endregion Fields and Properties

        public Scene()
        {
            this.objects = new();
            LightSource = new();
            ColorPicker = new(LightSource);

            LightSource.LightSourceChanged += ParametersChangedHandler;
            ColorPicker.ParametersChanged += ParametersChangedHandler;
        }


        #region Events
        public event Action<Scene>? SceneChanged;

        private void SendSceneChanged()
        {
            this.SceneChanged?.Invoke(this);
        }
        private void ParametersChangedHandler()
        {
            SendSceneChanged();
        }
        #endregion Events

        #region Loading Object
        public void LoadObjectFromFile(string fileName)
        {
            string fileContent;

            using (StreamReader reader = new StreamReader(fileName))
            {
                fileContent = reader.ReadToEnd();
            }

            LoadObject(fileContent);
        }


        public void LoadObject(string shapeObject)
        {
            List<Triangle> triangles = new List<Triangle>();
            PointF minPoint = new(float.MaxValue, float.MaxValue);
            PointF maxPoint = new(float.MinValue, float.MinValue);
            LightSource.MinZ = float.MinValue;
            List<Vertex> vertices = new List<Vertex>();
            List<Vector4> normalVectors = new List<Vector4>();

            string[] lines = shapeObject.Split("\n");

            foreach (var line in lines.Where((line) => line.StartsWith("v ")))
            {
                var values = line.Split(" ");
                var x = float.Parse(values[1]);
                var y = float.Parse(values[2]);
                var z = float.Parse(values[3]);
                vertices.Add(new Vertex(x, y, z));

                if (z > LightSource.MinZ)
                    LightSource.MinZ = z;

                if (x > maxPoint.X)
                    maxPoint.X = x;
                if (x < minPoint.X)
                    minPoint.X = x;
                if (y > maxPoint.Y)
                    maxPoint.Y = y;
                if (y < minPoint.Y)
                    minPoint.Y = y;
            }

            foreach (var line in lines.Where((line) => line.StartsWith("vn")))
            {
                var values = line.Split(" ");
                normalVectors.Add(new Vector4(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]), 0));
            }

            Triangle triangle = new();

            foreach (var line in lines.Where((line) => line.StartsWith("f")))
            {
                var faces = line.TrimStart('f', ' ').Split(" ");

                foreach (var face in faces)
                {
                    var vertexIndex = face.Split("//");
                    triangle.AddVertex(vertices[int.Parse(vertexIndex[0]) - 1], normalVectors[int.Parse(vertexIndex[1]) - 1]);
                }

                triangles.Add(triangle);
                triangle = new();
            }

            var objectSize = new SizeF(Math.Abs(maxPoint.X - minPoint.X), Math.Abs(maxPoint.Y - minPoint.Y));
            this.objects.Add(new Object3(triangles, objectSize, this.objects.Count - 1));
            SendSceneChanged();
        }
        #endregion

        public void Clear()
        {
            this.objects.Clear();
        }
    }
}
