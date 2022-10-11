
using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Repositories.Employee;
using bacit_dotnet.MVC.Repositories.Suggestion;
using bacit_dotnet.MVC.Repositories.Team;
using bacit_dotnet.MVC.Repositories.Employee;


namespace bacit_dotnet.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly ITeamRepository teamRepository;
        

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository, ITeamRepository teamRepository)
        {
            _logger = logger;
            this.employeeRepository= employeeRepository;
            this.suggestionRepository = suggestionRepository;
            this.teamRepository = teamRepository;
        }

        public IActionResult Index()
        {
            EmployeeViewModel model = new EmployeeViewModel();
            model.employees = employeeRepository.GetAll();
            foreach(EmployeeEntity emp in model.employees)
            {
                emp.suggestions = suggestionRepository.getByEmployeeID(emp.emp_id);
                emp.teams = teamRepository.Get(emp.emp_id);
            }
            return View(model);
        }


    }
}