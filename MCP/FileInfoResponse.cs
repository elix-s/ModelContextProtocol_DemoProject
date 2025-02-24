// Model/FileInfoResponse.cs
namespace FileSearchService.Model
{
    public class FileInfoResponse
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public long SizeBytes { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
