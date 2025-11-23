using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicznikiIndeksow",
                columns: table => new
                {
                    Prefix = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AktualnaWartosc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicznikiIndeksow", x => x.Prefix);
                });

            migrationBuilder.CreateTable(
                name: "Wydzialy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wydzialy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profesorzy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndeksUczelniany = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TytulNaukowy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresZamieszkania_Id = table.Column<int>(type: "int", nullable: false),
                    AdresZamieszkania_Ulica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresZamieszkania_Miasto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresZamieszkania_KodPocztowy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WydzialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesorzy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profesorzy_Wydzialy_WydzialId",
                        column: x => x.WydzialId,
                        principalTable: "Wydzialy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gabinety",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumerGabinetu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfesorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gabinety", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gabinety_Profesorzy_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesorzy",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Kursy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ECTS = table.Column<int>(type: "int", nullable: false),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    WydzialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kursy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kursy_Profesorzy_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesorzy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kursy_Wydzialy_WydzialId",
                        column: x => x.WydzialId,
                        principalTable: "Wydzialy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Studenci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndeksUczelniany = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RokStudiow = table.Column<int>(type: "int", nullable: false),
                    AdresZamieszkania_Id = table.Column<int>(type: "int", nullable: false),
                    AdresZamieszkania_Ulica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresZamieszkania_Miasto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresZamieszkania_KodPocztowy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    TematPracyDyplomowej = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromotorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studenci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Studenci_Profesorzy_PromotorId",
                        column: x => x.PromotorId,
                        principalTable: "Profesorzy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KursPrerequisite",
                columns: table => new
                {
                    KursId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KursPrerequisite", x => new { x.KursId, x.PrerequisiteId });
                    table.ForeignKey(
                        name: "FK_KursPrerequisite_Kursy_KursId",
                        column: x => x.KursId,
                        principalTable: "Kursy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KursPrerequisite_Kursy_PrerequisiteId",
                        column: x => x.PrerequisiteId,
                        principalTable: "Kursy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollmenty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Semestr = table.Column<int>(type: "int", nullable: false),
                    Ocena = table.Column<double>(type: "float", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    KursId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollmenty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollmenty_Kursy_KursId",
                        column: x => x.KursId,
                        principalTable: "Kursy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollmenty_Studenci_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Studenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollmenty_KursId",
                table: "Enrollmenty",
                column: "KursId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollmenty_StudentId",
                table: "Enrollmenty",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Gabinety_ProfesorId",
                table: "Gabinety",
                column: "ProfesorId",
                unique: true,
                filter: "[ProfesorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KursPrerequisite_PrerequisiteId",
                table: "KursPrerequisite",
                column: "PrerequisiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Kursy_ProfesorId",
                table: "Kursy",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Kursy_WydzialId",
                table: "Kursy",
                column: "WydzialId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesorzy_IndeksUczelniany",
                table: "Profesorzy",
                column: "IndeksUczelniany",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profesorzy_WydzialId",
                table: "Profesorzy",
                column: "WydzialId");

            migrationBuilder.CreateIndex(
                name: "IX_Studenci_IndeksUczelniany",
                table: "Studenci",
                column: "IndeksUczelniany",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Studenci_PromotorId",
                table: "Studenci",
                column: "PromotorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollmenty");

            migrationBuilder.DropTable(
                name: "Gabinety");

            migrationBuilder.DropTable(
                name: "KursPrerequisite");

            migrationBuilder.DropTable(
                name: "LicznikiIndeksow");

            migrationBuilder.DropTable(
                name: "Studenci");

            migrationBuilder.DropTable(
                name: "Kursy");

            migrationBuilder.DropTable(
                name: "Profesorzy");

            migrationBuilder.DropTable(
                name: "Wydzialy");
        }
    }
}
