using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=dpg-d7ih1pd7vvec73abm5sg-a.virginia-postgres.render.com;Port=5432;Database=db_sporttrack;Username=db_sporttrack_user;Password=WGOFP4V6h11iv2GXhWZ4SLViVfyr8Ljf;SSL Mode=Require;Trust Server Certificate=true";
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            Console.WriteLine("Conexión exitosa.");

            using var cmd = new NpgsqlCommand("SELECT \"Id\", \"Nombre\", \"Activo\" FROM catalogos.\"Clubes\";", conn);
            using var reader = cmd.ExecuteReader();
            Console.WriteLine("Clubes en Render:");
            while (reader.Read())
            {
                Console.WriteLine($"- ID: {reader.GetInt32(0)}, Nombre: {reader.GetString(1)}, Activo: {reader.GetBoolean(2)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
