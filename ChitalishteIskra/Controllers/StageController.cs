using Microsoft.AspNetCore.Mvc;

namespace ChitalishteIskra.Controllers
{
    public class StageController:Controller
    {
        public IActionResult Stage() => View("Stage");
    }
}
