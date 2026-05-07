-- =============================================
-- SCRIPT PARA CREAR 3 NUEVOS CLUBES Y ATLETAS
-- 4 Masculinos y 4 Femeninos por categoría en cada club
-- =============================================

DO $$
DECLARE
  -- CONFIGURACIÓN DE LOS CLUBES
  club_names TEXT[]  := ARRAY['Club de Regatas Rosario', 'Mar del Plata Rowing Club', 'Club Náutico Hacoaj'];
  club_siglas TEXT[] := ARRAY['CRR', 'MPRC', 'CNH'];
  club_emails TEXT[] := ARRAY['secretaria@crr.com.ar', 'info@mprc.org.ar', 'deportes@hacoaj.org.ar'];
  club_tels TEXT[]   := ARRAY['0341-435-0011', '0223-480-1234', '011-4749-0099'];
  club_locs TEXT[]   := ARRAY['Rosario, Santa Fe', 'Mar del Plata, Buenos Aires', 'Tigre, Buenos Aires'];

  -- Variables de control
  club_id INT;
  c_idx INT;
  
  -- Listas expandidas para generar nombres aleatorios
  nombres_m TEXT[] := ARRAY['Lucas','Matías','Agustín','Franco','Nicolás','Sebastián','Julián','Tomás','Bautista','Felipe','Santiago','Mateo','Enzo','Joaquín','Lautaro','Benjamín','Ignacio','Ramiro','Valentino','Gaspar'];
  nombres_f TEXT[] := ARRAY['Valentina','Camila','Sofía','Martina','Lucía','Florencia','Agustina','Emilia','Delfina','Victoria','Mia','Juana','Catalina','Elena','Abril','Morena','Zoe','Lola','Bianca','Isabella'];
  apellidos TEXT[] := ARRAY['González','Rodríguez','Fernández','López','Martínez','Pérez','García','Díaz','Sánchez','Romero','Sosa','Álvarez','Torres','Ruiz','Ramírez','Flores','Benítez','Acosta','Medina','Herrera'];

  -- Categorías y Edades (basadas en CategoriaEdadEnum)
  cat_ids INT[]   := ARRAY[1,2,3,4,5,6,7,8,9,10];
  cat_ages INT[]  := ARRAY[9,11,13,15,17,21,28,42,48,55]; 

  cat_idx INT;
  p_idx INT;
  p_nombre TEXT;
  p_apellido TEXT;
  p_fecha DATE;
  p_dni TEXT;
  p_email TEXT;
  p_cat INT;
  p_edad INT;
  seq INT := 0;

BEGIN
  -- 1. Iterar sobre los Clubes
  FOR c_idx IN 1..3 LOOP
    
    INSERT INTO catalogos."Clubes" ("Nombre", "Sigla", "Email", "Telefono", "Ubicacion")
    VALUES (club_names[c_idx], club_siglas[c_idx], club_emails[c_idx], club_tels[c_idx], club_locs[c_idx])
    ON CONFLICT ("Nombre") DO UPDATE SET "Nombre" = EXCLUDED."Nombre"
    RETURNING "Id" INTO club_id;

    RAISE NOTICE 'Procesando club: % (ID: %)', club_names[c_idx], club_id;

    -- 2. Iterar por Categorías
    FOR cat_idx IN 1..10 LOOP
      p_cat := cat_ids[cat_idx];
      p_edad := cat_ages[cat_idx];

      -- 4 Masculinos (SexoId = 1)
      FOR p_idx IN 1..4 LOOP
        seq := seq + 1;
        -- Selección circular de nombres para evitar repeticiones simples
        p_nombre   := nombres_m[(1 + ((seq + c_idx) % array_length(nombres_m, 1)))];
        p_apellido := apellidos[(1 + ((seq * c_idx) % array_length(apellidos, 1)))];
        
        p_fecha    := (CURRENT_DATE - (p_edad * 365 + (cat_idx * 7) + (p_idx * 3) + c_idx) * INTERVAL '1 day')::DATE;
        p_dni      := (50000000 + (club_id * 10000) + seq)::TEXT;
        p_email    := lower(p_nombre) || '.' || lower(p_apellido) || seq::TEXT || '@' || lower(club_siglas[c_idx]) || '.test';

        INSERT INTO regatas."Participantes"
          ("Nombre", "Apellido", "FechaNacimiento", "SexoId", "CategoriaId", "ClubId", "Pais", "Dni", "Email")
        VALUES
          (p_nombre, p_apellido, p_fecha, 1, p_cat, club_id, 'Argentina', p_dni, p_email)
        ON CONFLICT DO NOTHING;
      END LOOP;

      -- 4 Femeninos (SexoId = 2)
      FOR p_idx IN 1..4 LOOP
        seq := seq + 1;
        p_nombre   := nombres_f[(1 + ((seq + c_idx) % array_length(nombres_f, 1)))];
        p_apellido := apellidos[(1 + ((seq * c_idx) % array_length(apellidos, 1)))];
        
        p_fecha    := (CURRENT_DATE - (p_edad * 365 + (cat_idx * 7) + (p_idx * 3) + c_idx) * INTERVAL '1 day')::DATE;
        p_dni      := (50000000 + (club_id * 10000) + seq)::TEXT;
        p_email    := lower(p_nombre) || '.' || lower(p_apellido) || seq::TEXT || '@' || lower(club_siglas[c_idx]) || '.test';

        INSERT INTO regatas."Participantes"
          ("Nombre", "Apellido", "FechaNacimiento", "SexoId", "CategoriaId", "ClubId", "Pais", "Dni", "Email")
        VALUES
          (p_nombre, p_apellido, p_fecha, 2, p_cat, club_id, 'Argentina', p_dni, p_email)
        ON CONFLICT DO NOTHING;
      END LOOP;
    END LOOP;
  END LOOP;

  RAISE NOTICE 'Proceso completado. Se han procesado % atletas en total.', seq;
END $$;

-- Verificación final
SELECT 
    c."Nombre" as Club,
    cat."Nombre" as Categoria,
    s."Nombre" as Sexo,
    COUNT(p."Id") as Cantidad
FROM regatas."Participantes" p
JOIN catalogos."Clubes" c ON p."ClubId" = c."Id"
JOIN catalogos."Categorias" cat ON p."CategoriaId" = cat."Id"
JOIN catalogos."Sexos" s ON p."SexoId" = s."Id"
WHERE c."Sigla" IN ('CRR', 'MPRC', 'CNH')
GROUP BY c."Nombre", cat."Nombre", s."Nombre", cat."Id"
ORDER BY c."Nombre", cat."Id", s."Nombre";
