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
    }
}
