using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AuthInterface;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Microsoft.AspNetCore.Authorization;

namespace Controller.UnitTests
{
    [TestClass]
    public class LandingsSideControllerTester
    {
        private LandingsSideController _kontroller;

        [TestInitialize]
        public void Oppsett()
        {
            _kontroller = new LandingsSideController();

            // Setup standard HttpContext med authentisert bruker
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "test@example.com"),
                new Claim(ClaimTypes.Role, UserRoles.Innmelder)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var principal = new ClaimsPrincipal(identity);

            _kontroller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #region Sikkerhet
        [TestMethod]
        [Description("Tester at controller har Authorize attributt med Innmelder rolle")]
        public void Controller_HarAuthorizeAttributtMedInnmelderRolle()
        {
            // Arrange & Act
            var authorizeAttribute = typeof(LandingsSideController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.IsNotNull(authorizeAttribute, "Controller mangler Authorize attributt");
            Assert.AreEqual(UserRoles.Innmelder, authorizeAttribute.Roles, 
                "Authorize attributt har ikke korrekt rolle");
        }

        [TestMethod]
        [Description("Tester at controller har AntiForgeryToken beskyttelse")]
        public void Controller_HarAntiForgeryTokenBeskyttelse()
        {
            // Arrange & Act
            var attributes = typeof(LandingsSideController)
                .GetCustomAttributes(typeof(AutoValidateAntiforgeryTokenAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any(), "Controller mangler AutoValidateAntiforgeryToken attributt");
        }
        #endregion

        #region View Tester
        [TestMethod]
        [Description("Tester at LandingsSide-metoden returnerer korrekt View")]
        public void LandingsSide_ReturnererStandardView()
        {
            // Act
            var resultat = _kontroller.LandingsSide() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat, "Resultatet skal være en ViewResult");
            Assert.IsNull(resultat.ViewName, "ViewName skal være null (bruker standard view)");
        }

        [TestMethod]
        [Description("Tester at LandingsSide ikke returnerer noen modell")]
        public void LandingsSide_ReturnererIngenModell()
        {
            // Act
            var resultat = _kontroller.LandingsSide() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat, "Resultatet skal være en ViewResult");
            Assert.IsNull(resultat.Model, "ViewResult skal ikke inneholde noen modell");
        }
        #endregion
    }
}