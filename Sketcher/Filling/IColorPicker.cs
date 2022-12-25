using SketcherControl.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketcherControl.Filling
{
    public interface IColorPicker
    {
        void StartFillingTriangle(Polygon polugon);
        public Color GetColor(Polygon polygon, int x, int y);
        float InterpolateZ(Polygon polygon, int x, int y);
    }
}
