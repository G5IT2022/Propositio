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
            return View();
        }
        [HttpPost]
        public IActionResult NewUser(AdminNewUserModel model)
        {
            if (ModelState.IsValid)
            {
                EmployeeEntity newEmp = new EmployeeEntity
                {
                    emp_id = model.emp_id,
                    name = model.name,
                    salt = PassHash.GenerateSalt(),
                    passwordhash = model.password,
                    authorization_role_id = 1,
                    role_id = 1
                };
                var tmp = PassHash.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(newEmp.passwordhash), newEmp.salt);
                newEmp.passwordhash = Convert.ToBase64String(tmp);
                employeeRepository.Create(newEmp);
            }
            return View("NewUser");

        }
    }
}
