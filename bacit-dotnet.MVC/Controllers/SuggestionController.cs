using bacit_dotnet.MVC.Authentication;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Repositories.Employee;
using bacit_dotnet.MVC.Repositories.Suggestion;
using bacit_dotnet.MVC.Repositories.Team;
using bacit_dotnet.MVC.Repositories.Category;
using bacit_dotnet.MVC.Repositories.Timestamp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using bacit_dotnet.MVC.Models.Suggestion;
using bacit_dotnet.MVC.Repositories.Comment;
using System.Drawing.Text;

namespace bacit_dotnet.MVC.Controllers
{

    [Authorize]
    public class SuggestionController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly ITeamRepository teamRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ITimestampRepository timestampRepository;
        private readonly ICommentRepository commentRepository;
        private readonly ITokenService tokenservice;
        private readonly IConfiguration configuration;
        public SuggestionController(ILogger<HomeController> logger, ISuggestionRepository suggestionRepository, ICategoryRepository categoryRepository,
            IEmployeeRepository employeeRepository, ITeamRepository teamRepository, ITimestampRepository timestampRepository, ICommentRepository commentRepository,
            ITokenService tokenService, IConfiguration configuration)
        {
            _logger = logger;
            this.suggestionRepository = suggestionRepository;
            this.categoryRepository = categoryRepository;
            this.employeeRepository = employeeRepository;
            this.teamRepository = teamRepository;
            this.commentRepository = commentRepository;
            this.tokenservice = tokenService;
            this.timestampRepository = timestampRepository;
            this.configuration = configuration;

        }
        [Authorize]
        public IActionResult Index()
        {
           Console.WriteLine(HttpContext.User.FindFirst(ClaimTypes.Role).ToString().Split(" ")[1]);
            EmployeeSuggestionViewModel model = new EmployeeSuggestionViewModel();
            model.employees = employeeRepository.GetAll();
            foreach (EmployeeEntity emp in model.employees)
            {
                emp.suggestions = suggestionRepository.GetByEmployeeID(emp.emp_id);
                emp.teams = teamRepository.Get(emp.emp_id);
                foreach (SuggestionEntity suggestion in emp.suggestions)
                {
                    suggestion.categories = categoryRepository.GetCategoriesForSuggestion(suggestion.suggestion_id);
                    suggestion.timestamp = timestampRepository.Get(suggestion.suggestion_id);


                }
            }

            return View(model);

        }

        public IActionResult Register()
        {
            SuggestionRegisterModel suggestionRegisterModel = new SuggestionRegisterModel();
            suggestionRegisterModel.categories = categoryRepository.GetAll();
            return View(suggestionRegisterModel);
        }
        [HttpPost]
        public IActionResult Create(SuggestionRegisterModel model, IFormCollection collection)
        {
            SuggestionEntity suggestion = new SuggestionEntity
            {
                suggestion_id = suggestionRepository.GetNewSuggestionID(),
                title = model.title,
                description = model.description,
                status = STATUS.PLAN,
                categories = parseCategories(collection),
                ownership_emp_id = Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)),
                author_emp_id = Int32.Parse(User.FindFirstValue(ClaimTypes.UserData))
            };
            if (model.isJustDoIt == true)
            {
                suggestion.status = STATUS.JUSTDOIT;
            }
            else
            {
                suggestion.status = STATUS.PLAN;
            }
            suggestionRepository.Create(suggestion);
            timestampRepository.Create(suggestion.suggestion_id, model.dueByTimestamp);
            return RedirectToAction("Index");
        }

        private List<CategoryEntity> parseCategories(IFormCollection collection)
        {
            List<CategoryEntity> availableCategories = categoryRepository.GetAll();
            List<CategoryEntity> categories = new List<CategoryEntity>();
            foreach (var item in collection.Keys)
            {
                foreach (var category in availableCategories)
                {
                    if (category.category_name.Equals(item))
                    {
                        categories.Add(category);
                    }
                }
            }
            return categories;
        }

        //Dette er en metode for å hente info til ett forslag (1) og alle kommentarer som tilhører til forslaget (2)
        public IActionResult Details(int id)
        {
            SuggestionDetailsModel detailsModel = new SuggestionDetailsModel();
            //Vi setter den categories listen med categoryRepository hvor det finnes query select *
            //og metode Get som skal hente kategorier for et forslag fra suggestion_id 
            //Hente info til ett forslag
            detailsModel.suggestion = suggestionRepository.GetById(id);
            detailsModel.employee = employeeRepository.Get(detailsModel.suggestion.author_emp_id);
            detailsModel.employee.teams = teamRepository.Get(detailsModel.employee.emp_id);
            detailsModel.suggestion.categories = categoryRepository.GetCategoriesForSuggestion(detailsModel.suggestion.suggestion_id);
            detailsModel.suggestion.timestamp = timestampRepository.Get(detailsModel.suggestion.suggestion_id);


            //Hente alle kommentarer som tilhører til forslaget fra databasen            
            detailsModel.comment = commentRepository.Get(detailsModel.suggestion.suggestion_id);
            detailsModel.comment.poster = employeeRepository.Get(detailsModel.comment.emp_id);

            detailsModel.suggestion.comments = commentRepository.GetCommentsForSuggestion(detailsModel.suggestion.suggestion_id);
            //Dette er en foreach loop som brukes for å hente dato og hvem som skriver kommentar for ett forslag
            //List kommentar
            foreach (CommentEntity comment in detailsModel.suggestion.comments)
            {
                comment.poster = employeeRepository.Get(comment.emp_id);

            }
            if (detailsModel.suggestion == null)
            {
                return RedirectToAction("Index");
            }
            return View(detailsModel);
        }

        //Lage en ny kommentar
        [HttpPost]
        public IActionResult CreateComment(SuggestionDetailsModel model, IFormCollection collections)
        {
            CommentEntity comment = new CommentEntity
            {
                comment_id = commentRepository.GetNewCommentID(),
                description = model.description,
                suggestion_id = Int32.Parse(collections["suggestion_id"]),
                emp_id = Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)),
                createdTimestamp = DateTime.Now,

            };
            commentRepository.Create(comment);
            int result = commentRepository.Create(comment);

            if (result != 1)
            {
                //Noe har gått feil med å lage kommentaren         

            }
            return RedirectToAction("Details", "Suggestion", new { id = comment.suggestion_id });
        }
        //Favoritter
        [HttpPost]
        public void Favorite(int id)
        {
            SuggestionEntity suggestion = suggestionRepository.GetById(id);
            suggestion.favorite = !suggestion.favorite;
            suggestionRepository.Favorite(id, suggestion.favorite);
        }
    }
}

