using bacit_dotnet.MVC.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using bacit_dotnet.MVC.Models.Suggestion;
using bacit_dotnet.MVC.Repositories;
using bacit_dotnet.MVC.Helpers;

namespace bacit_dotnet.MVC.Controllers
{

    [Authorize]
    public class SuggestionController : Controller
    {
        private readonly ILogger<SuggestionController> logger;
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;
        private readonly IAdminRepository adminRepository;
        private readonly IFileRepository fileRepository;

        //Constructor
        public SuggestionController(ILogger<SuggestionController> logger,IAdminRepository adminRepository, IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository, IFileRepository fileRepository)
        {
            this.logger = logger;
            this.suggestionRepository = suggestionRepository;
            this.employeeRepository = employeeRepository;
            this.adminRepository = adminRepository;
            this.fileRepository = fileRepository;
        }

        //GET: /Suggestion/Index
        [HttpGet]
        public IActionResult Index(string sortOrder, string searchString, string filterParameter)
        {
            //Vi måtte bruke en annen modell enn den vi hadde før fordi vi må filtrere alle forslagene så vi må bruke en modell med kun listen over forslag
            SuggestionViewModel model = new SuggestionViewModel();
            model.suggestions = suggestionRepository.GetAll();

            //Henter kategoriene for filtrering
            model.categories = adminRepository.GetAllCategories();
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
            logger.LogInformation(LoggingHelper.PageAccessedLog(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), "SuggestionIndex"));
            return View("Index", model);
        }

