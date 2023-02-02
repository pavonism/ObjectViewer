using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace SketcherControl.Filling
{
    public class Light
    {
        #region Fields and Events
        public event Action? LightSourceChanged;

        private Object3? shape; 
        public Object3? Shape
        {
            get => this.shape;
            set
            {
                if(this.shape != value) 
                {
                    this.shape = value;
                    if(this.shape != null)
                    {
                        this.shape.Color = this.color;
                        this.shape.MoveTo(SceneLocation.X, SceneLocation.Y, SceneLocation.Z);
                    }
                }
            }
        }
        private Vector4 sceneLocation;
        private Vector4 colorVector;
        private Color color;
        #endregion

        #region Properties
        public float MinZ { get; set; } = 0;
        public virtual Vector4 SceneLocation
        {
            get => this.sceneLocation;
            set
            {
                if(this.sceneLocation != value)
                {
                    this.sceneLocation = value;
                    Shape?.MoveTo(sceneLocation.X, sceneLocation.Y, sceneLocation.Z);
                }
            }
        }
        public bool Show { get; set; }

        public Color Color
        {
            get => this.color;
            set
            {
                this.color = value;
                this.colorVector = value.ToVector();
                
                if(Shape!= null)
                {
                    Shape.Color = value;
                }
            }
        }

        public Vector4 ColorVector
        {
            get => this.colorVector;
            set
            {
                this.colorVector = value;
                this.color = value.ToColor();

                if (Shape != null)
                {
                    Shape.Color = this.color;
                }
            }
        }
        #endregion Properties

        #region Initialization
        public Light()
        {
        }
        #endregion

        public virtual Vector4 AdjustColorFromShader(Vector4 shaderColor, Vector4 pointLocation)
        {
            return shaderColor;
        }
        public virtual void RenderShape(DirectBitmap bitmap, Vector3 cameraVector, bool showLines, IPixelProcessor? pixelProcessor)
        {
            Shape?.Render(bitmap, cameraVector, showLines, pixelProcessor);
        }
    }
}
