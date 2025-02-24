// Context/FileSearchContext.cs
using FileSearchService.Model;

namespace FileSearchService.Context
{
    public class FileSearchContext
    {
        public IEnumerable<FileInfoResponse> SearchFiles(string drive, string searchPattern)
        {
            var root = new DirectoryInfo($"{drive}:\\");
            var queue = new Queue<DirectoryInfo>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var currentDir = queue.Dequeue();

                foreach (var file in GetFilesSafe(currentDir, searchPattern))
                {
                    yield return new FileInfoResponse
                    {
                        FileName = file.Name,
                        FullPath = file.FullName,
                        SizeBytes = file.Length,
                        CreationDate = file.CreationTime
                    };
                }

                foreach (var subDir in GetDirectoriesSafe(currentDir))
                {
                    queue.Enqueue(subDir);
                }
            }
        }

       private IEnumerable<FileInfo> GetFilesSafe(DirectoryInfo dir, string pattern)
        {
            try
            {
                return dir.EnumerateFiles($"*{pattern}*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Access error {dir.FullName}: {ex.Message}");
                return Enumerable.Empty<FileInfo>();
            }
        }

        private IEnumerable<DirectoryInfo> GetDirectoriesSafe(DirectoryInfo dir)
        {
            try
            {
                return dir.EnumerateDirectories();
            }
            catch
            {
                return new List<DirectoryInfo>();
            }
        }
    }
}

