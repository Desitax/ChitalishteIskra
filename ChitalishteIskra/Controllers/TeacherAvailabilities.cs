using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Controllers
{
    public class TeacherAvailabilities:Controller
    {
        public readonly ChitalishteIskraDbContext context;

        public TeacherAvailabilities(ChitalishteIskraDbContext _context)
        {
            this.context = _context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var teacherAvailabilities = await context.TeacherAvailabilities.ToListAsync();
            return View(teacherAvailabilities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeacherAvailability availabilities)
        {
            await context.TeacherAvailabilities.AddAsync(availabilities);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var edit = await context.Groups.FirstOrDefaultAsync(e => e.Id == id);
            if (edit == null)
            {
                return NotFound();
            }
            return View(edit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeacherAvailability teacherAvailabilities)
        {
            context.TeacherAvailabilities.Update(teacherAvailabilities);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var edit = await context.TeacherAvailabilities.FirstOrDefaultAsync(e => e.Id == id);
            if (edit != null)
            {
                context.TeacherAvailabilities.Remove(edit);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}
