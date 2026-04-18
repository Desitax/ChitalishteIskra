using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Controllers
{
    public class ContactController : Controller
    {
        private readonly ChitalishteIskraDbContext dbContext;

        public ContactController(ChitalishteIskraDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var message = new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                Content = model.Content,
                SentOn = DateTime.UtcNow,
                IsRead = false
            };

            await dbContext.ContactMessages.AddAsync(message);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Вашето съобщение беше изпратено успешно.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Messages()
        {
            var messages = await dbContext.ContactMessages
                .OrderByDescending(m => m.SentOn)
                .ToListAsync();

            return View(messages);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var message = await dbContext.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            if (!message.IsRead)
            {
                message.IsRead = true;
                await dbContext.SaveChangesAsync();
            }

            return View(message);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await dbContext.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            dbContext.ContactMessages.Remove(message);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Съобщението беше изтрито успешно.";

            return RedirectToAction(nameof(Messages));
        }
    }
}