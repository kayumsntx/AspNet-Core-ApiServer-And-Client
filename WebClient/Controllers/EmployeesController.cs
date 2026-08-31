using Microsoft.AspNetCore.Mvc;
using WebClient.Services;

namespace WebClient.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeService service;

        public EmployeesController(EmployeeService service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await service.GetEmployeesAsync();
            return View(employees);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await service.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            var success = await service.DeleteEmployeeAsync(id);
            if (success) return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Delete), new { id, error = "Could not delete" });
        }
    }
}
