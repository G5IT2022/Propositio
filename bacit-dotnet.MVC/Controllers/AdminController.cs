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
using Microsoft.AspNetCore.Mvc.TagHelpers;
using MySqlX.XDevAPI;
using bacit_dotnet.MVC.Models.AdminViewModels.TeamModels;

namespace bacit_dotnet.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        public AdminController(IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository)
        {
            this.employeeRepository = employeeRepository;
            this.suggestionRepository = suggestionRepository;
        }

        public IActionResult Index()
        {
            AdminIndexViewModel aivm = new AdminIndexViewModel();
            aivm.employees = employeeRepository.GetEmployees();
            aivm.teams = employeeRepository.GetTeams();
            aivm.categories = suggestionRepository.GetAllCategories();
            aivm.roles = employeeRepository.GetAllRoles();

            return View(aivm);
        }
        //Get: /Admin/newUser
        [HttpGet]
        public IActionResult NewUser()
        {
            ViewBag.Message = "Registrer ny ansatt";
            return View();
        }

        //registrere ny bruker
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
            if (result != 1)
            {
                ViewBag.Created = "Noe gikk galt, prøv igjen.";
            }
            else
            {
                ViewBag.Created = $"Ansatt {model.first_name + " " + model.last_name} ble opprettet!";
            }
            return View("NewUser");

        }
        //Get: /admin/editteam/team_id
        [HttpGet]
        public IActionResult EditTeam(int id)
        {
            AdminEditTeamModel aetm = new AdminEditTeamModel();
            aetm.team = employeeRepository.GetTeam(id);
            return View(aetm);
        }
        
        //Get: Admin/CreateNewTeam
        [HttpGet]
        public IActionResult CreateNewTeam()
        {           
            return View(GetMembers());
        }

        /**Denne private metoden gjør at:
         * 1. Ansattlisten genereres for AddTeamMemberModel i CreateNewTeam Viewet.
         * 2. I AddTeamMemberModel finnes det en list selectEmployeesForNewTeamModel. Det blir brukt til å lagre alle ansatte som du velger fra checkbox.
         * 3. Når du har valg ansatte og teamleder fra checkbox, samt du bruker samme teamnavn som allerede eksistert i database, returnerer det tilbake til AddTeamMemberModel
         * men alle ansatte og teamleder som du har valgt, er de ikke forsvunnet. Det vil si at du ikke trenger velge ansatte og teamleder på nytt. 
         * NB! Admin kan ikke bruke samme teamnavn.
         * @Return AddTeamMemberModel
         */        
        private AddTeamMemberModel GetMembers()
        {
            AddTeamMemberModel memberModel = new AddTeamMemberModel();
            memberModel.selectEmployeesForNewTeam = new List<SelectEmployeesForNewTeamModel>();
            var temp = employeeRepository.GetEmployees();

            foreach (var emp in temp)
            {
                memberModel.selectEmployeesForNewTeam.Add(new SelectEmployeesForNewTeamModel()
                {
                    emp_id = emp.emp_id,
                    employee = emp,
                    selected = false
                });
            }
            return memberModel;
        }

        /**
         * Denne metoden gjør at teamnavn og alle utvalgte ansatte + teamleder blir lagret i den nye listen selectEmployeesForNewTeam i AddTeamMemberModel.
         * @Parameter AddTeamMemberModel
         * @Return Admin/Index Viewet etter at du har trykket på Lagre knappen.
         */
        [HttpPost]
        public IActionResult CreateNewTeam(AddTeamMemberModel model)
        {           
            //Sjekk dersom Teamnavn allerede har eksistert i databasen
            var team = employeeRepository.GetTeamByName(model.team_name);
            //model henter team_name, team_lead_id, for i Team tabellen har vi både team_name og team_lead_id.
            //Det nye team_name skal bli sjekket gjennom GetTeamByName metoden i EmployeeRepository:             
            //1. Hvis det nye teamnavnet ikke har blitt brukt, blir det godkjent, samt generer det et nytt team_id og binding det nye teamnavnet og team_lead_id til AdminNewTeamModel.
            if (team == null)
            {
                team = employeeRepository.CreateNewTeam(new AdminNewTeamModel
                {
                    team_name = model.team_name,
                    team_lead_id = model.team_lead_id,
                });
               
                //Henter listen av utvalgte teamleder og alle ansatte fra checkbox                               
                var teamMembers = model.selectEmployeesForNewTeam.Where(a => a.selected);
                //new selected employees are inserted into TeamList Table
                //Utvalgte teamleder og alle utvalgte ansatte fra checkbox blir lagt til listen selectEmployeesForNewTeam som binding med AddTeamMember model
                foreach (var teamMember in teamMembers)
                {
                    employeeRepository.InsertMemberToTeam(team.team_id, teamMember.emp_id);
                }
            }
            //2. Dersom du bruker samme teamnavn som allerede har eksistert i databasen, får du ErrorMessage om dette
            //Returnerer GetMembers orivate metoden. NB! Du trenger ikke å velge ansatte og teamleder på nytt,
            //fordi det returnerer GetMembers metoden som returnerer AddTeamMemberModel.
            else
            {
                ViewBag.ErrorMessage = $"{model.team_name} har brukt. Vennligst prøv et nytt navn!";
                return View(GetMembers());
            }            
            return RedirectToAction("Index");
        }
        /**
         * Slett Team
         * @Parameter team_id
         * @Return Admin/Index
         */
        public IActionResult DeleteTeam(int team_id)
        {
            var result = employeeRepository.DeleteTeam(team_id);
            return RedirectToAction("Index");

        }

        //[HttpGet]
        //public IActionResult AddTeamMember()
        //{
        //    AddTeamMemberModel memberModel = new AddTeamMemberModel();

        //    memberModel.selectEmployeesForNewTeam = new List<SelectEmployeesForNewTeamModel>();
        //    var temp = employeeRepository.GetEmployees();
        //    foreach (var emp in temp)
        //    {
        //        memberModel.selectEmployeesForNewTeam.Add(new SelectEmployeesForNewTeamModel() { employee = emp, selected = false });
        //    }
        //    //memberModel.selectEmployees = employeeRepository.GetEmployees();            

        //    return View(memberModel);
        //}

        //[HttpPost]
        //public IActionResult AddTeamMember(List<SelectEmployeesForNewTeamModel> selectEmployees)
        //{
        //    //ienumerable<selectemployeesfornewteammodel> selectemployees = new list<selectemployeesfornewteammodel>();
        //    //list<employeeentity> employees = new list<employeeentity>();
        //    //foreach (var employee in selectemployees)
        //    //{
        //    //    if (employee.ischecked == true)
        //    //    {
        //    //        employees.add(employee.employee);
        //    //    }

        //    //}

        //    AdminNewTeamModel model = new AdminNewTeamModel();

        //    return RedirectToAction("CreateNewTeam");
        //}

    }
}
