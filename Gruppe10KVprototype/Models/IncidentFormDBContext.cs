using MySqlConnector;
using System.Threading.Tasks;
using Models;
using Models.InnmeldingModels;

namespace Gruppe10KVprototype.Models
{
    public class IncidentFormDBContext
    {
        private readonly string _connectionString;

        public IncidentFormDBContext(IConfiguration configuration)
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


    }
}



