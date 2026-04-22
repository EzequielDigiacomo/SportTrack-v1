-- =============================================
-- SEED: 3 Clubes + 8 atletas por categoría (M/F)
-- =============================================

-- 3 Clubes nuevos (IDs 2, 3, 4 asumiendo que ya existe ID 1)
INSERT INTO catalogos."Clubes" ("Nombre", "Sigla", "Email", "Telefono", "Ubicacion")
VALUES
  ('Club Náutico Río Plata', 'CNRP', 'info@cnrp.com.ar', '011-4521-0001', 'Buenos Aires'),
  ('Club Atlético Paraná', 'CAP', 'info@cap.com.ar', '0343-4521-0002', 'Paraná, Entre Ríos'),
  ('Club Deportivo Tigre', 'CDT', 'info@cdt.com.ar', '011-4521-0003', 'Tigre, Buenos Aires')
ON CONFLICT ("Nombre") DO NOTHING;

-- =============================================
-- INSERTAR PARTICIPANTES
-- Categorías: 1=Pre-Infantil, 2=Infantil, 3=Menor, 4=Cadete, 5=Junior,
--             6=Sub-23, 7=Senior, 8=MasterA, 9=MasterB, 10=MasterC
-- Sexo: 1=Masculino, 2=Femenino
-- =============================================

DO $$
DECLARE
  club_id INT;
  club_names TEXT[] := ARRAY['Club Náutico Río Plata', 'Club Atlético Paraná', 'Club Deportivo Tigre'];
  club_prefix TEXT[] := ARRAY['CNRP', 'CAP', 'CDT'];

  -- Nombres masculinos
  nombres_m TEXT[] := ARRAY['Lucas','Matías','Agustín','Franco','Nicolás','Sebastián','Julián','Tomás'];
  -- Nombres femeninos
  nombres_f TEXT[] := ARRAY['Valentina','Camila','Sofía','Martina','Lucía','Florencia','Agustina','Emilia'];
  -- Apellidos
  apellidos TEXT[] := ARRAY['González','Rodríguez','Fernández','López','Martínez','Pérez','García','Díaz'];

  -- Categorías: (id, edad_base, rango)
  cat_ids INT[]   := ARRAY[1,2,3,4,5,6,7,8,9,10];
  cat_ages INT[]  := ARRAY[9,10,12,14,16,19,25,42,52,62]; -- edad representativa

  ci INT; -- club index
  ci2 INT; -- inner club
  cat_idx INT;
  p_idx INT;
  p_sexo INT;
  p_nombre TEXT;
  p_apellido TEXT;
  p_edad INT;
  p_fecha DATE;
  p_cat INT;
  p_email TEXT;
  p_dni TEXT;
  seq INT := 0;
BEGIN
  FOR ci IN 1..3 LOOP
    SELECT "Id" INTO club_id FROM catalogos."Clubes" WHERE "Nombre" = club_names[ci];

    FOR cat_idx IN 1..10 LOOP
      p_cat := cat_ids[cat_idx];
      p_edad := cat_ages[cat_idx];

      -- 8 Masculinos
      FOR p_idx IN 1..8 LOOP
        seq := seq + 1;
        p_nombre   := nombres_m[p_idx];
        p_apellido := apellidos[p_idx];
        p_fecha    := (CURRENT_DATE - (p_edad * 365 + p_idx * 30) * INTERVAL '1 day')::DATE;
        p_email    := lower(p_nombre) || '.' || lower(p_apellido) || seq::TEXT || '@test.com';
        p_dni      := (30000000 + seq)::TEXT;

        INSERT INTO regatas."Participantes"
          ("Nombre","Apellido","FechaNacimiento","SexoId","CategoriaId","ClubId","Pais","Dni","Email")
        VALUES
          (p_nombre, p_apellido, p_fecha, 1, p_cat, club_id, 'Argentina', p_dni, p_email)
        ON CONFLICT DO NOTHING;
      END LOOP;

      -- 8 Femeninos
      FOR p_idx IN 1..8 LOOP
        seq := seq + 1;
        p_nombre   := nombres_f[p_idx];
        p_apellido := apellidos[p_idx];
        p_fecha    := (CURRENT_DATE - (p_edad * 365 + p_idx * 30) * INTERVAL '1 day')::DATE;
        p_email    := lower(p_nombre) || '.' || lower(p_apellido) || seq::TEXT || '@test.com';
        p_dni      := (30000000 + seq)::TEXT;

        INSERT INTO regatas."Participantes"
          ("Nombre","Apellido","FechaNacimiento","SexoId","CategoriaId","ClubId","Pais","Dni","Email")
        VALUES
          (p_nombre, p_apellido, p_fecha, 2, p_cat, club_id, 'Argentina', p_dni, p_email)
        ON CONFLICT DO NOTHING;
      END LOOP;

    END LOOP;
  END LOOP;
END $$;

-- Verificación
SELECT c."Nombre" AS Club, COUNT(p."Id") AS Total_Atletas
FROM catalogos."Clubes" c
LEFT JOIN regatas."Participantes" p ON p."ClubId" = c."Id"
GROUP BY c."Nombre"
ORDER BY c."Nombre";
