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
                    "INSERT INTO incident_form (subject, uttrykning, something, attach_file, description) VALUES (@subject, @uttrykning, @something, @attach_file, @description)",
                    connection);

                command.Parameters.AddWithValue("@subject", form.Subject);
                command.Parameters.AddWithValue("@uttrykning", form.Uttrykning);
                command.Parameters.AddWithValue("@something", form.Something);
                command.Parameters.AddWithValue("@attach_file", form.AttachFile);
                command.Parameters.AddWithValue("@description", form.Description);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
