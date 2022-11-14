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

namespace bacit_dotnet.MVC.Controllers
{
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
            model.teams = statisticsRepository.ListTeams();
            model.ListNumberOfSuggestionPerTeam = statisticsRepository.ListNumberOfSuggestionsPerTeam();
            return View(model);
        }
    }
}
