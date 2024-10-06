using MySqlConnector;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Models
{
    public class MariaDbContext
    {
        private readonly string _connectionString;

        public MariaDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
        }

        public async Task SaveIncidentForm(IncidentFormModel form)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "INSERT INTO incident_form (subject, uttrykning, something, attach_file, description, location_data) VALUES (@subject, @uttrykning, @something, @attach_file, @description, @location_data)",
                    connection);

                command.Parameters.AddWithValue("@subject", form.Subject);
                command.Parameters.AddWithValue("@uttrykning", form.Uttrykning);
                command.Parameters.AddWithValue("@something", form.Something);
                command.Parameters.AddWithValue("@attach_file", form.AttachFile);
                command.Parameters.AddWithValue("@description", form.Description);
                command.Parameters.AddWithValue("@location_data", form.GeoJson);  // Legger til GeoJSON-data

                await command.ExecuteNonQueryAsync();
            }
        }
    
        // Hent alle sakene fra incident_form-tabellen
        public async Task<List<IncidentFormModel>> GetAllIncidents()
        {
            var incidents = new List<IncidentFormModel>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT id, subject, uttrykning, something, attach_file, description, location_data FROM incident_form", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var incident = new IncidentFormModel
                        {
                            Id = reader.GetInt32("id"),
                            Subject = reader.GetString("subject"),
                            Uttrykning = reader.GetBoolean("uttrykning"),
                            Something = reader.GetBoolean("something"),
                            AttachFile = reader.GetBoolean("attach_file"),
                            Description = reader.GetString("description"),
                            GeoJson = reader.GetString("location_data")  // GeoJSON-data
                        };

                        incidents.Add(incident);
                    }
                }
            }

            return incidents;
        }
    }
}



