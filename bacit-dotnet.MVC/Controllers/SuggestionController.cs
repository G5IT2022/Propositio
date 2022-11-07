using bacit_dotnet.MVC.Authentication;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;
using bacit_dotnet.MVC.Models.Suggestion;
using bacit_dotnet.MVC.Repositories;
using bacit_dotnet.MVC.Helpers;

namespace bacit_dotnet.MVC.Controllers
{

    [Authorize]
    public class SuggestionController : Controller
    {

        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly IFileRepository fileRepository;


        public SuggestionController(IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository, IFileRepository fileRepository)
        {
            this.suggestionRepository = suggestionRepository;
            this.employeeRepository = employeeRepository;
            this.fileRepository = fileRepository;
        }


        [Authorize]
        public IActionResult Index(string sortOrder, string searchString, string filterParameter)
        {
            //Vi måtte bruke en annen modell enn den vi hadde før fordi vi må filtrere alle forslagene så vi må bruke en modell med kun listen over forslag
            SuggestionViewModel model = new SuggestionViewModel();

            model.suggestions = suggestionRepository.GetAll();
            //Henter kategoriene for filtrering
            model.categories = suggestionRepository.GetAllCategories();
            foreach (SuggestionEntity suggestion in model.suggestions)
            {
                //Setter author og responsible employee entitetene i forslagene til fullverdige employee entiteter slik at man kan vise info om den ansatte
                suggestion.author = employeeRepository.GetEmployee(suggestion.author_emp_id);
                suggestion.responsible_employee = employeeRepository.GetEmployee(suggestion.ownership_emp_id);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                //Sjekker om søkestrengen finnes i tittelen, beskrivelsen eller navnet på forfatter/ansvarlig
                //Måtte lage et midlertidig variabel for å søke med fordi jeg fikk ikke lov til å søke på samlignen som allerede var der
                var searched = model.suggestions.Where(
                   s => s.title.Contains(searchString) ||
                   s.description.Contains(searchString) ||
                   s.author.name.Contains(searchString) ||
                   s.responsible_employee.name.Contains(searchString));
                //Setter listen over forslag i modellen til de man har søkt etter
                model.suggestions = searched.ToList();
            }

            //Filtrering
            //Sjekker først at filteret er satt og at det ikke er en kategori
            if (!string.IsNullOrEmpty(filterParameter))
            {
                var newFilter = filterParameter.Split(" ");

                if (newFilter[0] != "Kategori")
                {
                    //model.suggestions = suggestionRepository.GetSuggestionsByStatus(filterParameter);
                    model.suggestions = FilterHelper.FilterSuggestions(model.suggestions, filterParameter);
                }
                else
                {
                    var CategoryToSend = new CategoryEntity();
                    foreach (CategoryEntity category in model.categories)
                    {
                        if (category.category_name.Equals(newFilter[1]))
                        {
                            CategoryToSend = category;
                        }
                    }
                    model.suggestions = FilterHelper.FilterCategories(model.suggestions, CategoryToSend);
                }
            }

            //Switch statement som sjekker hva du sorterer på, switch er basically en if/elseif/else
            switch (sortOrder)
            {
                case "name_asc":
                    var sortedNameAsc = model.suggestions.OrderBy(s => s.author.name);
                    model.suggestions = sortedNameAsc.ToList();
                    break;
                case "name_desc":
                    var sortedNameDesc = model.suggestions.OrderByDescending(s => s.author.name);
                    model.suggestions = sortedNameDesc.ToList();
                    break;
                case "date_old":
                    var sortedDateOld = model.suggestions.OrderBy(s => s.timestamp.createdTimestamp);
                    model.suggestions = sortedDateOld.ToList();
                    break;
                case "date_new":
                    var sortedDateNew = model.suggestions.OrderBy(s => s.timestamp.createdTimestamp);
                    model.suggestions = sortedDateNew.ToList();
                    break;
                default:
                    break;

            }


            //Hvis man sitter igjen med null forslag på slutten av søket/filtreringen får man en feilmelding
            if (model.suggestions.Count <= 0)
            {
                ViewBag.SortedMessage = "Fant ingen forslag med dine søkekriterier :(";
            }
            return View(model);
        }

        public IActionResult Register()
        {
            SuggestionRegisterModel suggestionRegisterModel = new SuggestionRegisterModel();
            suggestionRegisterModel.categories = suggestionRepository.GetAllCategories();
            suggestionRegisterModel.possibleResponsibleEmployees = employeeRepository.GetEmployeeSelectList();
            return View(suggestionRegisterModel);
        }
        /*
        public void UploadFile(string fileName)
        {
            var fileSavePath = "";
            var uploadedFile = Request.;
            fileName = Path.GetFileName(uploadedFile.FileName);
            fileSavePath = Server.MapPath("~/App_Data/UploadedFiles/" +
              fileName);
            uploadedFile.SaveAs(fileSavePath);
        }*/

