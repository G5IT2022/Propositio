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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly IAdminRepository adminRepository;
        public AdminController(IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository, IAdminRepository adminRepository)
        {
            this.employeeRepository = employeeRepository;
            this.suggestionRepository = suggestionRepository;
            this.adminRepository = adminRepository;
            this.adminRepository = adminRepository; 
        }
        //Get: /Admin/Index
        [HttpGet]
        public IActionResult Index(string sortOrder, string searchString)
        {
            AdminIndexViewModel aivm = new AdminIndexViewModel();
            aivm.employees = employeeRepository.GetEmployees();
            //aivm.teams = employeeRepository.GetTeams();
            //foreach (TeamEntity team in aivm.teams)
            //{
            //    team.teamleader = employeeRepository.GetEmployee(team.team_lead_id);

            //}
            aivm.categories = suggestionRepository.GetAllCategories();
            aivm.roles = adminRepository.GetAllRoles();
            AdminIndexViewModel model = new AdminIndexViewModel();
            model.employees = employeeRepository.GetEmployees();

            if (!String.IsNullOrEmpty(searchString))
            {
                //Søker på navnet/rollen til den ansatte
                var searched = model.employees.Where(
                   e => e.name.Contains(searchString) ||
                   e.role.role_name.Contains(searchString));
                //Setter listen med ansatte til det brukeren har søkt etter
                model.employees = searched.ToList();
            }
            //Switch statement som sjekker hva du sorterer på, switch er basically en if/elseif/else
            switch (sortOrder)
            {
                case "name_asc":
                    var sortedNameAsc = model.employees.OrderBy(e => e.name);
                    model.employees = sortedNameAsc.ToList();
                    break;
                case "name_desc":
                    var sortedNameDesc = model.employees.OrderByDescending(e => e.name);
                    model.employees = sortedNameDesc.ToList();
                    break;
                default:
                    break;
            }
            model.teams = employeeRepository.GetTeams();
            foreach (TeamEntity team in model.teams)
            {
                team.teamleader = employeeRepository.GetEmployee(team.team_lead_id);

            }
            model.categories = suggestionRepository.GetAllCategories();
            model.roles = adminRepository.GetAllRoles();

            return View(model);
        }
      
        //Get: /Admin/newUser
        [HttpGet]
        public IActionResult NewUser(AdminNewUserModel model)
        {
            ViewBag.Message = "Registrer ny ansatt";
            model.possibleRoles = adminRepository.GetRoleSelectList();
            ModelState.Clear();
            return View(model);
        }


        /**
         * Denne metoden er for å registrere en ny bruker
         * @Parameter AdminNewUserModel og en Collection
         * @Return Admin/Index
         */
        //Post: /Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(AdminNewUserModel model)
        {
            AdminNewUserModel newModel = new AdminNewUserModel();
            newModel.possibleRoles = adminRepository.GetRoleSelectList();
            ModelState.Remove("possibleRoles");
            if (ModelState.IsValid)
            {
                if (!adminRepository.UserExists(model.emp_id))
                {
                    var salt = PassHash.GenerateSalt();
                    EmployeeEntity newEmp = new EmployeeEntity
                    {
                        emp_id = model.emp_id,
                        name = model.first_name + " " + model.last_name,
                        salt = salt,
                        passwordhash = Convert.ToBase64String(PassHash.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(model.password), salt)),
                        authorization_role_id = model.isAdmin ? 2 : 1,
                        role_id = model.role_id
                    };
                    ViewBag.Error = $"Ansatt {model.first_name + " " + model.last_name} ble opprettet!";
                    adminRepository.CreateEmployee(newEmp);
                }
                else
                {
                    ViewBag.Error = $"Brukeren med ansattnummer {model.emp_id} finnes allerede!";
                }
            }
            return View("NewUser", newModel);
        }

        //Get: /admin/edituser/emp_id
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            AdminEditUserModel aeum = new AdminEditUserModel();
            aeum.possibleRoles = adminRepository.GetRoleSelectList();
            aeum.user = employeeRepository.GetEmployee(id);
            return View(aeum);
        }


        [HttpPost]
        public IActionResult UpdateUser(EmployeeEntity emp)
        {

            if (ModelState.IsValid)
            {
                adminRepository.UpdateEmployee(new EmployeeEntity
                {
                    name = emp.name,
                    passwordhash = emp.passwordhash,
                    role_id = emp.role_id
                });
                return RedirectToAction("Index", "Admin", new { id = emp.emp_id });
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { id = emp.emp_id });
            }
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

        //Get: /admin/editteam/team_id
        [HttpGet]
        public IActionResult EditTeam(int id)
        {
            AdminEditTeamModel adminEditTeamModel= new AdminEditTeamModel();
            adminEditTeamModel.team = employeeRepository.GetTeam(id);
            //Henter alle ansatte i listen fra databasen for employeeList
            var employeeList = employeeRepository.GetEmployeeSelectList();
            //selectListEmployees (i selectBox) inneholder ansatte som ikke inkludere de eksisterende ansatte i teamet
            adminEditTeamModel.selectListEmployees = returnEmployeeNotInTeam(adminEditTeamModel.team, employeeList);
            //Dette betyr at du kan hente den nående teamlederen av teamet, og sette den teamlederen å vises først i "velg teamleder dropdownlist"
            adminEditTeamModel.team_lead_id = adminEditTeamModel.team.team_lead_id;
            adminEditTeamModel.selectListForTeamLeader = new SelectList(employeeList, "Value", "Text", adminEditTeamModel.team.team_lead_id.ToString());
            
            return View(adminEditTeamModel);
        }
        public List<SelectListItem> returnEmployeeNotInTeam(TeamEntity team, List<SelectListItem> employeeList)
        {
            //Henter en liste av emp_id som eksisterer i teamet
            var employeeIDs = team.employees.Select(e => e.emp_id.ToString()).ToArray();
            //Henter alle ansatte for employeeList, men ikke inkluderer de eksisterende ansatte i teamet fra employeeIDs
            var result = employeeList.Where(e => !employeeIDs.Contains(e.Value.ToString())).ToList();
            return result;
        }

        [HttpPost]
        public IActionResult EditTeam(AdminEditTeamModel model)
        {
            TeamEntity team = new TeamEntity()
            {
                team_id = model.team_id,
                team_lead_id = model.team_lead_id,
                team_name = model.team.team_name,
                employees = new List<EmployeeEntity>()

            };

            if (model.selectedMemberTeamIDs != null)
            {
                foreach (int i in model.selectedMemberTeamIDs)
                {
                    var emp = employeeRepository.GetEmployee(i);
                    team.employees.Add(employeeRepository.GetEmployee(i));
                }
            }

            int result = adminRepository.UpdateTeam(team);
            if (result == 0)
            {
                ViewBag.Message = "";
            }
            return RedirectToAction("EditTeam", new { id = team.team_id });
        }
        public IActionResult DeleteTeamMember(int emp_id, int team_id)
        {
            var result = adminRepository.DeleteTeamMember(emp_id);
            return RedirectToAction("EditTeam", "Admin", new { id = team_id });
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


        [HttpPost]
        public IActionResult CreateCategory(string category_name)
        {
            adminRepository.CreateNewCategory(category_name);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteCategory(int category_id)
        {
            adminRepository.DeleteCategory(category_id);
            return RedirectToAction("Index");
        }

        /**
         * Denne metoden gjør at du kan legge til en ny rolle i databasen.
         * @Parameter AdminIndexViewModel
         * @Return Admin/Index - nye roller skal bli lagt til rollelisten.
         */
        [HttpPost]
        public IActionResult CreateNewRole(AdminIndexViewModel model)
        {
            AdminIndexViewModel aivm = new AdminIndexViewModel();
            aivm.employees = employeeRepository.GetEmployees();
            aivm.teams = employeeRepository.GetTeams();
            aivm.categories = suggestionRepository.GetAllCategories();
            aivm.roles = adminRepository.GetAllRoles();

            ModelState.Remove("role_id");
            //Første sjekker dersom rollen har eksistert i databasen
            //Return fale hvis den nye rollen ikke har eksistert i databasen
            //Return true når rollen har eksistert i databasen
            if (ModelState.IsValid)
            {
                var roles = adminRepository.GetAllRoles();
                var roleExists = false;                             

                foreach (RoleEntity role in roles)
                {
                    if (role.role_name.ToLower().Equals(model.role_name.ToLower()))
                    {
                        roleExists = true;
                    }
                }
              
                {
                    if (roleExists)
                    {                      
                        ViewBag.ErrorMessage = "Rollen eksisterer allerede i databasen.";
                        return View("Index", aivm);
                    }
                    else
                    {
                        RoleEntity role = new RoleEntity
                        {
                            role_name = model.role_name,
                        };
                        adminRepository.CreateNewRole(role);
                        ViewBag.ErrorMessage = "Rollen ble opprettet.";
                        aivm.roles = adminRepository.GetAllRoles();
                    }
                }
            }
            return View("Index", aivm);
        }

        /**
         * Denne metoden er for å slette rolle
         * @Parameter role_id 
         * @Return Admin/Index side
         */
        public IActionResult DeleteRole(int role_id)
        {
            var result = adminRepository.DeleteRole(role_id);
            return RedirectToAction("Index");
        }      
       
        
    }
}
