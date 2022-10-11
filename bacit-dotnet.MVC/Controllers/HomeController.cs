using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using bacit_dotnet.MVC.Repositories.Employee;

namespace bacit_dotnet.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository employeeRepository;

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            this.employeeRepository= employeeRepository;
        }

        public IActionResult Index()
        {
            EmployeeViewModel model = new EmployeeViewModel();
            model.employees = employeeRepository.GetAll();
            return View(model);
        }


    }
}