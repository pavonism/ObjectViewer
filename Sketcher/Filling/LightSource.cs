using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Numerics;

namespace SketcherControl.Filling
{
    public class LightSource
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
                    value?.MoveTo(SceneLocation.X, SceneLocation.Y, SceneLocation.Z);
                }
            }
        }
        private Vector4 lightSourceColor = new Vector4(0.5f, 1, 1, 0);
        #endregion

        #region Properties
        public float MinZ { get; set; } = 0;
        public Vector4 SceneLocation { get; set; } = new Vector4(0, 0, 4, 0);
        public bool Show { get; set; }

        public Color LightSourceColor
        {
            get => this.lightSourceColor.ToColor();
            set
            {
                if (this.lightSourceColor.ToColor() == value)
                    return;

                this.lightSourceColor = value.ToVector();
                this.LightSourceChanged?.Invoke();
            }
        }

        public Vector4 LightSourceVector => this.lightSourceColor;
        #endregion Properties

        #region Initialization
        public LightSource()
        {
        }
        #endregion

        #region Rendering
        public void Render(DirectBitmap bitmap, bool showLines = true, ColorPicker? colorPicker = null)
        {
            if(Shape != null)
            {
                var picker = colorPicker != null ? new LightSourceColorPicker(colorPicker, LightSourceColor) : null;
                Shape.RenderWithPicker(bitmap, showLines, picker);
            }
        }
        #endregion
    }

    public class LightSourceColorPicker : ColorPickerDecorator
    {
        private Color lightColor;

        public LightSourceColorPicker(ColorPicker colorPicker, Color lightColor) : base(colorPicker)
        {
            this.lightColor = lightColor;
        }

        public override Color GetColor(Polygon polygon, int x, int y)
        {
            return lightColor;
        }

        public override void StartFillingTriangle(Polygon polugon)
        {
        }
    }
}
