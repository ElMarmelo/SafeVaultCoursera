using Microsoft.AspNetCore.Mvc;
using SafeVault.Web.Models;
using System.Threading.Tasks;

namespace SafeVault.Web.Controllers
{
    public class WebformController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(WebformModel model)
        {
            if (!ModelState.IsValid)
            {
                // If validation fails, return the form view with error messages
                return View("Index", model);
            }

            // If validation passes, process the data (e.g., save to database)
            await ProcessFormData(model);

            return RedirectToAction("Success");
        }

        private async Task ProcessFormData(WebformModel model)
        {
            // Implement your logic here to process the form data
        }
    }
}
