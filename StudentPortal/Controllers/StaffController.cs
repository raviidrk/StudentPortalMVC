using Microsoft.AspNetCore.Mvc;
using StudentPortal.DTO;
using StudentPortal.Models;
using StudentPortal.Service;

namespace StudentPortal.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _service;

        public StaffController(IStaffService service)
        {
            _service = service;
        }

        // GET: /Staff/Index
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        // GET: /Staff/AddStaff
        [HttpGet]
        public IActionResult AddStaff()
        {
            return View(new StaffViewModel());
        }

        // POST: /Staff/AddStaff
        [HttpPost]
        public async Task<IActionResult> AddStaff(StaffViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new StaffResponseDto
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Password = model.Password,
                Role = model.Role,
                PhoneNumber = model.PhoneNumber
            };

            await _service.CreateAsync(dto);
            return RedirectToAction("Index");
        }

        // GET: /Staff/EditStaff/5
        [HttpGet]
        public async Task<IActionResult> EditStaff(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();

            
            return View(data);
        }

        // POST: /Staff/EditStaff
        [HttpPost]
        public async Task<IActionResult> EditStaff(StaffResponseDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

           

            await _service.UpdateAsync(model.Id, model);
            return RedirectToAction("Index");
        }

        // GET: /Staff/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}