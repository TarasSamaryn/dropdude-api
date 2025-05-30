using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinefieldServer.Migrations
{
    public partial class RenameSkinFieldsToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Міняємо тип колонок із integer[] на text
            migrationBuilder.AlterColumn<string>(
                name: "HeadsSkins",
                table: "Players",
                type: "text",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");

            migrationBuilder.AlterColumn<string>(
                name: "BodiesSkins",
                table: "Players",
                type: "text",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");

            migrationBuilder.AlterColumn<string>(
                name: "LegsSkins",
                table: "Players",
                type: "text",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");

            migrationBuilder.AlterColumn<string>(
                name: "MasksSkins",
                table: "Players",
                type: "text",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");

            // 2. Конвертуємо старі масиви у формат "id[-count],..."
            migrationBuilder.Sql(@"
                UPDATE ""Players"" AS p
                SET ""HeadsSkins"" = (
                  SELECT string_agg(
                    CASE WHEN ct > 1 THEN key::text || '-' || ct::text ELSE key::text END,
                    ',' ORDER BY key
                  )
                  FROM (
                    SELECT key::int AS key, COUNT(*) AS ct
                    FROM unnest(string_to_array(trim(BOTH '{}' FROM p.""HeadsSkins""),',')) AS key
                    GROUP BY key
                  ) AS t(key, ct)
                );
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Players"" AS p
                SET ""BodiesSkins"" = (
                  SELECT string_agg(
                    CASE WHEN ct > 1 THEN key::text || '-' || ct::text ELSE key::text END,
                    ',' ORDER BY key
                  )
                  FROM (
                    SELECT key::int AS key, COUNT(*) AS ct
                    FROM unnest(string_to_array(trim(BOTH '{}' FROM p.""BodiesSkins""),',')) AS key
                    GROUP BY key
                  ) AS t(key, ct)
                );
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Players"" AS p
                SET ""LegsSkins"" = (
                  SELECT string_agg(
                    CASE WHEN ct > 1 THEN key::text || '-' || ct::text ELSE key::text END,
                    ',' ORDER BY key
                  )
                  FROM (
                    SELECT key::int AS key, COUNT(*) AS ct
                    FROM unnest(string_to_array(trim(BOTH '{}' FROM p.""LegsSkins""),',')) AS key
                    GROUP BY key
                  ) AS t(key, ct)
                );
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Players"" AS p
                SET ""MasksSkins"" = (
                  SELECT string_agg(
                    CASE WHEN ct > 1 THEN key::text || '-' || ct::text ELSE key::text END,
                    ',' ORDER BY key
                  )
                  FROM (
                    SELECT key::int AS key, COUNT(*) AS ct
                    FROM unnest(string_to_array(trim(BOTH '{}' FROM p.""MasksSkins""),',')) AS key
                    GROUP BY key
                  ) AS t(key, ct)
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // можна додати зворотні операції, якщо потрібно
        }
    }
}
