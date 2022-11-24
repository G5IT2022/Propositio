using bacit_dotnet.MVC.Controllers;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.Suggestion;
using bacit_dotnet.MVC.Repositories;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace bacit_dotnet.MVC.Tests.Controllers
{
    [TestClass]
    public class SuggestionControllerUnitTests
    {

        //Felter
        private readonly Mock<ILogger<SuggestionController>> mockLogger;
        private readonly Mock<IAdminRepository> mockAdminRepository;
        private readonly Mock<ISuggestionRepository> mockSuggestionRepository;
        private readonly Mock<IEmployeeRepository> mockEmployeeRepository;
        private readonly Mock<IFileRepository> mockFileRepository;
        private readonly SuggestionController controller;
        Dictionary<string, StringValues> fields;

        //For Test
        private readonly SuggestionEntity createSuggestionModel = new SuggestionEntity
        {
            title = "Title",
            description = "Description",
            ownership_emp_id = 1,
            author_emp_id = 1,
            favorite = true,
            status = STATUS.ACT,
            suggestion_id = 1,

        };
        private IFormCollection collection;
        private List<CategoryEntity> categories;
        private List<SelectListItem> employeeSelectList;
        private List<SuggestionEntity> suggestions;
        private List<EmployeeEntity> employees;

        //Constructor
        public SuggestionControllerUnitTests()
        {
            //we have to use mockdata to create fake data so that it will not affect to our database            
            //  var mockLogger = new Mock<ILogger<SuggestionController>>();
            //create new object interface because we have dependency, so we need to create new object in order to pass interface into parameter
            //Oppsett av private felter
            fields = new Dictionary<string, StringValues>();
            categories = new List<CategoryEntity>() { new CategoryEntity() { category_id = 1, category_name = "HMS" } };
            employeeSelectList = new List<SelectListItem>();
            suggestions = new List<SuggestionEntity>();
            employees = new List<EmployeeEntity>();

            //Universell data for alle tester, legger inn data i listen suggestions og employees


            employees.Add(new EmployeeEntity()
            {
                emp_id = 1,
                name = "Johan"
            });
            employees.Add(new EmployeeEntity()
            {
                emp_id = 2,
                name = "Geir"
            });

            suggestions.Add(new SuggestionEntity()
            {
                title = "Title",
                description = "Description",
                ownership_emp_id = 1,
                author_emp_id = 1,
                favorite = true,
                status = STATUS.ACT,
                suggestion_id = 2
            });
            suggestions.Add(new SuggestionEntity()
            {
                title = "Tittel2",
                description = "Søkbar tekst her",
                ownership_emp_id = 1,
                author_emp_id = 1,
                favorite = true,
                status = STATUS.JUSTDOIT,
                suggestion_id = 3
            });






            //Dependencies
            mockLogger = new Mock<ILogger<SuggestionController>>();
            mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(a => a.GetAllCategories()).Returns(categories);
            mockEmployeeRepository = new Mock<IEmployeeRepository>();
            mockEmployeeRepository.Setup(a => a.GetEmployeeSelectList()).Returns(employeeSelectList);
            mockSuggestionRepository = new Mock<ISuggestionRepository>();
            mockSuggestionRepository.Setup(a => a.GetAll()).Returns(suggestions);
            mockFileRepository = new Mock<IFileRepository>();

            //System Under Test (SUT)
            controller = new SuggestionController(mockLogger.Object, mockAdminRepository.Object, mockEmployeeRepository.Object, mockSuggestionRepository.Object, mockFileRepository.Object).WithIdentity("2", "test", "1");
        }

        [Fact]
        public void SuggestionController_Index_Success()
        {
            //Arrange

            //Act
            var viewResult = controller.Index("", "", "") as ViewResult;

            //Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Index", viewResult.ViewName);
        }
        [Fact]
        public void SuggestionController_Index_Filter1Result()
        {
            //Arrange (Gjort i constructor
    

            //Act
            var viewResult = controller.Index("", "", "ACT") as ViewResult;
            var viewResultModel = viewResult.Model as SuggestionViewModel;

            //Assert
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.AreEqual(1, viewResultModel.suggestions.Count);
        }
        [Fact]
        public void SuggestionController_Index_Filter0Results()
        {
            //Arrange (Gjort i constructor)

            //Act
            var viewResult = controller.Index("", "", "PLAN") as ViewResult;
            var viewResultModel = viewResult.Model as SuggestionViewModel;

            //Assert
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.AreEqual(0, viewResultModel.suggestions.Count);
        }

        [Fact]
        public void SuggestionController_CreateSuggestion_ReturnRegisterFail()
        {

            //Arrange (Hvilken data trenger vi for å gjennomføre denne testen?)
            fields.Add("isJustDoIt", "true");
            fields.Add("dueByTimestamp", "");
            fields.Add("HMS", "");
            collection = new FormCollection(fields);

            //Act
            var viewResult = controller.CreateSuggestion(createSuggestionModel, collection) as ViewResult;
           
            //Assert
            Assert.AreEqual("Register", viewResult.ViewName);
        }
        
        [Fact]
        public void SuggestionController_CreateSuggestion_ReturnIndexSuccess()
        {
            //Arrange 
            fields.Add("isJustDoIt", "true");
            fields.Add("dueByTimestamp", "2022 - 11 - 29");
            fields.Add("HMS", "");
            collection = new FormCollection(fields);

           // var controller = new SuggestionController(mockLogger.Object, mockAdminRepository.Object, mockEmployeeRepository.Object, mockSuggestionRepository.Object, mockFileRepository.Object).WithIdentity("2", "test", "1");
            //Act
            var viewResult = (RedirectToActionResult) controller.CreateSuggestion(createSuggestionModel, collection);

            //Assert
            Assert.AreEqual("Index", viewResult.ActionName);
            Assert.AreEqual("Suggestion", viewResult.ControllerName);
        }
    }
}
