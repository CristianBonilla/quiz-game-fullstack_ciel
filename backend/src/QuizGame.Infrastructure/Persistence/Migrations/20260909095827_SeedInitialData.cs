using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuizGame.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdempotencyRecords",
                schema: "dbo",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SerializedResponse = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRecords", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Categories",
                columns: new[] { "Id", "Description", "DifficultyLevel", "IsActive", "Name", "PrizeAmount" },
                values: new object[,]
                {
                    { new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"), "Advanced data pipelines and machine learning questions for the final rounds.", 3, true, "Data & AI Engineering", 2000m },
                    { new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"), "Architectural styles, design patterns and system design questions.", 2, true, "Software Architecture", 500m },
                    { new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"), "Foundational programming and tooling questions.", 1, true, "Software Development", 100m }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Questions",
                columns: new[] { "Id", "CategoryId", "IsActive", "Text" },
                values: new object[,]
                {
                    { new Guid("095aa9ae-8d60-46a1-bba6-78e9fa6c86e7"), new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"), true, "¿Qué estructura de datos sigue el principio \"primero en entrar, primero en salir\" (FIFO)?" },
                    { new Guid("21e727f6-2da0-401a-9211-6ca24a85648f"), new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"), true, "¿Qué principio SOLID establece que una clase debe tener una única razón para cambiar?" },
                    { new Guid("2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac"), new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"), true, "¿Cuál de las siguientes opciones es un sistema de control de versiones?" },
                    { new Guid("3c933504-e478-4984-9044-365bd27ccb6c"), new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"), true, "¿Cuál de los siguientes es un principio de los sistemas orientados a microservicios?" },
                    { new Guid("3fa633a4-6979-45a9-a589-cf8c2754b6ec"), new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"), true, "¿Qué patrón arquitectónico separa una aplicación en Modelo, Vista y Controlador?" },
                    { new Guid("41905891-a86f-44f1-b8f4-3bdc12105fc0"), new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"), true, "¿Qué estilo arquitectónico expone recursos mediante URIs y verbos HTTP?" },
                    { new Guid("427c79c8-d4b8-48c4-9308-b7eb1ddf140e"), new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"), true, "¿Qué significa la sigla SQL?" },
                    { new Guid("45080bdc-002e-40b3-9027-15e764ff5c63"), new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"), true, "¿Qué métrica evalúa la proporción de verdaderos positivos sobre el total de positivos predichos por un modelo de clasificación?" },
                    { new Guid("4917bb8d-f18b-4afa-b437-4da489b75567"), new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"), true, "¿Qué herramienta se utiliza comúnmente para orquestar flujos de trabajo de datos (pipelines) por lotes?" },
                    { new Guid("5d34c73c-1f5a-4685-ad54-bee58fbf677d"), new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"), true, "¿Qué técnica de aprendizaje automático ajusta los pesos de una red neuronal usando el gradiente del error?" },
                    { new Guid("7992d28c-4399-4180-a957-8755f76e1184"), new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"), true, "¿Qué lenguaje se usa principalmente para dar estilo a las páginas web?" },
                    { new Guid("7d196c04-6a14-4946-959a-17166e0ad1f2"), new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"), true, "¿Qué proceso describe la extracción, transformación y carga de datos entre sistemas?" },
                    { new Guid("93a07525-cd78-42dd-bead-420e1cf7db0e"), new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"), true, "¿Qué patrón de diseño garantiza que una clase tenga una única instancia global?" },
                    { new Guid("990bd4b0-5e85-497e-a19e-2d4ef3d1263e"), new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"), true, "¿Qué componente se utiliza típicamente para distribuir la carga entre varias instancias de un servicio?" },
                    { new Guid("a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed"), new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"), true, "¿Qué arquitectura de red neuronal se destaca por su uso del mecanismo de atención y es la base de los modelos de lenguaje modernos?" },
                    { new Guid("a4999ba8-15f3-4585-844a-0f0e1f17dcee"), new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"), true, "¿Cuál de estas opciones es un tipo de dato primitivo en la mayoría de lenguajes de programación?" },
                    { new Guid("b85875b3-e141-4392-9447-58d2f2efbdd9"), new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"), true, "¿Qué tipo de base de datos está optimizada para el almacenamiento y consulta analítica de grandes volúmenes de datos en columnas?" },
                    { new Guid("bca1ad0d-c8e0-48f7-a83e-39d71540c206"), new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"), true, "¿Qué significa la sigla API?" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Answers",
                columns: new[] { "Id", "IsCorrect", "QuestionId", "Text" },
                values: new object[,]
                {
                    { new Guid("02e173bd-e815-412b-8e4a-c1a29ed15933"), false, new Guid("7d196c04-6a14-4946-959a-17166e0ad1f2"), "CRUD" },
                    { new Guid("0551a798-9577-4fc9-81fd-0fdf6907720f"), false, new Guid("93a07525-cd78-42dd-bead-420e1cf7db0e"), "Factory" },
                    { new Guid("0709943d-63f7-4d98-b4e7-89a62c8c690c"), false, new Guid("427c79c8-d4b8-48c4-9308-b7eb1ddf140e"), "Sistema de Consultas Lineales" },
                    { new Guid("0c62b880-f2b6-49c3-ae54-549aed98786e"), false, new Guid("095aa9ae-8d60-46a1-bba6-78e9fa6c86e7"), "Pila" },
                    { new Guid("0c7c8a6e-68ec-4859-b8ab-122c3d2e0f3c"), false, new Guid("21e727f6-2da0-401a-9211-6ca24a85648f"), "Sustitución de Liskov" },
                    { new Guid("0f37d8d4-e79f-46d6-9dd1-3a1bb07c199a"), false, new Guid("a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed"), "Máquina de vectores de soporte" },
                    { new Guid("14b94f04-b857-46e3-bc6f-8f37b237f363"), false, new Guid("095aa9ae-8d60-46a1-bba6-78e9fa6c86e7"), "Árbol" },
                    { new Guid("197fafb0-891c-404b-a5f8-4ce22e22f7d3"), true, new Guid("93a07525-cd78-42dd-bead-420e1cf7db0e"), "Singleton" },
                    { new Guid("1cb66463-8ecd-403f-af9b-9df36c213021"), false, new Guid("3fa633a4-6979-45a9-a589-cf8c2754b6ec"), "SOAP" },
                    { new Guid("206b1a69-23b1-424b-95a7-c744fa847946"), true, new Guid("5d34c73c-1f5a-4685-ad54-bee58fbf677d"), "Retropropagación" },
                    { new Guid("2246117f-ffef-47d2-87e7-6c6e95485867"), false, new Guid("41905891-a86f-44f1-b8f4-3bdc12105fc0"), "FTP" },
                    { new Guid("224fab6e-24cb-4902-a89a-bc02cf2dfe3d"), false, new Guid("45080bdc-002e-40b3-9027-15e764ff5c63"), "Latencia" },
                    { new Guid("26f60334-ec80-444b-9099-514204cdac89"), false, new Guid("3fa633a4-6979-45a9-a589-cf8c2754b6ec"), "REST" },
                    { new Guid("3145ff07-0517-49a6-bb02-96e59b490d49"), false, new Guid("4917bb8d-f18b-4afa-b437-4da489b75567"), "NGINX" },
                    { new Guid("32a71eb0-394b-4411-b995-bb8204edf856"), true, new Guid("095aa9ae-8d60-46a1-bba6-78e9fa6c86e7"), "Cola" },
                    { new Guid("3560622c-6851-43dd-962c-ce8c59d41d85"), false, new Guid("93a07525-cd78-42dd-bead-420e1cf7db0e"), "Decorator" },
                    { new Guid("357fd4f3-28a7-42d9-8ed3-9eeea8b3651f"), true, new Guid("3fa633a4-6979-45a9-a589-cf8c2754b6ec"), "MVC" },
                    { new Guid("39ec0665-b326-4be1-b7fd-6d55fef959c1"), false, new Guid("5d34c73c-1f5a-4685-ad54-bee58fbf677d"), "Poda de árboles" },
                    { new Guid("3b60fd8b-7b44-46a4-89a4-6771d4ec1995"), false, new Guid("7d196c04-6a14-4946-959a-17166e0ad1f2"), "CI/CD" },
                    { new Guid("3fa01924-ac0c-4276-90ab-a6c3407aa943"), true, new Guid("41905891-a86f-44f1-b8f4-3bdc12105fc0"), "REST" },
                    { new Guid("46f12667-a4bd-4183-bd36-d24c7c84e068"), false, new Guid("a4999ba8-15f3-4585-844a-0f0e1f17dcee"), "Arreglo" },
                    { new Guid("493e677c-02e7-44ee-b46f-41326ec5bcce"), true, new Guid("7d196c04-6a14-4946-959a-17166e0ad1f2"), "ETL" },
                    { new Guid("4aef8f31-4732-4fd0-9205-47ae7c4c6d7e"), false, new Guid("41905891-a86f-44f1-b8f4-3bdc12105fc0"), "SOAP" },
                    { new Guid("4c8d96ab-3e97-4d49-bc59-a404d215ada1"), false, new Guid("5d34c73c-1f5a-4685-ad54-bee58fbf677d"), "Validación cruzada" },
                    { new Guid("58155e79-65f0-4ecb-9f0c-bc278f2169a3"), false, new Guid("a4999ba8-15f3-4585-844a-0f0e1f17dcee"), "Objeto" },
                    { new Guid("583d0f0b-40f5-4a1f-b199-f8b78d60a099"), true, new Guid("990bd4b0-5e85-497e-a19e-2d4ef3d1263e"), "Balanceador de carga" },
                    { new Guid("5b97921c-55f9-49f4-80cf-32240dffb935"), false, new Guid("a4999ba8-15f3-4585-844a-0f0e1f17dcee"), "Clase" },
                    { new Guid("5d13aff7-6d70-4707-a825-408ccc728844"), true, new Guid("a4999ba8-15f3-4585-844a-0f0e1f17dcee"), "Booleano" },
                    { new Guid("65e31c46-cb67-4d38-9dc2-695322fc953a"), false, new Guid("990bd4b0-5e85-497e-a19e-2d4ef3d1263e"), "Caché local" },
                    { new Guid("6a740a2b-f51a-4927-bc7a-e4e65d875225"), false, new Guid("bca1ad0d-c8e0-48f7-a83e-39d71540c206"), "Aplicación de Procesos Internos" },
                    { new Guid("6c409052-d21b-4c9b-90a5-e85c76fda3eb"), false, new Guid("7992d28c-4399-4180-a957-8755f76e1184"), "HTML" },
                    { new Guid("6df053aa-fd38-4a71-afd0-28a320ac566a"), true, new Guid("2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac"), "Git" },
                    { new Guid("6ede8f53-90d2-4bf2-a12b-fa23aeb867b0"), false, new Guid("2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac"), "Nginx" },
                    { new Guid("7165f901-b72f-432a-a266-9cf5f9c44e62"), true, new Guid("a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed"), "Transformer" },
                    { new Guid("766fd5ca-8bf8-4ab0-9152-9979f44cc6af"), false, new Guid("b85875b3-e141-4392-9447-58d2f2efbdd9"), "Base de datos documental" },
                    { new Guid("7bec1930-def7-47ac-84b1-11de96b87c87"), true, new Guid("7992d28c-4399-4180-a957-8755f76e1184"), "CSS" },
                    { new Guid("7f43030c-1af6-453e-bb37-0944198860a7"), false, new Guid("b85875b3-e141-4392-9447-58d2f2efbdd9"), "Base de datos clave-valor" },
                    { new Guid("7f768266-e394-4fbe-9c29-55e5c6ef418b"), false, new Guid("45080bdc-002e-40b3-9027-15e764ff5c63"), "Cobertura de código" },
                    { new Guid("8500a296-cb9e-4e97-8c5c-b2f2d96ec446"), false, new Guid("3c933504-e478-4984-9044-365bd27ccb6c"), "Un solo proceso monolítico" },
                    { new Guid("8faea26f-c49a-4353-b580-36eaa695f69b"), true, new Guid("bca1ad0d-c8e0-48f7-a83e-39d71540c206"), "Interfaz de Programación de Aplicaciones" },
                    { new Guid("8fc0d028-64ba-48d8-8c57-b4a3b209ef91"), false, new Guid("095aa9ae-8d60-46a1-bba6-78e9fa6c86e7"), "Grafo" },
                    { new Guid("9825d3fd-0cf7-4148-bf91-4b5fe92a1d52"), false, new Guid("990bd4b0-5e85-497e-a19e-2d4ef3d1263e"), "Contenedor de dependencias" },
                    { new Guid("9b91d187-6b96-483b-a3b7-ce4a51bdf5fe"), false, new Guid("5d34c73c-1f5a-4685-ad54-bee58fbf677d"), "Normalización por lotes" },
                    { new Guid("a4aee812-2a6e-4a17-83f5-4942c3db8f1b"), true, new Guid("3c933504-e478-4984-9044-365bd27ccb6c"), "Despliegue independiente por servicio" },
                    { new Guid("b209bea0-cc42-44cb-9643-ab885a46c6fa"), false, new Guid("427c79c8-d4b8-48c4-9308-b7eb1ddf140e"), "Simulación de Cadenas Lineales" },
                    { new Guid("b3ab0076-11ea-4df7-b14f-ebb685adad09"), false, new Guid("7992d28c-4399-4180-a957-8755f76e1184"), "SQL" },
                    { new Guid("b76cd511-1cbc-4ccb-9745-83f1dca17d88"), false, new Guid("2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac"), "Docker" },
                    { new Guid("b82290b2-ece9-4510-ab39-9043ecaa4e05"), false, new Guid("41905891-a86f-44f1-b8f4-3bdc12105fc0"), "gRPC binario puro" },
                    { new Guid("c28194f2-bf5e-408a-83f3-ae4abf295e2b"), true, new Guid("427c79c8-d4b8-48c4-9308-b7eb1ddf140e"), "Lenguaje de Consulta Estructurado" },
                    { new Guid("c3e6a42e-2860-47f9-8187-abf6f6d548e2"), true, new Guid("b85875b3-e141-4392-9447-58d2f2efbdd9"), "Base de datos columnar" },
                    { new Guid("c62e8a06-f2c2-4ba6-bc4c-76367d2503a6"), false, new Guid("a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed"), "Red de Hopfield" },
                    { new Guid("c8b6dc4e-4ec2-4c07-9e40-097c01176227"), false, new Guid("2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac"), "Redis" },
                    { new Guid("cfcec57d-dfb5-40a9-8be9-be3b0066fa05"), false, new Guid("3fa633a4-6979-45a9-a589-cf8c2754b6ec"), "CRUD" },
                    { new Guid("d0f86c95-7cb3-4e86-a922-3f8fc67cc445"), false, new Guid("4917bb8d-f18b-4afa-b437-4da489b75567"), "Apache Kafka" },
                    { new Guid("d10505ae-96ab-4099-9a30-3a96021658a5"), false, new Guid("a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed"), "Perceptrón simple" },
                    { new Guid("d125b2ae-cf90-4051-9f27-c65084125f25"), true, new Guid("45080bdc-002e-40b3-9027-15e764ff5c63"), "Precisión (Precision)" },
                    { new Guid("d3334534-dc85-4d1e-a526-f6f8fb915895"), false, new Guid("7d196c04-6a14-4946-959a-17166e0ad1f2"), "TDD" },
                    { new Guid("d4db326d-da94-4123-bfde-1b73c44d51a5"), false, new Guid("bca1ad0d-c8e0-48f7-a83e-39d71540c206"), "Arquitectura de Programas Integrados" },
                    { new Guid("d8503a01-3339-46ae-95c6-5f713b0e84f0"), false, new Guid("21e727f6-2da0-401a-9211-6ca24a85648f"), "Inversión de dependencias" },
                    { new Guid("d8a06579-66d7-434e-b37d-38037079f444"), false, new Guid("b85875b3-e141-4392-9447-58d2f2efbdd9"), "Base de datos de grafos" },
                    { new Guid("d8e13761-ad7c-4857-9728-cfee81cc129a"), false, new Guid("93a07525-cd78-42dd-bead-420e1cf7db0e"), "Observer" },
                    { new Guid("d949a619-63f4-4487-879d-2bccc39118b3"), false, new Guid("427c79c8-d4b8-48c4-9308-b7eb1ddf140e"), "Software de Control Lógico" },
                    { new Guid("dc198a2a-0e14-4605-8540-f0cac3724a13"), false, new Guid("7992d28c-4399-4180-a957-8755f76e1184"), "JSON" },
                    { new Guid("e01251eb-a282-4550-9fd6-4e4731840988"), false, new Guid("3c933504-e478-4984-9044-365bd27ccb6c"), "Una única base de datos compartida obligatoria" },
                    { new Guid("e1532ac7-042e-4653-84d3-be7f8ea5302e"), false, new Guid("4917bb8d-f18b-4afa-b437-4da489b75567"), "Redis" },
                    { new Guid("e3f1ab62-6c00-4aa1-ba53-7ad08e2c09c8"), false, new Guid("3c933504-e478-4984-9044-365bd27ccb6c"), "Ausencia de comunicación entre servicios" },
                    { new Guid("e41bfa0b-4c9d-4698-bb4e-4f69a4ef3d3b"), false, new Guid("bca1ad0d-c8e0-48f7-a83e-39d71540c206"), "Análisis de Producto Informático" },
                    { new Guid("e4b1ec49-581d-47a3-8539-793fd038e771"), true, new Guid("21e727f6-2da0-401a-9211-6ca24a85648f"), "Responsabilidad única" },
                    { new Guid("ef215a79-7aad-4013-864e-bf764a007a9c"), true, new Guid("4917bb8d-f18b-4afa-b437-4da489b75567"), "Apache Airflow" },
                    { new Guid("f671855e-9d4b-4571-b6e8-e38ad8101aa1"), false, new Guid("45080bdc-002e-40b3-9027-15e764ff5c63"), "Complejidad ciclomática" },
                    { new Guid("fadc3e62-c0f6-4222-a183-829af84327b4"), false, new Guid("21e727f6-2da0-401a-9211-6ca24a85648f"), "Abierto/Cerrado" },
                    { new Guid("fe6fd12a-ce84-4c4a-82ce-a66c9f5ab717"), false, new Guid("990bd4b0-5e85-497e-a19e-2d4ef3d1263e"), "Compilador" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Pending",
                schema: "dbo",
                table: "OutboxMessages",
                column: "ProcessedOnUtc",
                filter: "[ProcessedOnUtc] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdempotencyRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "dbo");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("02e173bd-e815-412b-8e4a-c1a29ed15933"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("0551a798-9577-4fc9-81fd-0fdf6907720f"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("0709943d-63f7-4d98-b4e7-89a62c8c690c"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("0c62b880-f2b6-49c3-ae54-549aed98786e"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("0c7c8a6e-68ec-4859-b8ab-122c3d2e0f3c"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("0f37d8d4-e79f-46d6-9dd1-3a1bb07c199a"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("14b94f04-b857-46e3-bc6f-8f37b237f363"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("197fafb0-891c-404b-a5f8-4ce22e22f7d3"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("1cb66463-8ecd-403f-af9b-9df36c213021"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("206b1a69-23b1-424b-95a7-c744fa847946"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("2246117f-ffef-47d2-87e7-6c6e95485867"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("224fab6e-24cb-4902-a89a-bc02cf2dfe3d"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("26f60334-ec80-444b-9099-514204cdac89"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("3145ff07-0517-49a6-bb02-96e59b490d49"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("32a71eb0-394b-4411-b995-bb8204edf856"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("3560622c-6851-43dd-962c-ce8c59d41d85"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("357fd4f3-28a7-42d9-8ed3-9eeea8b3651f"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("39ec0665-b326-4be1-b7fd-6d55fef959c1"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("3b60fd8b-7b44-46a4-89a4-6771d4ec1995"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("3fa01924-ac0c-4276-90ab-a6c3407aa943"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("46f12667-a4bd-4183-bd36-d24c7c84e068"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("493e677c-02e7-44ee-b46f-41326ec5bcce"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("4aef8f31-4732-4fd0-9205-47ae7c4c6d7e"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("4c8d96ab-3e97-4d49-bc59-a404d215ada1"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("58155e79-65f0-4ecb-9f0c-bc278f2169a3"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("583d0f0b-40f5-4a1f-b199-f8b78d60a099"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("5b97921c-55f9-49f4-80cf-32240dffb935"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("5d13aff7-6d70-4707-a825-408ccc728844"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("65e31c46-cb67-4d38-9dc2-695322fc953a"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("6a740a2b-f51a-4927-bc7a-e4e65d875225"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("6c409052-d21b-4c9b-90a5-e85c76fda3eb"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("6df053aa-fd38-4a71-afd0-28a320ac566a"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("6ede8f53-90d2-4bf2-a12b-fa23aeb867b0"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("7165f901-b72f-432a-a266-9cf5f9c44e62"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("766fd5ca-8bf8-4ab0-9152-9979f44cc6af"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("7bec1930-def7-47ac-84b1-11de96b87c87"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("7f43030c-1af6-453e-bb37-0944198860a7"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("7f768266-e394-4fbe-9c29-55e5c6ef418b"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("8500a296-cb9e-4e97-8c5c-b2f2d96ec446"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("8faea26f-c49a-4353-b580-36eaa695f69b"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("8fc0d028-64ba-48d8-8c57-b4a3b209ef91"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("9825d3fd-0cf7-4148-bf91-4b5fe92a1d52"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("9b91d187-6b96-483b-a3b7-ce4a51bdf5fe"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("a4aee812-2a6e-4a17-83f5-4942c3db8f1b"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("b209bea0-cc42-44cb-9643-ab885a46c6fa"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("b3ab0076-11ea-4df7-b14f-ebb685adad09"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("b76cd511-1cbc-4ccb-9745-83f1dca17d88"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("b82290b2-ece9-4510-ab39-9043ecaa4e05"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("c28194f2-bf5e-408a-83f3-ae4abf295e2b"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("c3e6a42e-2860-47f9-8187-abf6f6d548e2"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("c62e8a06-f2c2-4ba6-bc4c-76367d2503a6"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("c8b6dc4e-4ec2-4c07-9e40-097c01176227"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("cfcec57d-dfb5-40a9-8be9-be3b0066fa05"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d0f86c95-7cb3-4e86-a922-3f8fc67cc445"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d10505ae-96ab-4099-9a30-3a96021658a5"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d125b2ae-cf90-4051-9f27-c65084125f25"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d3334534-dc85-4d1e-a526-f6f8fb915895"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d4db326d-da94-4123-bfde-1b73c44d51a5"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d8503a01-3339-46ae-95c6-5f713b0e84f0"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d8a06579-66d7-434e-b37d-38037079f444"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d8e13761-ad7c-4857-9728-cfee81cc129a"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("d949a619-63f4-4487-879d-2bccc39118b3"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("dc198a2a-0e14-4605-8540-f0cac3724a13"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("e01251eb-a282-4550-9fd6-4e4731840988"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("e1532ac7-042e-4653-84d3-be7f8ea5302e"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("e3f1ab62-6c00-4aa1-ba53-7ad08e2c09c8"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("e41bfa0b-4c9d-4698-bb4e-4f69a4ef3d3b"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("e4b1ec49-581d-47a3-8539-793fd038e771"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("ef215a79-7aad-4013-864e-bf764a007a9c"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("f671855e-9d4b-4571-b6e8-e38ad8101aa1"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("fadc3e62-c0f6-4222-a183-829af84327b4"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Answers",
                keyColumn: "Id",
                keyValue: new Guid("fe6fd12a-ce84-4c4a-82ce-a66c9f5ab717"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("095aa9ae-8d60-46a1-bba6-78e9fa6c86e7"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("21e727f6-2da0-401a-9211-6ca24a85648f"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("3c933504-e478-4984-9044-365bd27ccb6c"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("3fa633a4-6979-45a9-a589-cf8c2754b6ec"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("41905891-a86f-44f1-b8f4-3bdc12105fc0"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("427c79c8-d4b8-48c4-9308-b7eb1ddf140e"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("45080bdc-002e-40b3-9027-15e764ff5c63"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("4917bb8d-f18b-4afa-b437-4da489b75567"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("5d34c73c-1f5a-4685-ad54-bee58fbf677d"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("7992d28c-4399-4180-a957-8755f76e1184"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("7d196c04-6a14-4946-959a-17166e0ad1f2"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("93a07525-cd78-42dd-bead-420e1cf7db0e"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("990bd4b0-5e85-497e-a19e-2d4ef3d1263e"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("a4999ba8-15f3-4585-844a-0f0e1f17dcee"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("b85875b3-e141-4392-9447-58d2f2efbdd9"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("bca1ad0d-c8e0-48f7-a83e-39d71540c206"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1ab48485-17e9-4eae-bcf6-113bbf0135c6"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("36cafaf0-b7cd-4556-9ac7-4b5b83f39118"));

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("59ef08e6-1514-4379-87a1-c1b802d616ba"));
        }
    }
}
