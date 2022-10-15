using Microsoft.AspNetCore.Mvc;

namespace bacit_dotnet.MVC.Models
{
  public class AdminNewUserModel
    {
        public int emp_id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
    }
}
