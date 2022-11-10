using bacit_dotnet.MVC.Authentication;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using bacit_dotnet.MVC.Models.Suggestion;
using bacit_dotnet.MVC.Repositories;

namespace bacit_dotnet.MVC.Controllers
{


    public class MyTeamController : Controller
    {

        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;


        public MyTeamController(IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository)
        {
            this.employeeRepository = employeeRepository;
            this.suggestionRepository = suggestionRepository;
        }
        public IActionResult Index()
        {
            EmployeeSuggestionViewModel model = new EmployeeSuggestionViewModel();
            model.teams = new List<TeamEntity>();
            model.Employee = employeeRepository.GetEmployee(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)));
            foreach (TeamEntity team in model.Employee.teams)
            {
                model.teams.Add(employeeRepository.GetTeam(team.team_id)); 

            }
            foreach (TeamEntity team in model.teams) {
                foreach (EmployeeEntity employee in team.employees)
                {
                    employee.suggestions = suggestionRepository.GetSuggestionsByAuthorID(employee.emp_id);

                }
                    
            }
            return View(model);
        }
    }
}
