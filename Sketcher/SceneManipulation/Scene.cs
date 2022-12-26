using SketcherControl.Filling;
using SketcherControl.Shapes;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace SketcherControl.SceneManipulation
{
    public class Scene
    {
        #region Fields and Properties
        private List<Object3> objects;

        public LightSource LightSource { get; }
        public ColorPicker ColorPicker { get; }

        public IEnumerable<Object3> Objects => objects.ToList();
        public bool IsEmpty => !objects.Any();
        public Object3 MovingObject { get; set; }
        #endregion Fields and Properties

        public Scene()
        {
            objects = new();
            LightSource = new();
            ColorPicker = new(LightSource);

            LightSource.LightSourceChanged += ParametersChangedHandler;
            ColorPicker.ParametersChanged += ParametersChangedHandler;
        }


        #region Events
        public event Action<Scene>? SceneChanged;

        private void SendSceneChanged()
        {
            SceneChanged?.Invoke(this);
        }
        private void ParametersChangedHandler()
        {
            SendSceneChanged();
        }
        #endregion Events

        #region Loading Object
        public string LoadObjectDataFromFile(string fileName)
        {
            string fileContent;

            using (StreamReader reader = new StreamReader(fileName))
            {
                fileContent = reader.ReadToEnd();
            }

            return fileContent;
        }

        public Object3 AddObjectFromFile(string fileName)
        {
            var fileContent = LoadObjectDataFromFile(fileName);
            return AddObjectFromStringData(fileContent);
        }

        public Object3 AddObjectFromStringData(string shapeObject)
        {
            List<Triangle> triangles = new List<Triangle>();
            Vector3 minPoint = new(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxPoint = new(float.MinValue, float.MinValue, float.MinValue);

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
                if (z > maxPoint.Z)
                    maxPoint.Z = z;
                if (z < minPoint.Z)
                    minPoint.Z = z;
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

            var objectSize = new Vector3(Math.Abs(maxPoint.X - minPoint.X), Math.Abs(maxPoint.Y - minPoint.Y), Math.Abs(maxPoint.Z - minPoint.Z));
            var result = new Object3(triangles, objectSize, objects.Count - 1);
            objects.Add(result);
            SendSceneChanged();
            return result;
        }
        #endregion

        public void Clear()
        {
            objects.Clear();
        }
    }
}
