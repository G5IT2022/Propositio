namespace bacit_dotnet.MVC.DataAccess
{
    public static class FilePathBuilder
    {
        public static string GetRelativeFilepath(string fileName){
            return "~/img/" + fileName;
        }
    }
}
