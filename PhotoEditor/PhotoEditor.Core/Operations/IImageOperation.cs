using PhotoEditor.Core.Images;

namespace PhotoEditor.Core.Operations
{
    public interface IImageOperation
    {

        string Name { get; }

        IImage Apply(IImage source);
    }
}
