using Microsoft.Extensions.Configuration;
using Models.SaksbehandlerModels;
using MySqlConnector;

namespace Models.DbContexts;

public class AdviserFormDBContext

{
    private readonly string _connectionString;

    public AdviserFormDBContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
    }


    // Hent alle sakene fra incident_form-tabellen
    public async Task<List<AdviserFormModel>> GetAllIncidents()
    {
        var incidents = new List<AdviserFormModel>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT id, subject, uttrykning, something, attach_file, description, location_data FROM incident_form", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var incident = new AdviserFormModel
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

    public async Task<AdviserFormModel?> GetIncidentById(int id)
    {
        AdviserFormModel? incident = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT id, subject, uttrykning, something, attach_file, description, location_data FROM incident_form WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    incident = new AdviserFormModel
                    {
                        Id = reader.GetInt32("id"),
                        Subject = reader.GetString("subject"),
                        Uttrykning = reader.GetBoolean("uttrykning"),
                        Something = reader.GetBoolean("something"),
                        AttachFile = reader.GetBoolean("attach_file"),
                        Description = reader.GetString("description"),
                        GeoJson = reader.GetString("location_data")
                    };
                }
            }
        }

        return incident;
    }


}
