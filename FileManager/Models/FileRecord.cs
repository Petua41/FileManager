using FileManager.Infrastructure.Extensions;

namespace FileManager.Models
{
    public readonly record struct FileRecord(string Filename, string Icon, FileType Type, string FullPath);
}
