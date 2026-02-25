using Microsoft.AspNetCore.Mvc;

namespace ChitalishteIskra.Controllers
{
    public class AboutController:Controller
    {
        public IActionResult History() => View();
        public IActionResult Statutes() => View();
        public IActionResult Board() => View();
        public IActionResult Team() => View();
        public IActionResult Documents() => View();
        public IActionResult Projects() => View();
    }
}
