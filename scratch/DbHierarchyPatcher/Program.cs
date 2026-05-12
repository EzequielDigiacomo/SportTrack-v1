using System;
using Npgsql;

namespace DbPatcher
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

                // 1. Crear la columna ParentClubId
                using (var cmd = new NpgsqlCommand("ALTER TABLE catalogos.\"Clubes\" ADD COLUMN IF NOT EXISTS \"ParentClubId\" INTEGER NULL;", conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Columna ParentClubId creada.");
                }

                // 2. Agregar la restricción de FK
                try {
                    using var cmdFk = new NpgsqlCommand("ALTER TABLE catalogos.\"Clubes\" ADD CONSTRAINT \"FK_Clubes_Parent\" FOREIGN KEY (\"ParentClubId\") REFERENCES catalogos.\"Clubes\"(\"Id\") ON DELETE RESTRICT;", conn);
                    cmdFk.ExecuteNonQuery();
                    Console.WriteLine("Constraint FK agregada.");
                } catch {
                    Console.WriteLine("El constraint ya existía o hubo un error menor al crearlo.");
                }

                // 3. Mover los clubes a la Federación 1
                // Excluimos la Federación 1 misma (ID 1) y la nueva Federación 2 (ID 11)
                using (var cmdMove = new NpgsqlCommand("UPDATE catalogos.\"Clubes\" SET \"ParentClubId\" = 1 WHERE \"Id\" NOT IN (1, 11) AND \"ParentClubId\" IS NULL;", conn))
                {
                    int rows = cmdMove.ExecuteNonQuery();
                    Console.WriteLine($"Se movieron {rows} clubes bajo la Federación 1.");
                }

                Console.WriteLine("Parche de base de datos completado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
