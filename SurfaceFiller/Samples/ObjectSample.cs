
namespace SurfaceFiller.Samples
{
    internal class ObjectSample : BasicSample
    {
        public string Path { get; private set; }

        public ObjectSample(string path, string name) : base(name)
        {
            Path = path;
        }
    }
}
