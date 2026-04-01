using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Groups;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Controllers
{
    public class GroupsController:Controller
    {
        private readonly IGroupService groupService;
        private readonly UserManager<User> userManager;

        public GroupsController(IGroupService groupService, UserManager<User> userManager)
        {
            this.groupService = groupService;
            this.userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await groupService.GetAllAsync();

            var model = data.Select(g => new GroupIndexViewModel
            {
                Id = g.Id,
                Name = g.Name,
                TeacherId = g.TeacherId,
                TeacherName = g.TeacherName
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");

            ViewBag.Teachers = teachers
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.FirstName + " " + t.LastName
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GroupCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var teachers = await userManager.GetUsersInRoleAsync("Teacher");

                ViewBag.Teachers = teachers
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.FirstName + " " + t.LastName
                    })
                    .ToList();

                return View(model);
            }

            var dto = new CreateGroupDto
            {
                Name = model.Name,
                TeacherId = model.TeacherId
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

            var teachers = await userManager.GetUsersInRoleAsync("Teacher");

            ViewBag.Teachers = teachers
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.FirstName + " " + t.LastName
                })
                .ToList();

            var model = new GroupEditViewModel
            {
                Id = data.Id,
                Name = data.Name,
                TeacherId = data.TeacherId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GroupEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var teachers = await userManager.GetUsersInRoleAsync("Teacher");

                ViewBag.Teachers = teachers
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.FirstName + " " + t.LastName
                    })
                    .ToList();

                return View(model);
            }

            var dto = new CreateGroupDto
            {
                Name = model.Name,
                TeacherId = model.TeacherId
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