        //GET: /Suggestion/Register
        [HttpGet]
        public IActionResult Register(SuggestionRegisterModel model)
        {
            //Sjekker om modellen allerede er preparert, hvis den ikke er det så prepares den. 
            if (model.possibleResponsibleEmployees == null)
            {
                model = prepareSuggestionRegisterModel();
                ModelState.Clear();
            }
            logger.LogInformation(LoggingHelper.PageAccessedLog(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), "New Suggestion"));
            return View("Register", model);
        }

        //POST: /Suggestion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSuggestion(SuggestionEntity model, IFormCollection collection, IFormFile file = null)
        {

            //Logging variables
            var loggingEntity = "suggestion";

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
                logger.LogInformation(LoggingHelper.EntityCreatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity, "invalid modelstate"));
                return View("Register", newModel);
            }

            //Sett statusen på forslaget basert på om justdoit er true eller false, hvis man ikke har .Contains("true") så fungerer det ikke av en eller annen merkelig grunn
            model.status = collection["isJustDoIt"].Contains("true") == true ? STATUS.JUSTDOIT : STATUS.PLAN;

            //Henter ansattnr til forfatteren
            model.author_emp_id = Int32.Parse(User.FindFirstValue(ClaimTypes.UserData));


            //Sjekker om det er satt en frist
            if (collection["dueByTimestamp"].Equals("")){
                logger.LogInformation(LoggingHelper.EntityCreatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity, "no timestamp"));
                ViewBag.TimestampError = "Legg til en frist";
                return View("Register", newModel);
            }

            //Sjekker om fristen er satt i fremtiden
            DateTime newTime = Convert.ToDateTime(collection["dueByTimestamp"]);
            if (newTime.CompareTo(DateTime.Now) < 1)
            {
                logger.LogInformation(LoggingHelper.EntityCreatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
                ViewBag.TimestampError = "Fristen du har valgt har gått ut, vennligst velg en annen frist.";
                return View("Register", newModel);

            }
            model.timestamp = new TimestampEntity { dueByTimestamp = Convert.ToDateTime(collection["dueByTimestamp"]) };


            //Hjemmelaget verifisering for kategorier, dette sikrer at man har valgt minst en kategori
            var categories = parseCategories(collection);
            if (categories.Count < 1)
            {
                logger.LogInformation(LoggingHelper.EntityCreatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity, "no categories"));
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
                        logger.LogInformation("File: {name} uploaded successfully on {date}", imageEntity.image_filepath, DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    //Log ex
                    logger.LogCritical("File upload failed", ex);
                    ViewBag.Error = "File Upload Failed";
                }
            }

            //Sjekker en siste gang at alt er good to go
            if (ModelState.IsValid)
            {
                suggestionRepository.CreateSuggestion(model);
                ViewBag.Message = "Forslaget ble postet!";
                logger.LogInformation(LoggingHelper.EntityCreatedLogSuccess(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
            }
            return RedirectToAction("Index", "Suggestion", new {sortOrder="date_new"});

        }
        
        //GET: /Suggestion/Details/id
        [HttpGet]
        public IActionResult Details(int id)
        {
            SuggestionDetailsModel model = prepareSuggestionDetailsModel(id);
            foreach (CommentEntity comment in model.suggestion.comments)
            {
                if (comment != null)
                {
                    comment.poster = employeeRepository.GetEmployee(comment.emp_id);
                }
            }
            logger.LogInformation(LoggingHelper.PageAccessedLog(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), "Details"));
            return View("Details", model);
        }

        //POST: /Suggestion/CreateComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateComment(SuggestionDetailsModel model, IFormCollection collections)
        {
            var loggingEntity = "comment";
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
                logger.LogInformation(LoggingHelper.EntityCreatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
                return RedirectToAction("Details", "Suggestion", new { id = comment.suggestion_id });
            }
            else
            {
                logger.LogInformation(LoggingHelper.EntityCreatedLogSuccess(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
                return RedirectToAction("Details", "Suggestion", new { id = comment.suggestion_id });
            }
        }

        //POST: /Suggestion/EditComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditComment(CommentEntity comment, int suggestion_id)
        {
            var loggingEntity = "comment";
            
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
                logger.LogInformation(LoggingHelper.EntityUpdatedLogSuccess(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
                return RedirectToAction("Details", "Suggestion", new { id = suggestion_id });
            }
            else
            {
                logger.LogInformation(LoggingHelper.EntityUpdatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
                return RedirectToAction("Details", "Suggestion", new { id = suggestion_id });
            }

        }

        //POST: /Suggestion/Deletecomment/id
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(int comment_id, int suggestion_id)
        {
            var loggingEntity = "comment";
            var result = suggestionRepository.DeleteComment(comment_id);
            if(result != 1)
            {
                logger.LogInformation(LoggingHelper.EntityDeleteLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
            }
            else
            {
                logger.LogInformation(LoggingHelper.EntityDeleteLogSuccess(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
            }
            return RedirectToAction("Details", "Suggestion", new { id = suggestion_id });

        }

        //POST: /Suggestion/Favorite/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Favorite(int id, bool isFavorite)
        {
            isFavorite = !isFavorite;
            suggestionRepository.Favorite(id, isFavorite);
        }

        //GET: /Suggestion/Edit/id
        [HttpGet]
        public IActionResult Edit(int id, SuggestionEditModel model = null)
        {
            logger.LogInformation(LoggingHelper.PageAccessedLog(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), "edit suggestion"));
            //Hvis vi kommer til denne metoden fra detaljersiden vil forslaget i modellen være null, ellers vil den ha en verdi. 
            if (model.suggestion == null)
            {
                model = prepareSuggestionEditModel(id);
            }

            return View("Edit", model);
        }

        //POST: /Suggestion/EditSuggestion
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult EditSuggestion(SuggestionEditModel model)
        {
            //Logging
            var loggingEntity = "suggestion";
            //Model 
            //Fjerner overflødig data som ikke skal med i verifiseringen. 
            ModelState.Remove("possibleResponsibleEmployees");
            ModelState.Remove("suggestion.author");
            ModelState.Remove("suggestion.responsible_employee");

            SuggestionEditModel newModel = prepareSuggestionEditModel(model.suggestion.suggestion_id);
            //Denne sjekker om året ikke er 1 som er default året på en DateTime, dvs at ny dato er satt. 
            //Finnes sikkert en mer sikker måte å gjøre dette på men det fungerer. 
            if (model.newDueByDate.Year != 1)
            {
                //Sjekker om man har satt en dato i fremtiden
                if (model.newDueByDate.CompareTo(DateTime.Now) > 0)
                {
                    model.suggestion.timestamp.dueByTimestamp = model.newDueByDate;
                }
                else
                {

                    logger.LogInformation(LoggingHelper.EntityUpdatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity, "expired date input"));
                    ViewBag.Message = "Fristen du har valgt har gått ut, vennligst velg en annen frist.";
                    return View("Edit", newModel);
                }
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
            if (result != 0)
            {
                //Det funket, returner brukeren til detaljer siden
                logger.LogInformation(LoggingHelper.EntityUpdatedLogSuccess(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
                return RedirectToAction("Details", new { id = model.suggestion.suggestion_id });
            }
            //Hvis metoden kommer så langt har noe gått galt, send brukeren tilbake til redigersiden med feilmelding. 
            ViewBag.Message = "Forslaget kunne ikke bli oppdatert, prøv igjen.";
            logger.LogInformation(LoggingHelper.EntityUpdatedLogFailed(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), loggingEntity));
            return View("Edit", new { id = model.suggestion.suggestion_id, model = newModel });
        }

        /// <summary>
        /// Denne metoden oppdaterer statusen på ett forslag
        /// </summary>
        /// <param name="suggestion_id">En suggestion id</param>
        /// <param name="status">En status i fra STATUS enum i suggestionEntity</param>
        /// <returns>IActionResult Suggestion/Details/id</returns>
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
            logger.LogInformation(LoggingHelper.EntityUpdatedLogSuccess(Int32.Parse(User.FindFirstValue(ClaimTypes.UserData)), "suggestionStatus"));
            return RedirectToAction("Details", new { id = suggestion_id });
        }


        /// <summary>
        /// Denne metoden gjør en IFormCollection om til en liste med categryEntities så vi kan legge de inn i db
        /// </summary>
        /// <param name="collection">En IFormCollection</param>
        /// <returns>En liste med CategoryEntity</returns>
        private List<CategoryEntity> parseCategories(IFormCollection collection)
        {
            List<CategoryEntity> availableCategories = adminRepository.GetAllCategories();
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


        /// <summary>
        /// Denne metoden klargjør en ny SuggestionRegisterModel full av data
        /// </summary>
        /// <returns>SuggestionRegisterModel</returns>
        private SuggestionRegisterModel prepareSuggestionRegisterModel()
        {
            SuggestionRegisterModel model = new SuggestionRegisterModel();
            model.categories = adminRepository.GetAllCategories();
            model.possibleResponsibleEmployees = employeeRepository.GetEmployeeSelectList();

            return model;
        }

        /// <summary>
        /// Denne metoden klargjør en ny SuggestionDetailsModel full av data
        /// </summary>
        /// <param name="suggestion_id">En suggestion id</param>
        /// <returns>SuggestionDetailsModel</returns>
        private SuggestionDetailsModel prepareSuggestionDetailsModel(int suggestion_id)
        {
            SuggestionDetailsModel model = new SuggestionDetailsModel();
            model.suggestion = suggestionRepository.GetSuggestionBySuggestionID(suggestion_id);
            model.employee = employeeRepository.GetEmployee(model.suggestion.author_emp_id);
            model.suggestion.author = employeeRepository.GetEmployee(model.suggestion.author_emp_id);
            model.suggestion.responsible_employee = employeeRepository.GetEmployee(model.suggestion.ownership_emp_id);

            return model;
        }

        /// <summary>
        /// Denne metoden klargjør en ny SuggestionEditModel full av data
        /// </summary>
        /// <param name="suggestion_id">En suggestion id</param>
        /// <returns>SuggestionEditModel</returns>
        private SuggestionEditModel prepareSuggestionEditModel(int suggestion_id)
        {
            SuggestionEditModel model = new SuggestionEditModel();
            model.suggestion = suggestionRepository.GetSuggestionBySuggestionID(suggestion_id);
            model.suggestion.responsible_employee = employeeRepository.GetEmployee(model.suggestion.ownership_emp_id);
            model.suggestion.author = employeeRepository.GetEmployee(model.suggestion.author_emp_id);
            model.possibleResponsibleEmployees = employeeRepository.GetEmployeeSelectList();
            return model;
        }
    }
}

