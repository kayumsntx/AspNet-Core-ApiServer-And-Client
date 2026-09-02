using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;
using System.Text.Json;
using WebClient.Services;

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
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var titles = await service.GetExperienceTitlesAsync();
        ViewBag.ExperienceTitles = titles;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeDTO dto, List<ExperienceDTO> expList)
    {
        dto.ExperiencesString = System.Text.Json.JsonSerializer.Serialize(expList);
        var success = await service.PostEmployeeAsync(dto);
        if (success) return RedirectToAction(nameof(Index));
        return View(dto);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await service.GetEmployeeByIdAsync(id);
        if (employee == null) return NotFound();
        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };
        var dto = new EmployeeDTO
        {
            EmployeeId = employee.EmployeeId,
            EmployeeName = employee.EmployeeName,
            IsActive = employee.IsActive,
            JoinDate = employee.JoinDate,
            ImageUrl = employee.ImageUrl,
            ExperiencesString = JsonSerializer.Serialize(employee.Experiences.Select(x => new
            {
                ExperienceTitleId = x.ExperienceTitleId,
                Duration = x.Duration
            }), options)
        };
       
        ViewBag.ExperienceTitles = await service.GetExperienceTitlesAsync();
        return View(dto);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, EmployeeDTO dto, List<ExperienceDTO> expList)
    {
        if (id != dto.EmployeeId) return BadRequest();
        dto.ExperiencesString = System.Text.Json.JsonSerializer.Serialize(expList);
        var success = await service.PutEmployeeAsync(id, dto);
        if (success) return RedirectToAction(nameof(Index));
        return View(dto);
    }
}