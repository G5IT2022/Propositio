namespace bacit_dotnet.MVC.DataAccess
{
    public static class FilePathBuilder
    {
        public static string GetRelativeFilepath(string fileName){
            //Path.Combine(Environment.CurrentDirectory, "UploadedFiles/") + fileName;
            //return Path.Combine(Environment.CurrentDirectory, "wwwroot/uploadedImages/") + fileName;
            return "~/uploadedImages/" + fileName;
        }
    }
}
