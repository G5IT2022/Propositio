using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Web.Helpers;
using System.Linq;
using bacit_dotnet.MVC.Models.Statistics;
using bacit_dotnet.MVC.Repositories;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace bacit_dotnet.MVC.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsRepository statisticsRepository;

        public StatisticsController(IStatisticsRepository statisticsRepository)
        {
            this.statisticsRepository = statisticsRepository;
        }

        public IActionResult Index()
        {
            StatisticsViewModel model = new StatisticsViewModel();
            model.statuses = statisticsRepository.ListStatuses();
            model.ListNumberOfSuggestionsPerStatus = statisticsRepository.ListNumberOfSuggestionsPerStatus();
            model.teams = statisticsRepository.ListTeams();
            model.ListNumberOfSuggestionPerTeam = statisticsRepository.ListNumberOfSuggestionsPerTeam();
            model.employees = statisticsRepository.ListEmployees();
            model.ListTopNumberOfSuggestionsOfTopFiveEmployees = statisticsRepository.ListTopNumberOfSuggestionsOfTopFiveEmployees();
            model.categories = statisticsRepository.ListCategories();
            model.ListNumberOfSuggestionsPerCategory = statisticsRepository.ListNumberOfSuggestionsPerCategory();
            model.totalNumberOfcategoriesForSuggestions = statisticsRepository.TotalNumberCategoriesFromSuggestions();
            return View(model);
        }
    }
}
