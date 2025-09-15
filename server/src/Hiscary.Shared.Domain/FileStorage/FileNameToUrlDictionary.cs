namespace Hiscary.Shared.Domain.FileStorage;

public sealed class FileNameToUrlDictionary : Dictionary<string, string>
{
    public FileNameToUrlDictionary(Dictionary<string, string> value)
        : base(value ?? throw new ArgumentNullException(nameof(value)))
    {
    }
}
