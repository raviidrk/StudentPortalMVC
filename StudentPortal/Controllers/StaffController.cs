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

      
            public async Task<IActionResult> Index()
            {
                var data = await _service.GetAllAsync();
                return View(data);
            }

            //  GET (Create/Edit)
            [HttpGet]
            public async Task<IActionResult> Upsert(int id)
            {
                if (id == 0)
                    return View(new StaffViewModel());

                var data = await _service.GetByIdAsync(id);

                var vm = new StaffViewModel
                {
                    Id = data.Id,
                    FullName = data.FullName,
                    UserName = data.UserName,
                    Role = data.Role,
                    PhoneNumber = data.PhoneNumber
                };

                return View(vm);
            }

            //  POST (Create/Update)
            [HttpPost]
            public async Task<IActionResult> Upsert(StaffViewModel model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                if (model.Id == 0)
                {
                    //  CREATE
                    var dto = new StaffResponseDto
                    {
                        FullName = model.FullName,
                        UserName = model.UserName,
                        Password = model.Password,
                        Role = model.Role,
                        PhoneNumber = model.PhoneNumber
                    };

                    await _service.CreateAsync(dto);
                }
                else
                {
                    //  UPDATE
                    var dto = new StaffResponseDto
                    {
                        Id = model.Id,
                        FullName = model.FullName,
                        Role = model.Role,
                        PhoneNumber = model.PhoneNumber
                    };

                    await _service.UpdateAsync(model.Id, dto);
                }

                return RedirectToAction("Index");
            }

            // DELETE
            public async Task<IActionResult> Delete(int id)
            {
                await _service.DeleteAsync(id);
                return RedirectToAction("Index");
            }
        }
    
}
