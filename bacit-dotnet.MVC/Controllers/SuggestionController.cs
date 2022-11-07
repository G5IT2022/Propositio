﻿using bacit_dotnet.MVC.Authentication;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;
using bacit_dotnet.MVC.Models.Suggestion;
using bacit_dotnet.MVC.Repositories;

namespace bacit_dotnet.MVC.Controllers
{

    [Authorize]
    public class SuggestionController : Controller
    {

        private readonly IEmployeeRepository employeeRepository;
        private readonly ISuggestionRepository suggestionRepository;


        public SuggestionController(IEmployeeRepository employeeRepository, ISuggestionRepository suggestionRepository)
        {
            this.suggestionRepository = suggestionRepository;
            this.employeeRepository = employeeRepository;
        }
        [Authorize]
        public IActionResult Index()
        {
            EmployeeSuggestionViewModel model = new EmployeeSuggestionViewModel();
            model.employees = employeeRepository.GetEmployees();
            foreach (EmployeeEntity emp in model.employees)
            {
                emp.suggestions = suggestionRepository.GetSuggestionsByAuthorID(emp.emp_id);
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
        public IActionResult Create(SuggestionRegisterModel model, IFormCollection collection)
        {
            ModelState.Remove("Categories");
            if (ModelState.IsValid)
            {
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

