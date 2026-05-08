START TRANSACTION;

ALTER TABLE regatas."Eventos" ADD "BotesHabilitados" text;

ALTER TABLE regatas."Eventos" ADD "CategoriasHabilitadas" text;

ALTER TABLE regatas."Eventos" ADD "DistanciasHabilitadas" text;

UPDATE catalogos."Categorias" SET "EdadMin" = 8
WHERE "Id" = 1;

UPDATE catalogos."Categorias" SET "EdadMax" = 12, "EdadMin" = 11
WHERE "Id" = 2;

UPDATE catalogos."Categorias" SET "EdadMax" = 14, "EdadMin" = 13
WHERE "Id" = 3;

UPDATE catalogos."Categorias" SET "EdadMax" = 16, "EdadMin" = 15
WHERE "Id" = 4;

UPDATE catalogos."Categorias" SET "EdadMax" = 18, "EdadMin" = 17
WHERE "Id" = 5;

UPDATE catalogos."Categorias" SET "EdadMax" = 23, "EdadMin" = 19
WHERE "Id" = 6;

UPDATE catalogos."Categorias" SET "EdadMin" = 19
WHERE "Id" = 7;

INSERT INTO catalogos."Categorias" ("Id", "EdadMax", "EdadMin", "Nombre")
VALUES (11, 99, 0, 'Control');

SELECT setval(
    pg_get_serial_sequence('catalogos."Categorias"', 'Id'),
    GREATEST(
        (SELECT MAX("Id") FROM catalogos."Categorias") + 1,
        nextval(pg_get_serial_sequence('catalogos."Categorias"', 'Id'))),
    false);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260508003834_AddHabilitacionesToEvento', '8.0.0');

COMMIT;

