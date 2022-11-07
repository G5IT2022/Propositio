namespace bacit_dotnet.MVC.Repositories
{
    public interface IFileRepository
    {
        public bool UploadFile (IFormFile file);
    }
}
