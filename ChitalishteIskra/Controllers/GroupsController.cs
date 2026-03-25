using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Groups;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Controllers
{
    public class GroupsController:Controller
    {
        private readonly IGroupService groupService;

        public GroupsController(IGroupService groupService)
        {
            this.groupService = groupService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await groupService.GetAllAsync();

            var model = data.Select(g => new GroupIndexViewModel
            {
                Id = g.Id,
                Name = g.Name
            });

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GroupCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new CreateGroupDto
            {
                Name = model.Name
            };

            await groupService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await groupService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            var model = new GroupEditViewModel
            {
                Id = data.Id,
                Name = data.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GroupEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new CreateGroupDto
            {
                Name = model.Name
            };

            await groupService.UpdateAsync(model.Id, dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await groupService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }




        public IActionResult ANPT() => View();
        public IActionResult Babsbg() => View();
        public IActionResult Iskritsa() => View();
        public IActionResult Sevtopolis() => View();
        public IActionResult Ekarte() => View();
        public IActionResult PetkoStainov() => View();
        public IActionResult DetskaTeatralna() => View();
        public IActionResult MladeshkaTeatralna() => View();
        public IActionResult ArtSchool() => View();
        public IActionResult Blue() => View();
        public IActionResult SportniTantsi() => View();
    }
}
