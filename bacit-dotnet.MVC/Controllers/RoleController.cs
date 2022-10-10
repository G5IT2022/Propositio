using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Repositories.Role;
using Microsoft.AspNetCore.Mvc;

namespace bacit_dotnet.MVC.Controllers
{
    public class RoleController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoleRepository roleRepository;

        public RoleController(ILogger<HomeController> logger, IRoleRepository roleRepository)
        {
            _logger = logger;
            this.roleRepository = roleRepository;
        }

        public IActionResult Index()
        {
            RoleViewModel model = new RoleViewModel();
            model.roles = roleRepository.GetAllRoles();
            return View(model);
        }
    }
}
