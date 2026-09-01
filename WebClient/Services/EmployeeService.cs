using SharedLibrary.DTOs;
using SharedLibrary.Models;

namespace WebClient.Services
{
    public class EmployeeService
    {
        private readonly HttpClient _http;

        public EmployeeService(HttpClient http)
        {
            _http = http;
        }
        public async Task<List<Employee>> GetEmployeesAsync() => await _http.GetFromJsonAsync<List<Employee>>("api/employees") ?? new();
        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/employees/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<Employee?> GetEmployeeByIdAsync(int id) => await _http.GetFromJsonAsync<Employee>($"api/employees/{id}");

        public async Task<bool> PostEmployeeAsync(EmployeeDTO dto)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.EmployeeName), "EmployeeName");
            content.Add(new StringContent(dto.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(dto.JoinDate.ToString("yyyy-MM-dd")), "JoinDate");
            content.Add(new StringContent(dto.ExperiencesString), "ExperiencesString");
            if (dto.ImageFile != null)
            {
                var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                content.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
            }
            var response = await _http.PostAsync("api/employees", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<ExperienceTitle>> GetExperienceTitlesAsync()
        {
            var titles = await _http.GetFromJsonAsync<List<ExperienceTitle>>("api/employees/titles");
            return titles ?? new List<ExperienceTitle>();
        }

        public async Task<bool> PutEmployeeAsync(int id, EmployeeDTO dto)
        {
            dto.EmployeeId = id;
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.EmployeeId.ToString()), "EmployeeId");
            content.Add(new StringContent(dto.EmployeeName), "EmployeeName" ?? "");
            content.Add(new StringContent(dto.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(dto.JoinDate.ToString("yyyy-MM-dd")), "JoinDate");
            content.Add(new StringContent(dto.ExperiencesString ?? "[]"), "ExperiencesString");
            if (dto.ImageFile != null)
            {
                var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(dto.ImageFile.ContentType);
                content.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
            }
            else
            {
                content.Add(new StringContent(dto.ImageUrl ?? ""), "ImageUrl");
            }
            var response = await _http.PutAsync($"api/employees/{id}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
