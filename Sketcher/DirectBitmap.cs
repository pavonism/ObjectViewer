using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SketcherControl
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        private void InitializeBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public DirectBitmap(Bitmap bitmap) : this(bitmap.Width, bitmap.Height)
        {
            Load(bitmap);
        }

        public DirectBitmap(int width, int height)
        {
            InitializeBitmap(width, height);
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + ((Height - y) * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + ((Height - y - 1) * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Load(Bitmap bitmap)
        {
            if (bitmap.Width != Width || bitmap.Height != Height)
                InitializeBitmap(bitmap.Width, bitmap.Height);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    SetPixel(i, bitmap.Height - j, bitmap.GetPixel(i, j));
                }
            }
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
