using Microsoft.AspNetCore.Mvc;
using bacit_dotnet.MVC.Models;
using bacit_dotnet.MVC.Repositories.Employee;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace bacit_dotnet.MVC.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        public AdminController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewUser()
        {
            ViewBag.Message = "Registrer ny ansatt";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewUser(AdminNewUserModel model, IFormCollection coll)
        {
            int result = 0;
            if (ModelState.IsValid)
            {
                EmployeeEntity newEmp = new EmployeeEntity
                {
                    emp_id = model.emp_id,
                    name = model.first_name + " " + model.last_name,
                    salt = PassHash.GenerateSalt(),
                    passwordhash = model.password,
                    authorization_role_id = 1,
                    role_id = 1
                };
                var tmp = PassHash.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(newEmp.passwordhash), newEmp.salt);
                newEmp.passwordhash = Convert.ToBase64String(tmp);
                result = employeeRepository.Create(newEmp);
            }
            if(result != 1)
            {
                ViewBag.Created = "Noe gikk galt, prøv igjen.";
            }
            else
            {
                ViewBag.Created = $"Ansatt {model.first_name + " " + model.last_name} ble opprettet!";
            }
            return View("NewUser");

        }
    }
}
