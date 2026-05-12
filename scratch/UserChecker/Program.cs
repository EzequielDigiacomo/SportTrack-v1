using System;
using Npgsql;

namespace UserChecker
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

                using (var cmd = new NpgsqlCommand("SELECT \"Id\", \"Username\", \"Rol\", \"ClubId\", \"Activo\" FROM seguridad.\"Usuarios\";", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("ID | Username | Rol | ClubId | Activo");
                        Console.WriteLine("-------------------------------------");
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Id"]} | {reader["Username"]} | {reader["Rol"]} | {reader["ClubId"] ?? "NULL"} | {reader["Activo"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
