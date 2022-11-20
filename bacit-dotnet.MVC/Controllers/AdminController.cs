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
            AdminIndexViewModel aivm = prepareAdminIndexViewModel();            
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

        /**
         * Denne private metoden er brukt for å lage en ny model av AdminIndexViewModel
         * slik at du kan hente alle Employees, Teams, Categories, Roles
         * @Return model
         */
        private AdminIndexViewModel prepareAdminIndexViewModel()
        {
            AdminIndexViewModel model = new AdminIndexViewModel();
            model.employees = employeeRepository.GetEmployees();
            model.teams = employeeRepository.GetTeams();
            foreach (TeamEntity team in model.teams)
            {
                team.teamleader = employeeRepository.GetEmployee(team.team_lead_id);
            }
            model.categories = suggestionRepository.GetAllCategories();
            model.roles = adminRepository.GetAllRoles();

            return model;
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
        /**
         * Denne metoden er for å oppdatere informasjon av en ansatt
         * @Parameter EmployeeEntity
         * @Return Admin/Index Den ansatte blir oppdatert.
         */
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
        
        /**
         * Denne metoden er for å hente CreateNewTeam Viewet.
         * @Return CreateNewTeam Viewet
         */
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
            //Get ansatteliste for å velge teamleder
            memberModel.selectTeamleader = temp.Select(ld => new SelectListItem { Value = ld.emp_id.ToString(), Text = ld.name }).ToList();
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
            //Det nye team_name skal bli sjekket gjennom GetTeamByName metoden i EmployeeRepository          
            //Sjekker dersom du bruker samme teamnavn som allerede har eksistert i databasen, får du ErrorMessage om dette
            //Returnerer GetMembers private metoden. NB! Du trenger ikke å velge ansatte og teamleder på nytt,
            //fordi det returnerer GetMembers metoden som returnerer AddTeamMemberModel.
            if (team != null)
            {
                ViewBag.ErrorMessage = $"{model.team_name} har brukt. Vennligst prøv et nytt navn!";
                return View(GetMembers());
            }
            //Sjekker dersom ansatte har valgt fra selectbox
            if (!model.selectEmployeesForNewTeam.Any(a => a.selected))
            {
                ViewBag.ErrorMessage = $"Vennligst, velg ansatte!";
                return View(GetMembers());
            }
            //Sjekker hvis det nye teamnavnet ikke har blitt brukt, blir det godkjent, samt generer det et nytt team_id og binding det nye teamnavnet og team_lead_id til AdminNewTeamModel.
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
        /**
         * Denne metoden gjør at du kan redigere teamnavn, legge til medlemmer og endre teamleder.
         * @Parameter AdminEditTeamModel
         * @Return Admin/Editteam teamet blir oppdatert. 
         */
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
            if (result != 1)
            {
                ViewBag.Message = $"{model.team.team_name} har blitt oppdatert!";
            }
            //ViewBag.Message = $"abc";
            return RedirectToAction("EditTeam", new { id = team.team_id });
        }
        /**
         * Denne metoden er for å slette ett eksisterende medlem i teamet
         * @Parameter emp_id, team_id
         * @Return Admin/Editteam - det medlemmet blir slettet.
         */
        public IActionResult DeleteTeamMember(int emp_id, int team_id)
        {
            var employee = employeeRepository.GetEmployee(emp_id);

            //Hvis den ansatte er medlem i flere team, slett fra teamet
            if(employee.teams.Count > 1)
            {
                var result = adminRepository.DeleteTeamMember(emp_id, team_id);               
            }
            else
            {
                //Hvis ikke må vi sørge for at den ansatte er med i minst ett team så vi legger de til i "Uten Team" teamet
                employeeRepository.InsertMemberToTeam(1, emp_id);
                adminRepository.DeleteTeamMember(emp_id, team_id);
            }
            return RedirectToAction("EditTeam", "Admin", new { id = team_id });
        }

        /**
         * Denne metoden er for å slette et team basert på team_id
         * @Parameter team_id
         * @Return Admin/Index - det teamet blir slettet.
         */
        public IActionResult DeleteTeam(int team_id)
        {
            var result = employeeRepository.DeleteTeam(team_id);
            return RedirectToAction("Index");

        }

        /**
         * Denne metoden er for å lage en ny kategori
         * @Parameter category_name
         * @Return  Admin/Index en ny kategori blir opprettet i databasen.
         */
        [HttpPost]
        public IActionResult CreateCategory(string category_name)
        {
            adminRepository.CreateNewCategory(category_name);
            return RedirectToAction("Index");
        }

        /**
         * Denne metoden er for å slette en kategori
         * @Parameter category_id
         * @Return Admin/Index den kategorien blir slettet.
         */
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
            AdminIndexViewModel adminIndexViewModel = prepareAdminIndexViewModel();
            //adminIndexViewModel.employees = employeeRepository.GetEmployees();
            //adminIndexViewModel.teams = employeeRepository.GetTeams();
            //adminIndexViewModel.categories = suggestionRepository.GetAllCategories();
            //adminIndexViewModel.roles = adminRepository.GetAllRoles();

            //Sjekker dersom det nye rollenavnet eksisterte allerede i databasen
            var role = adminRepository.GetRoleByName(model.role_name.ToLower());
            if (role!= null)
            {
                ViewBag.ErrorMessage = $"{model.role_name} eksisterer allerede i databasen. Vennligst, prøv å legge til en ny rolle!";
                return View("Index", adminIndexViewModel);
            }    
            //Hvis det er en ny rolle og ikke har eksistert i database, blir en ny rolle laget i databasen.
            else
            {
                
                role = adminRepository.CreateNewRole(new AdminIndexViewModel
                {
                   
                    role_name = model.role_name,
                   
                });
                ViewBag.ErrorMessage = @"Rollen blir opprettet!";

            }
            return RedirectToAction("Index", new { role.role_id });
        }
       
        /**
         * Denne metoden er for å slette rolle
         * @Parameter role_id 
         * @Return Admin/Index - den rollen blir slettet.
         */
        public IActionResult DeleteRole(int role_id)
        {
            var result = adminRepository.DeleteRole(role_id);
            return RedirectToAction("Index");
        }            
     
    }
}
