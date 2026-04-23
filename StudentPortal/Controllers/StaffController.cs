using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.DTO;
using StudentPortal.Models;
using StudentPortal.Service;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Admin,superadmin")]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // GET: /Staff/Index
        public async Task<IActionResult> Index()
        {
            var staffList = await _staffService.GetAllAsync();
            return View(staffList);

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
            try
            {
                var dto = new StaffResponseDto
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Password = model.Password,
                    Role = model.Role,
                    PhoneNumber = model.PhoneNumber
                };

                await _staffService.CreateAsync(dto);   
                TempData["Success"] = "Staff added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong: {ex.Message}");
                return View(model);
            }
        }

        // GET: /Staff/EditStaff/5
        [HttpGet]
        public async Task<IActionResult> EditStaff(int id)
        {

            var data = await _staffService.GetByIdAsync(id);   
            if (data == null) return NotFound();

            return View(data);

        }

        // POST: /Staff/EditStaff
        [HttpPost]
        
        public async Task<IActionResult> EditStaff(int id, StaffResponseDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {

                await _staffService.UpdateAsync(id, model);   
                TempData["Success"] = "Staff updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong: {ex.Message}");
                return View(model);
            }
        }

       
        [HttpPost]
        
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _staffService.DeleteAsync(id);   
                TempData["Success"] = "Staff deleted successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Delete failed: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}