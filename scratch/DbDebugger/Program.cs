using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=localhost;Port=5432;Database=SportTrackDB_Dev;Username=sporttrackdbadmin;Password=Admin2508";

        using var finalConn = new NpgsqlConnection(connString);
        finalConn.Open();

        Console.WriteLine("=== ASEGURANDO USUARIO SUPERADMIN ===");
        using (var cmd = new NpgsqlCommand("UPDATE seguridad.\"Usuarios\" SET \"Rol\" = 'SuperAdmin', \"Username\" = 'superadmin' WHERE \"Id\" = 1;", finalConn))
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Usuario con ID 1 actualizado a 'superadmin' con rol 'SuperAdmin'.");
        }

        Console.WriteLine("\n=== USUARIOS ===");
        using (var cmd = new NpgsqlCommand("SELECT \"Id\", \"Username\", \"Rol\", \"ClubId\" FROM seguridad.\"Usuarios\";", finalConn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var username = reader.GetString(1);
                var rol = reader.GetString(2);
                var clubId = reader.IsDBNull(3) ? "NULL" : reader.GetInt32(3).ToString();
                Console.WriteLine($"Id: {id}, Username: {username}, Rol: '{rol}', ClubId: {clubId}");
            }
        }

        Console.WriteLine("\n=== CLUBES ===");
        using (var cmd = new NpgsqlCommand("SELECT \"Id\", \"Nombre\", \"ParentClubId\" FROM catalogos.\"Clubes\";", finalConn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var nombre = reader.GetString(1);
                var parentId = reader.IsDBNull(2) ? "NULL" : reader.GetInt32(2).ToString();
                Console.WriteLine($"Id: {id}, Nombre: {nombre}, ParentClubId: {parentId}");
            }
        }

        Console.WriteLine("\n=== EVENTOS ===");
        using (var cmd = new NpgsqlCommand("SELECT \"Id\", \"Nombre\", \"ClubId\" FROM regatas.\"Eventos\";", finalConn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var nombre = reader.GetString(1);
                var clubId = reader.IsDBNull(2) ? "NULL" : reader.GetInt32(2).ToString();
                Console.WriteLine($"Id: {id}, Nombre: {nombre}, ClubId: {clubId}");
            }
        }
    }
}
