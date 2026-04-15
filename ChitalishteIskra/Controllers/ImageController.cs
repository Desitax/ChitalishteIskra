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


        [HttpGet]
        public IActionResult UploadFile()
        {
            return View(new FileUploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(FileUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Document == null || model.Document.Length == 0)
            {
                ModelState.AddModelError(nameof(model.Document), "Моля, избери файл.");
                return View(model);
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(model.Document.FileName);
            var safeName = $"{fileNameWithoutExtension}-{Guid.NewGuid():N}";

            var fileUrl = await imageService.UploadFileAsync(model.Document, safeName);

            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                ModelState.AddModelError(string.Empty, "Файлът не можа да бъде качен.");
                return View(model);
            }

            model.FileUrl = fileUrl;
            model.FileName = model.Document.FileName;
            ViewBag.Message = "Файлът е качен успешно.";

            return View(model);
        }
    }
}
