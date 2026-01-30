using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static ChitalishteIskra.Data.Entities.LessonType;

namespace ChitalishteIskra.Controllers
{
    public class LessonsController:Controller
    {
        public readonly ChitalishteIskraDbContext context;
        public LessonsController(ChitalishteIskraDbContext _context)
        {
            this.context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lesson = await context.Lessons
                .Include(l => l.Type)
                .ToListAsync();
            return View(lesson);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var lessonType = new LessonType();

            ViewBag.LessonTypes = Enum.GetValues(typeof(LessonTypeName))
               .Cast<LessonTypeName>()
               .ToList()
               .Select(d => new SelectListItem()
               {
                   Text = d.ToString(),
                   Value = d.ToString()
               })
               .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lesson lesson)
        {
            await context.Lessons.AddAsync(lesson);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var lesson = await context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Lesson lesson)
        {
            context.Lessons.Update(lesson);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var lesson = await context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson != null)
            {
                context.Lessons.Remove(lesson);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
