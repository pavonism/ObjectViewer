using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;

namespace SketcherControl.Filling
{
    public class TargetColorColorPickerDecorator : ColorPickerDecorator
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public override Color TargetColor { get; set; }

        public TargetColorColorPickerDecorator(ColorPicker colorPicker, Color color) : base(colorPicker)
        {
            TargetColor = color;
        }
    }
}
