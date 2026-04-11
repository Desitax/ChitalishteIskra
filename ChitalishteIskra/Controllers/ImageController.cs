using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Models.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChitalishteIskra.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ImageController : Controller
    {
        private readonly IImageService imageService;

        public ImageController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View(new ImageUploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Picture == null || model.Picture.Length == 0)
            {
                ModelState.AddModelError(nameof(model.Picture), "Моля, избери валидна снимка.");
                return View(model);
            }

            var fileName = $"history-{Guid.NewGuid():N}";
            var imageUrl = await imageService.UploadImageAsync(model.Picture, fileName);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ModelState.AddModelError(string.Empty, "Снимката не можа да бъде качена в Cloudinary.");
                return View(model);
            }

            model.ImageUrl = imageUrl;
            ViewBag.Message = "Снимката е качена успешно.";

            return View(model);
        }
    }
}
