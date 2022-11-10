using Microsoft.Extensions.Hosting.Internal;

namespace bacit_dotnet.MVC.Repositories
{
    public class FileRepository : IFileRepository
    {
        readonly IWebHostEnvironment hostingEnvironment;
        public FileRepository(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        //upload bilde/fil
        public bool UploadFile(IFormFile file)
        {
            string path = "";

            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, hostingEnvironment.WebRootPath, "uploadedImages"));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        file.CopyToAsync(fileStream);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }
}
