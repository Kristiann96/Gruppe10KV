/*using Microsoft.AspNetCore.Mvc;
using Gruppe10KVprototype.Models; // Make sure to include the namespace for UserCaseModel and CaseService

namespace Gruppe10KVprototype.Controllers
{
    public class UserPagesController : Controller
    {
        private readonly CaseService _caseService;

        // Constructor to inject CaseService
        public UserPagesController(CaseService caseService)
        {
            _caseService = caseService;
        }

        public IActionResult UserCases()
        {
            // Get user cases for the logged-in user
            var userCases = _caseService.GetUserCasesForUser(User.Identity.Name);
            return View(userCases); // Pass the user cases to the view
        }
    }
}
*/

using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class InnmelderSiderController : Controller
{
    public IActionResult UserCases()
    {
        return View("~/Views/UserPages/UserCase.cshtml");
    }
}