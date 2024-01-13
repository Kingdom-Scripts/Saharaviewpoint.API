namespace Saharaviewpoint.Core.Models.Configurations;

public class FileSettings
{
    public string FullPath { get; set; }
    public string FilePath { get; set; }
    public string RequestPath { get; set; }
    public int MaxSizeLength { get; set; }
    public List<string> PermittedFileTypes { get; set; }
}
