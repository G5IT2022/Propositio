using Microsoft.AspNetCore.Mvc;
using bacit_dotnet.MVC.Models;
using System.Data.SqlClient;
using bacit_dotnet.MVC.DataAccess;
using MySql.Data.MySqlClient;
using bacit_dotnet.MVC.Repositories;
using bacit_dotnet.MVC.Entities;
using System.Web;
using Microsoft.AspNetCore.Components.Authorization;
using bacit_dotnet.MVC.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace bacit_dotnet.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ITokenService tokenservice;
        private readonly IConfiguration configuration;
        private string generatedToken = null;

        public AccountController(IEmployeeRepository employeeRepository, ITokenService tokenservice, IConfiguration configuration)
        {
            this.employeeRepository = employeeRepository;
            this.tokenservice = tokenservice;
            this.configuration = configuration;
        }


        //Get account
        [HttpGet]
        public IActionResult LogIn(AccountViewModel model)
        {
            return View(model);
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Verify(AccountViewModel model)
        {
            EmployeeEntity emp = employeeRepository.GetEmployee(model.emp_id);
            if (emp == null)
            {
                ViewBag.ErrorMessage = "Ansattnr eller passord er feil, vennligst prøv igjen.";
                return View("LogIn", new AccountViewModel());
            }
            if (model.emp_id <= 10)
            {
                emp = employeeRepository.DummyAuthenticate(model.emp_id, model.password);

            }
            else
            {
                byte[] password = PassHash.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(model.password), emp.salt);
                string passwordstring = Convert.ToBase64String(password);
                emp = employeeRepository.DummyAuthenticate(model.emp_id, passwordstring);
                
            }
            if (emp == null)
            {
                ViewBag.ErrorMessage = "Passordet er feil, vennligst prøv igjen.";
                return View("LogIn", new AccountViewModel());
            }

            emp.authorizationRole = employeeRepository.GetEmployeeRoleName(emp.authorization_role_id);
            generatedToken = tokenservice.BuildToken(configuration["Jwt:Key"].ToString(), configuration["Jwt:Issuer"].ToString(), emp);
            if (generatedToken != null)
            {
                HttpContext.Session.SetString("Token", generatedToken);
                return RedirectToAction("Index", "Suggestion");
            }
            else
            {
                ViewBag.ErrorMessage = "Login feilet, vennligst prøv igjen.";
                return View("LogIn", new AccountViewModel());
            }
        }
        [AllowAnonymous]
        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult MyAccount()
        {
            MyAccountViewModel model = new MyAccountViewModel();
            model.employees = employeeRepository.GetEmployees();
            model.teams = employeeRepository.GetTeams();
            return View(model);
        }

    }
}
