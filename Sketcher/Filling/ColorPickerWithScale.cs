using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;

namespace SketcherControl.Filling
{
    public class ColorPickerWithScale : ColorPickerDecorator
    {
        private Matrix4x4 invertTransform;
        private Matrix4x4 transform;

        public int Width { get; set; }
        public int Height { get; set; }
        public override Color TargetColor { get; set; }

        public ColorPickerWithScale(ColorPicker colorPicker, Matrix4x4 transform, int width, int height) : base(colorPicker)
        {
            this.transform = transform;
            Width = width;
            Height = height;
            Matrix4x4.Invert(this.transform, out this.invertTransform);
        }

        public override Vector4 Scale(Vector4 vector)
        {
            var renderLocation = vector;

            renderLocation.W = 1;
            renderLocation = Vector4.Transform(renderLocation, transform);
            renderLocation /= renderLocation.W;
            renderLocation.X *= Width / 2;
            renderLocation.Y *= Height / 2;
            renderLocation.X += Width / 2;
            renderLocation.Y += Height / 2;
            renderLocation.W = 0;

            return renderLocation;
        }

        public override Vector4 ScaleBack(Vector4 vector)
        {
            var modelLocation = vector;
            modelLocation.Y -= Height / 2;
            modelLocation.X -= Width / 2;
            modelLocation.Y /= Height / 2;
            modelLocation.X /= Width / 2;

            modelLocation.W = 1;
            modelLocation = Vector4.Transform(modelLocation, transform);
            modelLocation /= modelLocation.W;
            modelLocation.W = 0;

            return modelLocation;
        }
    }
}
