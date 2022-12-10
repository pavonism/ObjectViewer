using System.Numerics;
using System.Runtime.Intrinsics;

namespace SketcherControl.Filling
{
    public class ColorPickerWithScale : ColorPickerDecorator
    {
        private float scale;
        private float offsetX;
        private float offsetY;

        public ColorPickerWithScale(ColorPicker colorPicker, float scale, float offsetX, float offsetY) : base(colorPicker)
        {
            this.scale = scale;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public override Vector4 Scale(Vector4 vector)
        {
            return new Vector4()
            {
                X = vector.X * scale + offsetX,
                Y = vector.Y * scale + offsetY,
                Z = vector.Z,
                W = vector.W,
            };
        }

        public override Vector4 ScaleBack(Vector4 vector)
        {
            return new Vector4()
            {
                X = (vector.X - offsetX) / this.scale,
                Y = (vector.Y - offsetY) / this.scale,
                Z = vector.Z,
                W = vector.W,
            };
        }
    }
}
