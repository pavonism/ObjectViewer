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
        private List<LightSource> lightSources;

        public LightSource LightSource => this.lightSources.First();
        public ColorPicker ColorPicker { get; private set; }

        public IEnumerable<Object3> Objects => objects.ToList();
        public IEnumerable<LightSource> Lights => lightSources.ToList();
        public bool IsEmpty => !objects.Any();
        public Object3 MovingObject { get; set; }
        #endregion Fields and Properties

        public Scene()
        {
            objects = new();
            lightSources = new();

        }


        #region Events
        public event Action<Scene>? SceneChanged;

        public void AddObject(Object3 obj)
        {
            this.objects.Add(obj);
            SendSceneChanged();
        }

        public void AddLightSource(LightSource lightSource)
        {
            this.lightSources.Add(lightSource);
            lightSource.LightSourceChanged += ParametersChangedHandler;
            ColorPicker = new(LightSource);
            ColorPicker.ParametersChanged += ParametersChangedHandler;
            SendSceneChanged();
        }

        private void SendSceneChanged()
        {
            SceneChanged?.Invoke(this);
        }
        private void ParametersChangedHandler()
        {
            SendSceneChanged();
        }
        #endregion Events

        public void Clear()
        {
            objects.Clear();
        }
    }
}
