using SketcherControl.Filling;
using SketcherControl.Shapes;

namespace SketcherControl.SceneManipulation
{
    public class Scene
    {
        #region Fields and Properties
        private List<Object3> objects;
        private List<Light> lightSources;
        public Light LightSource => this.lightSources.First();

        public IEnumerable<Object3> Objects => objects.ToList();
        public IEnumerable<Light> Lights => lightSources.ToList();
        public bool IsEmpty => !objects.Any();
        public Object3 MovingObject { get; set; }
        public Reflector Reflector { get; set; }

        private Shader shader;
        public Shader Shader
        {
            get => this.shader;
            set
            {
                if(this.shader != value) 
                {
                    this.shader = value;
                    SendSceneChanged();
                }
            }
        }
        #endregion Fields and Properties

        #region Initialization
        public Scene()
        {
            objects = new();
            lightSources = new();
        }
        #endregion

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

        #region Public methods
        public void AddObject(Object3 obj)
        {
            this.objects.Add(obj);
            SendSceneChanged();
        }

        public void AddLightSource(Light lightSource)
        {
            this.lightSources.Add(lightSource);
            lightSource.LightSourceChanged += ParametersChangedHandler;
            SendSceneChanged();
        }

        public void Clear()
        {
            objects.Clear();
        }
        #endregion Public methods
    }
}