        [HttpPost]
        public IActionResult Create(SuggestionRegisterModel model, IFormCollection collection, IFormFile file = null)
        {
            ModelState.Remove("file");
            ModelState.Remove("possibleResponsibleEmployees");
            ModelState.Remove("Categories");
            if (ModelState.IsValid)
            {
                if(file != null)
                {
                 
                }
              
                SuggestionEntity suggestion = new SuggestionEntity
                {
                    title = model.title,
                    description = model.description,
                    status = STATUS.PLAN,
                    categories = parseCategories(collection),
                    ownership_emp_id = Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)),
                    timestamp = new TimestampEntity
                    {
                        dueByTimestamp = model.dueByTimestamp
                    },
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
          
                if(file != null)
                {
                    try
                    {
                        if (fileRepository.UploadFile(file))
                        {
                            ImageEntity imageEntity = new ImageEntity()
                            {
                                image_filepath = file.FileName
                            };
                            suggestion.images.Add(imageEntity);
                            ViewBag.Message = "File Upload Successful";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log ex
                        ViewBag.Message = "File Upload Failed";
                    }
                }
                suggestionRepository.CreateSuggestion(suggestion);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Register");
            }
        }
        /**
         * Denne private metoden gjør at:
         * 1. du kan hente list av katergorier
         * 2. du kan legge til kategorier som du velger fra checkbox til forslaget.
         * @Paramter collection - en samling av kategorier
         * @Return utvaglte kategorier listen
         */
        private List<CategoryEntity> parseCategories(IFormCollection collection)
        {
            List<CategoryEntity> availableCategories = suggestionRepository.GetAllCategories();
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

        /**
         * Dette er en metode for å hente info til ett forslag og alle kommentarer som tilhører til forslaget.
         * @Parameter suggestion_id
         * @Return informasjon av et forslag i Details Viewet
         */
        public IActionResult Details(int id)
        {
            SuggestionDetailsModel detailsModel = new SuggestionDetailsModel();
            detailsModel.suggestion = suggestionRepository.GetSuggestionBySuggestionIDWithCommentsAndImages(id);
            detailsModel.employee = employeeRepository.GetEmployee(detailsModel.suggestion.author_emp_id);
            detailsModel.suggestion.author = employeeRepository.GetEmployee(detailsModel.suggestion.author_emp_id);
            detailsModel.suggestion.responsible_employee = employeeRepository.GetEmployee(detailsModel.suggestion.ownership_emp_id);
            foreach (CommentEntity comment in detailsModel.suggestion.comments)
            {
                if (comment != null)
                {
                    comment.poster = employeeRepository.GetEmployee(comment.emp_id);
                }
            }


            if (detailsModel.suggestion == null)
            {
                return RedirectToAction("Index");
            }
            return View(detailsModel);
        }

        //Suggestion/Details/suggestion_id
        [HttpPost]
        public IActionResult CreateComment(SuggestionDetailsModel model, IFormCollection collections)
        {
            CommentEntity comment = new CommentEntity
            {
                description = model.description,
                suggestion_id = Int32.Parse(collections["suggestion_id"]),
                emp_id = Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)),
                createdTimestamp = DateTime.Now,

            };
            int result = suggestionRepository.CreateComment(comment);

            if (result != 1)
            {
                ViewBag.Message = "Noe gikk galt, prøv igjen";
                return RedirectToAction("Details", "Suggestion", new { id = comment.suggestion_id });
            }
            else
            {
                ViewBag.Message = "Kommentaren din ble postet!";
                return RedirectToAction("Details", "Suggestion", new { id = comment.suggestion_id });
            }
        }
        /**
         * Denne metoden gjør at du kan slette kommentarer i forslaget
         * @Parameter comment_id og suggestion id
         * @Return Suggestion/Details/suggestion_id
         */
        public IActionResult DeleteComment(int comment_id, int suggestion_id)
        {
            var result = suggestionRepository.DeleteComment(comment_id);
            return RedirectToAction("Details", "Suggestion", new { id = suggestion_id });

        }
        //Favoritter
        [HttpPost]
        public void Favorite(int id)
        {
            SuggestionEntity suggestion = suggestionRepository.GetSuggestionBySuggestionID(id);
            suggestion.favorite = !suggestion.favorite;
            suggestionRepository.Favorite(id, suggestion.favorite);
        }
    }
}

