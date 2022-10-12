using Microsoft.AspNetCore.Mvc;
using bacit_dotnet.MVC.Models;
using System.Data.SqlClient;
using bacit_dotnet.MVC.DataAccess;
using MySql.Data.MySqlClient;

namespace bacit_dotnet.MVC.Controllers
{
    public class AccountController : Controller
    {
        //Get account
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        
        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}
