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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Web.WebPages.Html;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Update.Internal;

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

        //Get: /Suggestion/Index
        [HttpGet]
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
                    var sortedDateOld = model.suggestions.OrderByDescending(s => s.timestamp.createdTimestamp.Date).ThenByDescending(s => s.timestamp.createdTimestamp.TimeOfDay);
                    model.suggestions = sortedDateOld.ToList();
                    break;
                case "date_new":
                    var sortedDateNew = model.suggestions.OrderBy(s => s.timestamp.createdTimestamp.Date).ThenByDescending(s => s.timestamp.createdTimestamp.TimeOfDay);
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

        //Get: /Suggestion/Register
        [HttpGet]
        public IActionResult Register(SuggestionRegisterModel model)
        {
            if (model.possibleResponsibleEmployees == null)
            {
               model = prepareSuggestionRegisterModel();
               ModelState.Clear();
            }
            return View(model);
        }

        //Post: /Suggestion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSuggestion(SuggestionEntity model, IFormCollection collection, IFormFile file = null)
        {

            //Klargjør en ny modell til etter verifisering/registrering, man kan ikke bare returnere et view uten modell. 
            SuggestionRegisterModel newModel = prepareSuggestionRegisterModel();

            //Vi må fjerne noen ting fra modellen fordi de er irrelevante for å lage et forslag
            //timestamp og fil blir verifisert utenom
            ModelState.Remove("author");
            ModelState.Remove("responsible_employee");
            ModelState.Remove("file");
            ModelState.Remove("timestamp");

            //Hvis modellen ikke er gyldig returner feilmelding, denne sjekken er her på starten så vi slipper å gjøre en hel del andre sjekker først. 
            if (!ModelState.IsValid)
            {
                return View("Register", newModel);
            }

            //Sett statusen på forslaget basert på om justdoit er true eller false, hvis man ikke har .Contains("true") så fungerer det ikke av en eller annen merkelig grunn
            Console.WriteLine(collection["isJustDoIt"]);
            model.status = collection["isJustDoIt"].Contains("true") == true ? STATUS.JUSTDOIT : STATUS.PLAN;

            //Henter ansattnr til forfatteren
            model.author_emp_id = Int32.Parse(User.FindFirstValue(ClaimTypes.UserData));

            if (collection["dueByTimestamp"].Equals("")) {
                ViewBag.TimestampError = "Legg til en frist";
                return View("Register", newModel);
            }
            model.timestamp = new TimestampEntity { dueByTimestamp = Convert.ToDateTime(collection["dueByTimestamp"]) };
            

            //Hjemmelaget verifisering for kategorier, dette sikrer at man har valgt minst en kategori
            var categories = parseCategories(collection);
            if (categories.Count < 1)
            {
                ViewBag.CategoryError = "Velg minst en kategori";
                return View("Register", newModel);
            }
            else
            {
                model.categories = categories;
            }

            //Hvis brukeren har lastet opp en fil prøver vi å laste den opp til mappen/lagre filstien i databasen
            if (file != null)
            {
                try
                {
                    //Hvis filen ble lastet opp i mappen legger man til bildet i modellen så filstien kan bli lastet opp i databasen. 
                    if (fileRepository.UploadFile(file))
                    {
                        ImageEntity imageEntity = new ImageEntity()
                        {
                            image_filepath = file.FileName
                        };
                        model.images.Add(imageEntity);
                        ViewBag.Message = "File Upload Successful";
                    }
                }
                catch (Exception ex)
                {
                    //Log ex
                    ViewBag.Error = "File Upload Failed";
                }
            }

            //Sjekker en siste gang at alt er good to go
            if (ModelState.IsValid)
            {
                suggestionRepository.CreateSuggestion(model);
                ViewBag.Message = "Forslaget ble postet!";
            }
            return View("Register", newModel);

        }

        /// <summary>
        /// Denne metoden klargjør nytt forslag modellen med å hente kategorier og de ansatte
        /// @Return: SuggestionRegisterModel full av data
        /// </summary>
       private SuggestionRegisterModel prepareSuggestionRegisterModel()
        {
            SuggestionRegisterModel model = new SuggestionRegisterModel();
            model.categories = suggestionRepository.GetAllCategories();
            model.possibleResponsibleEmployees = employeeRepository.GetEmployeeSelectList();

            return model;
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
        // Get: /Suggestion/Details/id
        [HttpGet]
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

        //Post: /Suggestion/CreateComment
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

        /*
         *Metode for å redigere kommentar 
         * @Param comment_id og suggestion_id
        */
        //Post: /Suggestion/EditComment
        [HttpPost]
        public IActionResult EditComment(CommentEntity comment, int suggestion_id)
        {
            ModelState.Remove("poster");
            ModelState.Remove("suggestion");
            if (ModelState.IsValid)
            {
                suggestionRepository.UpdateComment(new CommentEntity
                {
                    comment_id = comment.comment_id,
                    description = comment.description,
                    lastUpdatedTimestamp = DateTime.Now
                });
                return RedirectToAction("Details", "Suggestion", new { id = suggestion_id });
            }
            else
            {
                return RedirectToAction("Details", "Suggestion", new { id = suggestion_id });
            }

        }

        /*
         * Denne metoden gjør at du kan slette kommentarer i forslaget
         * @Parameter comment_id og suggestion id
         * @Return Suggestion/Details/suggestion_id
         */
        //Post: /Suggestion/Deletecomment
        public IActionResult DeleteComment(int comment_id, int suggestion_id)
        {
            var result = suggestionRepository.DeleteComment(comment_id);
            return RedirectToAction("Details", "Suggestion", new { id = suggestion_id });

        }

        //Favoritter
        //Post: /Suggestion/Favorite/id
        [HttpPost]
        public void Favorite(int id)
        {
            SuggestionEntity suggestion = suggestionRepository.GetSuggestionBySuggestionID(id);
            suggestion.favorite = !suggestion.favorite;
            suggestionRepository.Favorite(id, suggestion.favorite);
        }
        //Get: /Suggestion/Edit/id
        [HttpGet]
        public IActionResult Edit(int id)
        {
            SuggestionEditModel model = new SuggestionEditModel();
            model.suggestion = suggestionRepository.GetSuggestionBySuggestionIDWithCommentsAndImages(id);
            model.suggestion.responsible_employee = employeeRepository.GetEmployee(model.suggestion.ownership_emp_id);
            model.suggestion.author = employeeRepository.GetEmployee(model.suggestion.author_emp_id);
            model.possibleResponsibleEmployees = employeeRepository.GetEmployeeSelectList();

            return View(model);
        }

        //Post: /Suggestion/EditSuggestion
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult EditSuggestion(SuggestionEditModel model)
        {
            ModelState.Remove("possibleResponsibleEmployees");
            ModelState.Remove("suggestion.author");
            ModelState.Remove("suggestion.responsible_employee");
            //Denne sjekker om året ikke er 1 som er default året på en DateTime, dvs at ny dato er satt. 
            //Finnes sikkert en mer sikker måte å gjøre dette på men det fungerer. 
            if (model.newDueByDate == null)
            {
                model.suggestion.timestamp.dueByTimestamp = model.newDueByDate;
            }
            //Sjekker om ansvaret er oppdatert
            if (model.responsibleEmployeeID != model.suggestion.ownership_emp_id)
            {
                model.suggestion.ownership_emp_id = model.responsibleEmployeeID;
            }
            //Lager en ny suggestionentity som vi kan sende til repositoryet
            SuggestionEntity suggestion = new SuggestionEntity()
            {
                suggestion_id = model.suggestion.suggestion_id,
                description = model.suggestion.description + " " + model.newDescription,
                ownership_emp_id = model.suggestion.ownership_emp_id,
                timestamp = new TimestampEntity()
                {
                    dueByTimestamp = model.suggestion.timestamp.dueByTimestamp,
                    lastUpdatedTimestamp = DateTime.Now
                }
            };

            int result = suggestionRepository.UpdateSuggestion(suggestion);
            if (result == 0)
            {
                //Noe gikk galt
                ViewBag.Message = "";
            }
            return RedirectToAction("Edit", new { id = model.suggestion.suggestion_id });

        }

        //Oppdaterer status på et forslag
        //Suggestion/UpdateStatus
        public IActionResult UpdateStatus(int suggestion_id, STATUS status)
        {
            switch (status)
            {
                case STATUS.PLAN:
                    suggestionRepository.UpdateSuggestionStatus(suggestion_id, "DO");
                    break;
                case STATUS.DO:
                    suggestionRepository.UpdateSuggestionStatus(suggestion_id, "STUDY");
                    break;
                case STATUS.STUDY:
                    suggestionRepository.UpdateSuggestionStatus(suggestion_id, "ACT");
                    break;
                case STATUS.ACT:
                    suggestionRepository.UpdateSuggestionStatus(suggestion_id, "FINISHED");
                    break;
                case STATUS.JUSTDOIT:
                    suggestionRepository.UpdateSuggestionStatus(suggestion_id, "FINISHED");
                    break;
            }
            return RedirectToAction("Details", new { id = suggestion_id });
        }
    }
}

