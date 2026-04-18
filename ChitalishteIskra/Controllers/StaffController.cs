using Microsoft.AspNetCore.Mvc;

namespace ChitalishteIskra.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult ANPT() => View();
        public IActionResult Babsbg() => View();
        public IActionResult Iskritsa() => View();
        public IActionResult Sevtopolis() => View();
        public IActionResult PetkoStainov() => View();
        public IActionResult DetskaTeatralna() => View();
        public IActionResult MladejkaTeatralna() => View();
        public IActionResult ArtSchool() => View();
        public IActionResult Blue() => View();
        public IActionResult SportniTantsi() => View();
    }
}
