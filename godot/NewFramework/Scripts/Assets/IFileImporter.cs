namespace BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public interface IFileImporter
{
    public byte[] GetFileContent(string path);
    public string GetFileText(string path);
    public JsonWrapper GetJsonParsed(string path);
}