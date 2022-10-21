using Microsoft.AspNetCore.Mvc;
using bacit_dotnet.MVC.Repositories.Employee;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using bacit_dotnet.MVC.Models.AdminViewModels;
using bacit_dotnet.MVC.Repositories.Team;
using bacit_dotnet.MVC.Repositories.Role;
using bacit_dotnet.MVC.Repositories.Category;
using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Models.Suggestion;

namespace bacit_dotnet.MVC.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ITeamRepository teamRepository;
        private readonly IRoleRepository roleRepository;
        private readonly ICategoryRepository categoryRepository;
        public AdminController(IEmployeeRepository employeeRepository, ITeamRepository teamRepository, IRoleRepository roleRepository, ICategoryRepository categoryRepository)
        {
            this.employeeRepository = employeeRepository;
            this.teamRepository = teamRepository;
            this.roleRepository = roleRepository;
            this.categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            AdminIndexViewModel aivm = new AdminIndexViewModel();
            aivm.employees = new List<EmployeeEntity>();
            aivm.teams = teamRepository.GetAll();
            foreach(TeamEntity team in aivm.teams)
            {
                team.employees = teamRepository.GetEmployeesForTeam(team.team_id);
                foreach(EmployeeEntity emp in team.employees)
                {
                    emp.role = roleRepository.Get(emp.role_id);
                    emp.teams = teamRepository.Get(emp.emp_id);
                    if (!aivm.employees.Contains(emp))
                    {
                    aivm.employees.Add(emp);
                    }
                }
            }
         
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
                result = employeeRepository.Create(newEmp);
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
            aetm.team = teamRepository.GetTeam(id);
            aetm.team.employees = teamRepository.GetEmployeesForTeam(id);
            return View(aetm);
        }

        [HttpPost]
        public IActionResult CreateNewTeam()
        {
           
            return View();
        }
           
    }


}
