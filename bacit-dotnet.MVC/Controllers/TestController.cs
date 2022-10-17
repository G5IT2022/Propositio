using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.Test;
using bacit_dotnet.MVC.Repositories.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bacit_dotnet.MVC.Controllers
{
    public class TestController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        public TestController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        // GET: TestController
        public ActionResult Index()
        {
            TestViewModel model = new TestViewModel();
            model.categories = categoryRepository.GetAll();
            return View(model);
        }
        // POST: TestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryEntity category)
        {
            if (ModelState.IsValid)
            {
                int i = categoryRepository.Create(category);
            }
            return View("Index");
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
        // POST: TestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(TestViewModel model)
        {

            CategoryEntity category = new CategoryEntity();
            int i = categoryRepository.Delete(category);

            return RedirectToAction("Index");
        }
    }
}
