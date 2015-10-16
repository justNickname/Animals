using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Animals.Controllers;
using Animals.Models;
using System.Web.Mvc;
using System.Web;


namespace Animals.Tests
{
    [TestClass]
    public class AnimalControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            AnimalController controller = new AnimalController(new AnimalFakeRepository());
            // Act
            ActionResult result = controller.Index() as ActionResult;
            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void Create()
        {
            // Arrange
            AnimalController controller = new AnimalController(new AnimalFakeRepository());
            // Act
            ActionResult result = controller.Create() as ActionResult;
            // Assert
            Assert.IsNotNull(result);
        }
    }
}
