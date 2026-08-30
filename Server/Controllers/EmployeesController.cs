using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using SharedLibrary.DTOs;
using SharedLibrary.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ServerContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeesController(ServerContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            var employee = await _context.Employees.Include(e => e.Experiences).ThenInclude(ex => ex.ExperienceTitle).ToListAsync();
            return Ok(employee);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.Include(e => e.Experiences).ThenInclude(ex => ex.ExperienceTitle).FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]

        //{"experienceTitleId": 1, "duration": 12},{ "experienceTitleId": 2,"duration": 15}


        public async Task<IActionResult> PostEmployee([FromForm] EmployeeDTO dto)
        {
            string fileName = "noimage.png";
            if (dto.ImageFile != null)
            {
                fileName = await SaveImageAsync(dto.ImageFile);
            }
            var employee = new Employee
            {
                EmployeeName = dto.EmployeeName,
                IsActive = dto.IsActive,
                JoinDate = dto.JoinDate,
                ImageName = fileName,
                ImageUrl = "/images/" + fileName
            };
            if (!string.IsNullOrWhiteSpace(dto.ExperiencesString))
            {
                try
                {
                    string rawJson = dto.ExperiencesString.Trim();
                    if (rawJson.StartsWith("{") && rawJson.EndsWith("}"))
                    {
                        rawJson = $"[{rawJson}]";
                    }

                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    var experiences = System.Text.Json.JsonSerializer.Deserialize<List<ExperienceDTO>>(rawJson, options);

                    if (experiences != null)

                    {
                        foreach (var item in experiences)
                        {
                            var titleExists = await _context.ExperiencesTitles.AnyAsync(t => t.ExperienceTitleId == item.ExperienceTitleId);
                            if (!titleExists)
                            {
                                return BadRequest("Experience title not exists");
                            }
                            employee.Experiences.Add(new Experience
                            {
                                ExperienceTitleId = item.ExperienceTitleId,
                                Duration = item.Duration,
                            });
                        }

                    }
                }
                catch (System.Text.Json.JsonException JX)
                {

                    return BadRequest($"{JX.Message}");
                }

            }
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string uploadDir = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return filePath;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            var imagePath = Path.Combine(_env.WebRootPath, "images", employee.ImageName ?? "");
            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);   
            }
          _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok("employee");
        }
    }
}
