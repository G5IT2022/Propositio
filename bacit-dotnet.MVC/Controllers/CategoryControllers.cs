using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace bacit_dotnet.MVC.Controllers
{
    public class CategoryControllers : Controller 
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryRepository categoryRepository;

        public CategoryControllers(ILogger<HomeController> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            this.categoryRepository = categoryRepository;
        }

        public IActionResult CategoryIndex()
        {
            CategoryViewModel model = new CategoryViewModel();
            model.categories = categoryRepository.getAll();
            return View(model);
        }

    }
}

