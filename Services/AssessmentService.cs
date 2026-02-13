using Assessment.Interfaces;
using Assessment.Modals;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Text.Json;

namespace Assessment.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ILogger<AssessmentService> _logger;
        private readonly string _connectionString;
        private readonly IHttpClientFactory _httpClientFactory;

        public AssessmentService(ILogger<AssessmentService> logger, string connectionString, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
        }
        /// <summary>
        /// Get single record by id. If not found, fetch from 3rd party API, save, then return.
        /// </summary>
        public async Task<InformationDto?> GetByIdAsync(int id)
        {
            InformationDto? info = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT UserId, Name, Email, Date FROM Information WHERE UserId = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        info = new InformationDto
                        {
                            UserId = reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Date = reader.GetDateTime(3)
                        };
                    }
                }
            }
            if (info != null)
                return info;

            // Fetch from 3rd party API
            var client = _httpClientFactory.CreateClient();

            // Retrieve API key from environment variable
            var apiKey = Environment.GetEnvironmentVariable("THIRD_PARTY_API_KEY");
            // If the API requires a header, add it (example: Authorization)
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            var response = await client.GetAsync($"https://jsonplaceholder.typicode.com/users/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var extUser = JsonSerializer.Deserialize<JsonPlaceholderUser>(json);
                if (extUser == null)
                    return null;

                var newInfo = new InformationDto
                {
                    UserId = extUser.id,
                    Name = extUser.name,
                    Email = extUser.email,
                    Date = DateTime.Now
                };
                // Save to DB
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand("INSERT INTO Information (UserId, Name, Email, Date) VALUES (@UserId, @Name, @Email, @Date)", conn);
                    cmd.Parameters.AddWithValue("@UserId", newInfo.UserId);
                    cmd.Parameters.AddWithValue("@Name", (object?)newInfo.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)newInfo.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", newInfo.Date);
                    await cmd.ExecuteNonQueryAsync();
                }
                return newInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Update record by id.
        /// </summary>
        public async Task<bool> UpdateAsync(int id, InformationDto updated)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE Informations SET Name = @Name, Email = @Email, Date = @Date WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@Name", (object?)updated.Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)updated.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", updated.Date);
                cmd.Parameters.AddWithValue("@UserId", id);
                var rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }

        /// <summary>
        /// Get all Information records from the database.
        /// </summary>
        public async Task<List<InformationDto>> GetAllAsync()
        {
            var result = new List<InformationDto>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT UserId, Name, Email, Date FROM Information", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var info = new InformationDto
                        {
                            UserId = reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Date = reader.GetDateTime(3)
                        };
                        result.Add(info);
                    }
                }
            }
            return result;
        }

    }
}
