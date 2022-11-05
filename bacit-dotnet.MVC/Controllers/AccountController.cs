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
using System.Web.WebPages;
using bacit_dotnet.MVC.Helpers;

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
            if (model.password.IsEmpty())
            {
                ViewBag.ErrorMessage = "Vennligst skriv inn et passord";
            }
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
                if (emp == null)
                {
                    ViewBag.ErrorMessage += " Passordet er feil, prøv igjen.";
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
                if (model.password.IsEmpty())
                {
                    ViewBag.ErrorMessage = "Vennligst skriv inn et passord";
                }
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

        public IActionResult MyAccount(string sortOrder, string searchString, string filterParameter)
        {
            MyAccountViewModel model = new MyAccountViewModel();
            model.employee = employeeRepository.GetEmployee(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)));
            model.suggestions = suggestionRepository.GetSuggestionsByAuthorID(model.employee.emp_id);
            foreach (SuggestionEntity suggestion in model.suggestions)
            {
                suggestion.author = employeeRepository.GetEmployee(suggestion.author_emp_id);
                suggestion.responsible_employee = employeeRepository.GetEmployee(suggestion.ownership_emp_id);
            }
            model.teams = new List<TeamEntity>();
            var teamCount = model.employee.teams.Count();
            for (int i = 0; i < teamCount; i++)
            {
                model.teams.Add(employeeRepository.GetTeam(model.employee.teams.ElementAt(i).team_id));
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                //Sjekker om søkestrengen finnes i tittelen, beskrivelsen eller navnet på forfatter/ansvarlig
                //Måtte lage et midlertidig variabel for å søke med fordi jeg fikk ikke lov til å søke på samlignen som allerede var der
                var searched = model.suggestions.Where(
                   s => s.title.Contains(searchString) ||
                   s.description.Contains(searchString) ||
                   s.author.name.Contains(searchString) ||
                   s.responsible_employee.name.Contains(searchString));
                //Setter listen over forslag i modellen til de man har søkt etter
                model.suggestions = searched.ToList();
            }
            if (filterParameter != "noFilter")
            {
                model.suggestions = FilterHelper.FilterSuggestions(model.suggestions, filterParameter);
            }

            //Switch statement som sjekker hva du sorterer på, switch er basically en if/elseif/else men lettere syntax syntes jeg
            switch (sortOrder)
            {
                case "name_asc":
                    var sortedNameAsc = model.suggestions.OrderBy(s => s.author.name);
                    model.suggestions = sortedNameAsc.ToList();
                    break;
                case "name_desc":
                    var sortedNameDesc = model.suggestions.OrderByDescending(s => s.author.name);
                    model.suggestions = sortedNameDesc.ToList();
                    break;
                case "date_old":
                    var sortedDateOld = model.suggestions.OrderBy(s => s.timestamp.createdTimestamp);
                    model.suggestions = sortedDateOld.ToList();
                    break;
                case "date_new":
                    var sortedDateNew = model.suggestions.OrderBy(s => s.timestamp.createdTimestamp);
                    model.suggestions = sortedDateNew.ToList();
                    break;
                default:
                    break;

            }
            //Hvis man sitter igjen med null forslag på slutten av søket/filtreringen får man en feilmelding
            if (model.suggestions.Count <= 0)
            {
                ViewBag.SortedMessage = "Fant ingen forslag med dine søkekriterier :(";
            }
            return View(model);
        }

    }
}
