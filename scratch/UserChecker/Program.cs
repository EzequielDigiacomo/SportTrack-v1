using System;
using Npgsql;

namespace UserLinker
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Host=dpg-d7ih1pd7vvec73abm5sg-a.virginia-postgres.render.com;Username=db_sporttrack_user;Password=WGOFP4V6h11iv2GXhWZ4SLViVfyr8Ljf;Database=db_sporttrack;SSL Mode=Require;Trust Server Certificate=true";
            
            try 
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("Conectado a Render...");

                // Vinculamos a los administradores sueltos a la Federación 1 (ID 1)
                string sql = "UPDATE seguridad.\"Usuarios\" SET \"ClubId\" = 1 WHERE \"Username\" IN ('admin', 'ezequiel_admin') AND \"ClubId\" IS NULL;";
                
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Se vincularon {rows} administradores a la Federación 1.");
                }

                Console.WriteLine("Vinculación de usuarios completada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
