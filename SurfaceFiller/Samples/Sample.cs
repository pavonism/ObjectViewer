using SurfaceFiller.Components;
using System.Collections;

namespace SurfaceFiller.Samples
{
    internal abstract class BasicSample
    {
        public string Name { get; set; }

        public BasicSample(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    internal abstract class Sample : BasicSample, IComboItem
    {
        protected Sample(string name) : base(name)
        {
        }

        public abstract Image GetThumbnail(int width, int height);
    }
}
