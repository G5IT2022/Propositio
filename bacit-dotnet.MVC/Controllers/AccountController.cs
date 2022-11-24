using Microsoft.AspNetCore.Mvc;
using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Repositories;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Claims;
using System.Web.WebPages;
using bacit_dotnet.MVC.Helpers;
using Microsoft.AspNetCore.Authentication;

namespace bacit_dotnet.MVC.Controllers
{
        [Authorize]
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly IAdminRepository adminRepository;
        private readonly ITokenService tokenservice;
        private readonly IConfiguration configuration;
        private readonly ILogger<AccountController> logger;
        private string generatedToken = null;
        public AccountController(ILogger<AccountController> logger, IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository, IAdminRepository adminRepository, ITokenService tokenservice, IConfiguration configuration)
        {
            this.employeeRepository = employeeRepository;
            this.suggestionRepository = suggestionRepository;
            this.adminRepository = adminRepository;
            this.tokenservice = tokenservice;
            this.configuration = configuration;
            this.logger = logger;
        }


        //GET: Account/LogIn
        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn(LogInViewModel model)
        {
            logger.LogInformation("Login view accessed on {date}", DateTime.Now);
            ModelState.Clear();
            return View(model);
        }

        //GET: Account/LogOut
        [HttpGet]
        public IActionResult LogOut()
        {
            logger.LogInformation("User with emp_id: {emp_id} logged out of the application on {date}", User.FindFirstValue(ClaimTypes.UserData).ToString(), DateTime.Now);
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn");
        }

        //POST: Account/VerifyUser
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult VerifyUser(LogInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                logger.LogInformation("Login failed on", DateTime.Now);
                return View("LogIn", model);
            }

            //Finnes brukeren som prøver å logge inn
            bool userExists = adminRepository.UserExists(model.emp_id);
            //Hvis ikke brukeren finnes sender vi tilbake feilmelding til brukeren og logger en feilet innlogging. 
            if (!userExists)
            {
                logger.LogInformation("Login failed, non-existent emp_id: {emp_id} on {date}", model.emp_id, DateTime.Now);
                ViewBag.ErrorMessage = $"Finner ikke brukeren med ansattnummer: {model.emp_id}";
                return View("LogIn", new LogInViewModel());
            }

            //Brukeren finnes, vi må sjekke om passordet stemmer overens, derfor preparerer vi en ny EmployeeEntity som vi kan lagre 
            //Informasjon om brukeren i
            EmployeeEntity emp = new EmployeeEntity();

            //Vi må sjekke om det er en av de 10 orginale brukerene som prøver å logge inn fordi de har ikke salt. 
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

            //Hvis vi kommer så langt har ikke systemet funnet en bruker med den komboen av id og passord, dvs at passordet er feil 
            //Fordi brukeren vet vi eksisterer. Sender bruker feilmelding og logger i systemet. 
            if (emp == null)
            {
                ViewBag.ErrorMessage = $"Passordet er  feil for bruker med ansattnummer: {model.emp_id}";
                logger.LogInformation("Login failed, wrong password on {date} by {emp_id}", DateTime.Now, model.emp_id);
                return View("LogIn", new LogInViewModel());
            }
            logger.LogInformation("Login Successful! User with emp_id: {emp_id} logged in on {date}", model.emp_id, DateTime.Now);

            //Hvis vi har kommet så langt må vi finne autorisasjonsrollen til brukeren
            emp.authorizationRole = adminRepository.AuthorizeUser(emp.emp_id);
            //Så må vi genrerer et JWT til brukeren med emp entiteten. 
            generatedToken = tokenservice.BuildToken(configuration["Jwt:Key"].ToString(), configuration["Jwt:Issuer"].ToString(), emp);
            if (generatedToken != null)
            {
                logger.LogInformation("Creation of JWT was succesful on user with emp_id: {emp_id} logged in on {date}", model.emp_id, DateTime.Now);
                HttpContext.Session.SetString("Token", generatedToken);
                return RedirectToAction("Index", "Suggestion");
            }
            //Hvis appen skulle feile å genrere et token av en eller annen grunn blir man sendt tilbake med feilmelding og logget i systemet. 
            else
            {
                logger.LogInformation("Creation of JWT failed on user with emp_id: {emp_id} logged in on {date}", model.emp_id, DateTime.Now);
                ViewBag.ErrorMessage = "Login feilet, vennligst prøv igjen.";
                return View("LogIn", new LogInViewModel());
            }

        }

        //GET: Account/MyAccount
        [HttpGet]
        public IActionResult MyAccount(string sortOrder, string searchString, string filterParameter)
        {
            MyAccountViewModel model = new MyAccountViewModel();
            model.employee = employeeRepository.GetEmployee(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)));
            model.suggestions = suggestionRepository.GetSuggestionsByAuthorID(model.employee.emp_id);
            model.categories = adminRepository.GetAllCategories();
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
                    var sortedDateOld = model.suggestions.OrderByDescending(s => s.timestamp.createdTimestamp.Date).ThenByDescending(s => s.timestamp.createdTimestamp.TimeOfDay);
                    model.suggestions = sortedDateOld.ToList();
                    break;
                case "date_new":
                    var sortedDateNew = model.suggestions.OrderBy(s => s.timestamp.createdTimestamp.Date).ThenByDescending(s => s.timestamp.createdTimestamp.TimeOfDay);
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
            logger.LogInformation(LoggingHelper.PageAccessedLog(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), "My Account"));
            return View(model);
        }

        //GET: Account/MyTeams
        [HttpGet]
        public IActionResult MyTeams()
        {
            MyTeamsViewModel model = new MyTeamsViewModel();
            model.teams = new List<TeamEntity>();
            //Henter den ansatte
            model.employee = employeeRepository.GetEmployee(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)));

            foreach (TeamEntity team in model.employee.teams)
            {
                //Vi trenger mer utfyllende informasjon om teamene så vi må bruke getteam metoden
                model.teams.Add(employeeRepository.GetTeam(team.team_id));
            }

            foreach (TeamEntity team in model.teams)
            {
                //Vi trenger også all informasjon om de ansatte
                foreach (EmployeeEntity emp in team.employees)
                {
                    emp.suggestions = suggestionRepository.GetSuggestionsByAuthorID(emp.emp_id);
                    foreach (SuggestionEntity suggestion in emp.suggestions)
                    {
                        suggestion.author = employeeRepository.GetEmployee(suggestion.author_emp_id);
                        suggestion.responsible_employee = employeeRepository.GetEmployee(suggestion.ownership_emp_id);
                    }
                }
            }
            logger.LogInformation(LoggingHelper.PageAccessedLog(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), "My Teams"));
            return View(model);
        }
    }
}
