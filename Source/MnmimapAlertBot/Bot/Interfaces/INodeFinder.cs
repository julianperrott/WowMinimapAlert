using System.Drawing;

namespace MinimapAlert
{
    public interface INodeFinder
    {
        Point? Find(bool highlight);
    }
}