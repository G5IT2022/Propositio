﻿using bacit_dotnet.MVC.Controllers;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace bacit_dotnet.MVC.Tests.Controllers
{
    [TestClass]
    public class SuggestionControllerUnitTests
    {
        private readonly IAdminRepository _mockAdminRepository;
        private readonly int _roleId = 3;
        private readonly SuggestionEntity _createSuggestionModel = new SuggestionEntity
        {
            title = "Title",
            description = "Description",
            ownership_emp_id = 1,
            author_emp_id = 1,
            favorite = true,
            status = STATUS.ACT,
            suggestion_id = 1,

        };
        private IFormCollection _collection;
        private List<CategoryEntity> _categories;
        private List<SelectListItem> _employeeSelectList;

        private void InitData()
        {

            Dictionary<string, StringValues> fields = new Dictionary<string, StringValues>();
            fields.Add("isJustDoIt", "true");
            fields.Add("dueByTimestamp", "");
            _collection = new FormCollection(fields);

            _categories = new List<CategoryEntity>();
            _employeeSelectList = new List<SelectListItem>();
        }
        [Fact]
        public void TestCreateSuggestion_WithDueByTimestamp_Empty()
        {
            //we have to use mockdata to create fake data so that it will not affect to our database            
            InitData();
            var mockLogger = new Mock<ILogger<SuggestionController>>();
            //create new object interface because we have dependency, so we need to create new object in order to pass interface into parameter
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(a => a.GetAllCategories()).Returns(_categories);
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            mockEmployeeRepository.Setup(a => a.GetEmployeeSelectList()).Returns(_employeeSelectList);
            var mockSuggestionRepository = new Mock<ISuggestionRepository>();
            var mockFileRepository = new Mock<IFileRepository>();

            //Constructor
            var controller = new SuggestionController(mockLogger.Object, mockAdminRepository.Object, mockEmployeeRepository.Object, mockSuggestionRepository.Object, mockFileRepository.Object).WithIdentity("2","test","1");
            var viewResult = controller.CreateSuggestion(_createSuggestionModel, _collection) as ViewResult;
            Assert.AreEqual(viewResult.ViewName, "Register");
        }
    }
}
