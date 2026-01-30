using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static ChitalishteIskra.Data.Entities.LessonType;

namespace ChitalishteIskra.Controllers
{
    public class BookLessonsController:Controller
    {
        public readonly ChitalishteIskraDbContext context;
        public BookLessonsController(ChitalishteIskraDbContext _context)
        {
            this.context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var booking = await context.BookLessons.ToListAsync();
            return View(booking);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var lessonType = new LessonType();

            ViewBag.LessonTypes = Enum.GetValues(typeof(LessonTypeName))
               .Cast<LessonTypeName>()
               .Select(e => new SelectListItem
               {
                   Value = ((int)e).ToString(),   
                   Text = e.ToString()            
               })
               .ToList();

            return View(new BookLesson());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookLesson booking)
        {
            await context.BookLessons.AddAsync(booking);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var booking = await context.BookLessons.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookLesson booking)
        {
            context.BookLessons.Update(booking);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var booking = await context.BookLessons.FirstOrDefaultAsync(b => b.Id == id);
            if (booking != null)
            {
                context.BookLessons.Remove(booking);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
