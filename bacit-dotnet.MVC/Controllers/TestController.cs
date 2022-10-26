using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.Test;
using bacit_dotnet.MVC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bacit_dotnet.MVC.Controllers
{
    public class TestController : Controller
    { 
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        public TestController(IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository)
        {
            this.employeeRepository = employeeRepository;
            this.suggestionRepository = suggestionRepository;
        }
        // GET: TestController
        public ActionResult Index()
        {
            TestViewModel model = new TestViewModel();
            model.starEmployee = employeeRepository.GetEmployee(2);
            model.employees = employeeRepository.GetEmployees();
            model.starTeam = employeeRepository.GetTeam(1);
            model.teams = employeeRepository.GetTeams();
            // model.starSuggestion = suggestionRepository.GetSuggestionBySuggestionID(1);
            // model.starSuggestion.author = employeeRepository.GetEmployee(model.starSuggestion.author_emp_id);
            model.suggestions = suggestionRepository.GetAll();
            return View(model);
        }

        // POST: TestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CategoryEntity cateogry)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
