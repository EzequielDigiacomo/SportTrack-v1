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

                string sql = "SELECT \"Id\", \"Nombre\", \"ParentClubId\" FROM catalogos.\"Clubes\" ORDER BY \"Id\";";
                
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using var reader = cmd.ExecuteReader();
                    Console.WriteLine("=== CLUBES EN RENDER ===");
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var nombre = reader.GetString(1);
                        var parentId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                        Console.WriteLine($"ID: {id}, Nombre: {nombre}, ParentId: {parentId}");
                    }
                    Console.WriteLine("========================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
