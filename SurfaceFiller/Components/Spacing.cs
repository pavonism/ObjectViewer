using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceFiller.Components
{
    internal class Spacing : Divider
    {
        public Spacing(int space)
        {
            Margin = new Padding(0, space, 0, 0);
            Height = 0;
        }
    }
}
