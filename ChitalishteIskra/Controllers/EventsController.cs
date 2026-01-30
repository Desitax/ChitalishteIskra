using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Controllers
{
    public class EventsController:Controller
    {
        public readonly ChitalishteIskraDbContext context;

        public EventsController(ChitalishteIskraDbContext _context)
        {
            this.context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var events = await context.Events.ToListAsync();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event evnt)
        {
            await context.Events.AddAsync(evnt);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var edit = await context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (edit == null)
            {
                return NotFound();
            }
            return View(edit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Event evnt)
        {
            context.Events.Update(evnt);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var edit = await context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (edit != null)
            {
                context.Events.Remove(edit);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
