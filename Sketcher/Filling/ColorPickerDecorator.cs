using System.Numerics;

namespace SketcherControl.Filling
{
    public class ColorPickerDecorator : ColorPicker
    {
        public ColorPicker _colorPicker;

        public ColorPickerDecorator(ColorPicker colorPicker)
        {
            _colorPicker = colorPicker;
        }

        public override Interpolation InterpolationMode { get => _colorPicker.InterpolationMode; set => _colorPicker.InterpolationMode = value; }
        public override float KD { get => _colorPicker.KD; set => _colorPicker.KD = value; }
        public override float KS { get => _colorPicker.KS; set => _colorPicker.KD = value; }
        public override int M { get => _colorPicker.M; set => _colorPicker.KD = value; }
        public override Color TargetColor { get => _colorPicker.TargetColor; set => _colorPicker.TargetColor = value; }
        public override DirectBitmap? Pattern { get => _colorPicker.Pattern; set => _colorPicker.Pattern = value; }
        public override DirectBitmap? NormalMap { get => _colorPicker.NormalMap; set => _colorPicker.NormalMap = value; }
        public override LightSource LightSource { get => _colorPicker.LightSource; set =>_colorPicker.LightSource = value; }
    }
}
