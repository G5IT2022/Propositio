using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Repositories.Category;
using bacit_dotnet.MVC.Repositories.Employee;
using bacit_dotnet.MVC.Repositories.Suggestion;
using bacit_dotnet.MVC.Repositories.Team;
using Microsoft.AspNetCore.Mvc;

namespace bacit_dotnet.MVC.Controllers
{
    public class SuggestionController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly ITeamRepository teamRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly ICategoryRepository categoryRepository;

        public SuggestionController(ILogger<HomeController> logger, ISuggestionRepository suggestionRepository, ICategoryRepository categoryRepository, IEmployeeRepository employeeRepository, ITeamRepository teamRepository)
        {
            _logger = logger;
            this.suggestionRepository = suggestionRepository;
            this.categoryRepository = categoryRepository;
            this.employeeRepository = employeeRepository;
            this.teamRepository = teamRepository;
        }

        public IActionResult Index()
        {
            EmployeeViewModel model = new EmployeeViewModel();
            model.employees = employeeRepository.GetAll();
            foreach (EmployeeEntity emp in model.employees)
            {
                emp.suggestions = suggestionRepository.getByEmployeeID(emp.emp_id);
                emp.teams = teamRepository.Get(emp.emp_id);
                foreach(SuggestionEntity suggestion in emp.suggestions)
                {
                    suggestion.categories = categoryRepository.getCategoriesForSuggestion(suggestion.suggestion_id);
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            SuggestionRegisterModel suggestionRegisterModel = new SuggestionRegisterModel();
            suggestionRegisterModel.categories = categoryRepository.getAll();
            return View(suggestionRegisterModel);
        }
        [HttpPost]
        public IActionResult Create(SuggestionRegisterModel model, IFormCollection collection)
        {
            SuggestionEntity suggestion = new SuggestionEntity
            {
                suggestion_id = suggestionRepository.getNewSuggestionID(),
                title = model.title,
                description = model.description,
                status = STATUS.PLAN,
                categories = parseCategories(collection),
                isJustDoIt = model.isJustDoIt,
                ownership_emp_id = 1, 
                poster_emp_id = 1,
                timestamp_id = 1
            };
            suggestionRepository.Add(suggestion);
            return RedirectToAction("Index");
        }

        private List<CategoryEntity> parseCategories(IFormCollection collection)
        {
            List<CategoryEntity> availableCategories = categoryRepository.getAll();
            List <CategoryEntity > categories = new List<CategoryEntity>();
            foreach (var item in collection.Keys)
            {
                foreach(var category in availableCategories)
                {
                    if (category.category_name.Equals(item))
                    {
                        categories.Add(category);
                    }
                }
            }
            return categories;
        }
    }

}
