namespace SketcherControl.Filling
{
    public class TargetColorColorPickerDecorator : ColorPickerDecorator
    {
        public override Color TargetColor { get; set; }

        public TargetColorColorPickerDecorator(ColorPicker colorPicker, Color color) : base(colorPicker)
        {
            TargetColor = color;
        }
    }
}
