using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SketcherControl.SceneManipulation
{
    internal class SceneRenderParameters
    {
        public Matrix4x4 View { get; set; }
        public Matrix4x4 Position { get; set; }
        public int ViewWidth { get; set; }
        public int ViewHeight { get; set; }
        public Vector3 LookVector { get; set; }
        public Vector3 CameraVector { get; set; }
        public bool ShowLines { get; set; }
        public bool Fill { get; set; }
        public bool Fog { get; set; }
    }

}
