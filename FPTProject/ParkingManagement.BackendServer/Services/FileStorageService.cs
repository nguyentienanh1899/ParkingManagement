using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Services
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        private const string USER_CONTENT_FOLDER_NAME = "user-attachments";
        /// <summary>
        /// Combines two strings into a path. 
        /// </summary>
        /// <param name="webHostEnvironment">
        /// String 1: Path of wwwroot in asembly
        /// String 2: Name of folder USER_CONTENT_FOLDER_NAME
        /// </param>
        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, USER_CONTENT_FOLDER_NAME);
        }
        // get url of file in user-attachments folder
        public string GetFileUrl(string fileName)
        {
            return $"/{USER_CONTENT_FOLDER_NAME}/{fileName}";
        }
        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {
            //Determines whether the given path refers to an existing directory on disk.
            if (!Directory.Exists(_userContentFolder))
                Directory.CreateDirectory(_userContentFolder);
            //Combine the path to the directory with the filename
            var filePath = Path.Combine(_userContentFolder, fileName);
            //Opens the file if it exists and seeks to the end of the file, or creates a new file. 
            using var output = new FileStream(filePath, FileMode.Create);
            //Asynchronously reads the bytes from the current stream and writes them to another stream.
            await mediaBinaryStream.CopyToAsync(output);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}
