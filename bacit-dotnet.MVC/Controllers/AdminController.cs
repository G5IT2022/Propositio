using Microsoft.AspNetCore.Mvc;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using bacit_dotnet.MVC.Models.AdminViewModels;
using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Models.Suggestion;
using System.Security.Cryptography.X509Certificates;
using bacit_dotnet.MVC.Repositories;

namespace bacit_dotnet.MVC.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        public AdminController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            AdminIndexViewModel aivm = new AdminIndexViewModel();
            aivm.employees = new List<EmployeeEntity>();
            aivm.teams = employeeRepository.GetTeams();         
            return View(aivm);
        }
        [HttpGet]
        public IActionResult NewUser()
        {
            ViewBag.Message = "Registrer ny ansatt";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewUser(AdminNewUserModel model, IFormCollection coll)
        {
            int result = 0;
            if (ModelState.IsValid)
            {
                EmployeeEntity newEmp = new EmployeeEntity
                {
                    emp_id = model.emp_id,
                    name = model.first_name + " " + model.last_name,
                    salt = PassHash.GenerateSalt(),
                    passwordhash = model.password,
                    authorization_role_id = 1,
                    role_id = 1
                };
                var tmp = PassHash.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(newEmp.passwordhash), newEmp.salt);
                newEmp.passwordhash = Convert.ToBase64String(tmp);
                result = employeeRepository.CreateEmployee(newEmp);
            }
            if(result != 1)
            {
                ViewBag.Created = "Noe gikk galt, prøv igjen.";
            }
            else
            {
                ViewBag.Created = $"Ansatt {model.first_name + " " + model.last_name} ble opprettet!";
            }
            return View("NewUser");

        }

        [HttpGet]
        public IActionResult EditTeam(int id)
        {
            AdminEditTeamModel aetm = new AdminEditTeamModel();
            aetm.team = employeeRepository.GetTeam(id);
            return View(aetm);
        }

        [HttpGet]
        public IActionResult CreateNewTeam()
        {
            AdminNewTeamModel adminNewTeamModel = new AdminNewTeamModel();       
            return View(adminNewTeamModel);
        }      
        

        [HttpGet]
        public IActionResult AddTeamMember()
        {
            TeamMemberModel memberModel = new TeamMemberModel();
            memberModel.employees = employeeRepository.GetEmployees();
            memberModel.teams = employeeRepository.GetTeams();
            return View(memberModel);     
        }
        
    }


}
