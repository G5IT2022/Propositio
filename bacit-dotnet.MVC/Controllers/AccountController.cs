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
using System.Security.Claims;

namespace bacit_dotnet.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly IAdminRepository adminRepository;
        private readonly ITokenService tokenservice;
        private readonly IConfiguration configuration;
        private string generatedToken = null;

        public AccountController(IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository, IAdminRepository adminRepository, ITokenService tokenservice, IConfiguration configuration)
        {
            this.employeeRepository = employeeRepository;
            this.suggestionRepository = suggestionRepository;
            this.adminRepository = adminRepository;
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
            //Finnes brukeren som prøver å logge inn
            bool userExists = adminRepository.UserExists(model.emp_id);
            if (userExists)
            {
                //Ja det gjør de, sjekk om passordet stemmer overens
                EmployeeEntity emp = new EmployeeEntity();
                if (model.emp_id <= 10)
                {
                    emp = adminRepository.AuthenticateUser(model.emp_id, model.password);

                }
                else
                {
                    byte[] salt = adminRepository.GetSalt(model.emp_id);
                    byte[] password = PassHash.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(model.password), salt);
                    string passwordstring = Convert.ToBase64String(password);
                    emp = adminRepository.AuthenticateUser(model.emp_id, passwordstring);

                }
                if(emp == null)
                {
                    ViewBag.ErrorMessage = "Passordet er feil, prøv igjen.";
                    return View("LogIn", new AccountViewModel());
                }

                emp.authorizationRole = adminRepository.AuthorizeUser(emp.emp_id);
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
            else
            {
                ViewBag.ErrorMessage = $"Finner ikke brukeren med ansattnummer: {model.emp_id}";
                return View("LogIn", new AccountViewModel());
            }
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn");
        }

        public IActionResult MyAccount()
        {
            MyAccountViewModel model = new MyAccountViewModel();
            model.employee = employeeRepository.GetEmployee(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)));
            model.employee.suggestions = suggestionRepository.GetSuggestionsByAuthorID(model.employee.emp_id);
            model.teams = new List<TeamEntity>();
            var teamCount = model.employee.teams.Count();
            for (int i = 0; i < teamCount; i++)
            {
                model.teams.Add(employeeRepository.GetTeam(model.employee.teams.ElementAt(i).team_id));
            }
            return View(model);
        }

    }
}
