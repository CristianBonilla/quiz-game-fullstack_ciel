BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095827_SeedInitialData'
)
BEGIN
    CREATE TABLE [dbo].[IdempotencyRecords] (
        [RequestId] uniqueidentifier NOT NULL,
        [RequestName] nvarchar(200) NOT NULL,
        [SerializedResponse] nvarchar(4000) NULL,
        [CreatedOnUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_IdempotencyRecords] PRIMARY KEY ([RequestId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095827_SeedInitialData'
)
BEGIN
    CREATE TABLE [dbo].[OutboxMessages] (
        [Id] uniqueidentifier NOT NULL,
        [Type] nvarchar(400) NOT NULL,
        [Content] nvarchar(4000) NOT NULL,
        [OccurredOnUtc] datetime2 NOT NULL,
        [ProcessedOnUtc] datetime2 NULL,
        [Error] nvarchar(2000) NULL,
        CONSTRAINT [PK_OutboxMessages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095827_SeedInitialData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'DifficultyLevel', N'IsActive', N'Name', N'PrizeAmount') AND [object_id] = OBJECT_ID(N'[dbo].[Categories]'))
        SET IDENTITY_INSERT [dbo].[Categories] ON;
    EXEC(N'INSERT INTO [dbo].[Categories] ([Id], [Description], [DifficultyLevel], [IsActive], [Name], [PrizeAmount])
    VALUES (''1ab48485-17e9-4eae-bcf6-113bbf0135c6'', N''Advanced data pipelines and machine learning questions for the final rounds.'', 3, CAST(1 AS bit), N''Data & AI Engineering'', 2000.0),
    (''36cafaf0-b7cd-4556-9ac7-4b5b83f39118'', N''Architectural styles, design patterns and system design questions.'', 2, CAST(1 AS bit), N''Software Architecture'', 500.0),
    (''59ef08e6-1514-4379-87a1-c1b802d616ba'', N''Foundational programming and tooling questions.'', 1, CAST(1 AS bit), N''Software Development'', 100.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'DifficultyLevel', N'IsActive', N'Name', N'PrizeAmount') AND [object_id] = OBJECT_ID(N'[dbo].[Categories]'))
        SET IDENTITY_INSERT [dbo].[Categories] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095827_SeedInitialData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'IsActive', N'Text') AND [object_id] = OBJECT_ID(N'[dbo].[Questions]'))
        SET IDENTITY_INSERT [dbo].[Questions] ON;
    EXEC(N'INSERT INTO [dbo].[Questions] ([Id], [CategoryId], [IsActive], [Text])
    VALUES (''095aa9ae-8d60-46a1-bba6-78e9fa6c86e7'', ''59ef08e6-1514-4379-87a1-c1b802d616ba'', CAST(1 AS bit), N''¿Qué estructura de datos sigue el principio "primero en entrar, primero en salir" (FIFO)?''),
    (''21e727f6-2da0-401a-9211-6ca24a85648f'', ''36cafaf0-b7cd-4556-9ac7-4b5b83f39118'', CAST(1 AS bit), N''¿Qué principio SOLID establece que una clase debe tener una única razón para cambiar?''),
    (''2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac'', ''59ef08e6-1514-4379-87a1-c1b802d616ba'', CAST(1 AS bit), N''¿Cuál de las siguientes opciones es un sistema de control de versiones?''),
    (''3c933504-e478-4984-9044-365bd27ccb6c'', ''36cafaf0-b7cd-4556-9ac7-4b5b83f39118'', CAST(1 AS bit), N''¿Cuál de los siguientes es un principio de los sistemas orientados a microservicios?''),
    (''3fa633a4-6979-45a9-a589-cf8c2754b6ec'', ''36cafaf0-b7cd-4556-9ac7-4b5b83f39118'', CAST(1 AS bit), N''¿Qué patrón arquitectónico separa una aplicación en Modelo, Vista y Controlador?''),
    (''41905891-a86f-44f1-b8f4-3bdc12105fc0'', ''36cafaf0-b7cd-4556-9ac7-4b5b83f39118'', CAST(1 AS bit), N''¿Qué estilo arquitectónico expone recursos mediante URIs y verbos HTTP?''),
    (''427c79c8-d4b8-48c4-9308-b7eb1ddf140e'', ''59ef08e6-1514-4379-87a1-c1b802d616ba'', CAST(1 AS bit), N''¿Qué significa la sigla SQL?''),
    (''45080bdc-002e-40b3-9027-15e764ff5c63'', ''1ab48485-17e9-4eae-bcf6-113bbf0135c6'', CAST(1 AS bit), N''¿Qué métrica evalúa la proporción de verdaderos positivos sobre el total de positivos predichos por un modelo de clasificación?''),
    (''4917bb8d-f18b-4afa-b437-4da489b75567'', ''1ab48485-17e9-4eae-bcf6-113bbf0135c6'', CAST(1 AS bit), N''¿Qué herramienta se utiliza comúnmente para orquestar flujos de trabajo de datos (pipelines) por lotes?''),
    (''5d34c73c-1f5a-4685-ad54-bee58fbf677d'', ''1ab48485-17e9-4eae-bcf6-113bbf0135c6'', CAST(1 AS bit), N''¿Qué técnica de aprendizaje automático ajusta los pesos de una red neuronal usando el gradiente del error?''),
    (''7992d28c-4399-4180-a957-8755f76e1184'', ''59ef08e6-1514-4379-87a1-c1b802d616ba'', CAST(1 AS bit), N''¿Qué lenguaje se usa principalmente para dar estilo a las páginas web?''),
    (''7d196c04-6a14-4946-959a-17166e0ad1f2'', ''1ab48485-17e9-4eae-bcf6-113bbf0135c6'', CAST(1 AS bit), N''¿Qué proceso describe la extracción, transformación y carga de datos entre sistemas?''),
    (''93a07525-cd78-42dd-bead-420e1cf7db0e'', ''36cafaf0-b7cd-4556-9ac7-4b5b83f39118'', CAST(1 AS bit), N''¿Qué patrón de diseño garantiza que una clase tenga una única instancia global?''),
    (''990bd4b0-5e85-497e-a19e-2d4ef3d1263e'', ''36cafaf0-b7cd-4556-9ac7-4b5b83f39118'', CAST(1 AS bit), N''¿Qué componente se utiliza típicamente para distribuir la carga entre varias instancias de un servicio?''),
    (''a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed'', ''1ab48485-17e9-4eae-bcf6-113bbf0135c6'', CAST(1 AS bit), N''¿Qué arquitectura de red neuronal se destaca por su uso del mecanismo de atención y es la base de los modelos de lenguaje modernos?''),
    (''a4999ba8-15f3-4585-844a-0f0e1f17dcee'', ''59ef08e6-1514-4379-87a1-c1b802d616ba'', CAST(1 AS bit), N''¿Cuál de estas opciones es un tipo de dato primitivo en la mayoría de lenguajes de programación?''),
    (''b85875b3-e141-4392-9447-58d2f2efbdd9'', ''1ab48485-17e9-4eae-bcf6-113bbf0135c6'', CAST(1 AS bit), N''¿Qué tipo de base de datos está optimizada para el almacenamiento y consulta analítica de grandes volúmenes de datos en columnas?''),
    (''bca1ad0d-c8e0-48f7-a83e-39d71540c206'', ''59ef08e6-1514-4379-87a1-c1b802d616ba'', CAST(1 AS bit), N''¿Qué significa la sigla API?'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'IsActive', N'Text') AND [object_id] = OBJECT_ID(N'[dbo].[Questions]'))
        SET IDENTITY_INSERT [dbo].[Questions] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095827_SeedInitialData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsCorrect', N'QuestionId', N'Text') AND [object_id] = OBJECT_ID(N'[dbo].[Answers]'))
        SET IDENTITY_INSERT [dbo].[Answers] ON;
    EXEC(N'INSERT INTO [dbo].[Answers] ([Id], [IsCorrect], [QuestionId], [Text])
    VALUES (''02e173bd-e815-412b-8e4a-c1a29ed15933'', CAST(0 AS bit), ''7d196c04-6a14-4946-959a-17166e0ad1f2'', N''CRUD''),
    (''0551a798-9577-4fc9-81fd-0fdf6907720f'', CAST(0 AS bit), ''93a07525-cd78-42dd-bead-420e1cf7db0e'', N''Factory''),
    (''0709943d-63f7-4d98-b4e7-89a62c8c690c'', CAST(0 AS bit), ''427c79c8-d4b8-48c4-9308-b7eb1ddf140e'', N''Sistema de Consultas Lineales''),
    (''0c62b880-f2b6-49c3-ae54-549aed98786e'', CAST(0 AS bit), ''095aa9ae-8d60-46a1-bba6-78e9fa6c86e7'', N''Pila''),
    (''0c7c8a6e-68ec-4859-b8ab-122c3d2e0f3c'', CAST(0 AS bit), ''21e727f6-2da0-401a-9211-6ca24a85648f'', N''Sustitución de Liskov''),
    (''0f37d8d4-e79f-46d6-9dd1-3a1bb07c199a'', CAST(0 AS bit), ''a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed'', N''Máquina de vectores de soporte''),
    (''14b94f04-b857-46e3-bc6f-8f37b237f363'', CAST(0 AS bit), ''095aa9ae-8d60-46a1-bba6-78e9fa6c86e7'', N''Árbol''),
    (''197fafb0-891c-404b-a5f8-4ce22e22f7d3'', CAST(1 AS bit), ''93a07525-cd78-42dd-bead-420e1cf7db0e'', N''Singleton''),
    (''1cb66463-8ecd-403f-af9b-9df36c213021'', CAST(0 AS bit), ''3fa633a4-6979-45a9-a589-cf8c2754b6ec'', N''SOAP''),
    (''206b1a69-23b1-424b-95a7-c744fa847946'', CAST(1 AS bit), ''5d34c73c-1f5a-4685-ad54-bee58fbf677d'', N''Retropropagación''),
    (''2246117f-ffef-47d2-87e7-6c6e95485867'', CAST(0 AS bit), ''41905891-a86f-44f1-b8f4-3bdc12105fc0'', N''FTP''),
    (''224fab6e-24cb-4902-a89a-bc02cf2dfe3d'', CAST(0 AS bit), ''45080bdc-002e-40b3-9027-15e764ff5c63'', N''Latencia''),
    (''26f60334-ec80-444b-9099-514204cdac89'', CAST(0 AS bit), ''3fa633a4-6979-45a9-a589-cf8c2754b6ec'', N''REST''),
    (''3145ff07-0517-49a6-bb02-96e59b490d49'', CAST(0 AS bit), ''4917bb8d-f18b-4afa-b437-4da489b75567'', N''NGINX''),
    (''32a71eb0-394b-4411-b995-bb8204edf856'', CAST(1 AS bit), ''095aa9ae-8d60-46a1-bba6-78e9fa6c86e7'', N''Cola''),
    (''3560622c-6851-43dd-962c-ce8c59d41d85'', CAST(0 AS bit), ''93a07525-cd78-42dd-bead-420e1cf7db0e'', N''Decorator''),
    (''357fd4f3-28a7-42d9-8ed3-9eeea8b3651f'', CAST(1 AS bit), ''3fa633a4-6979-45a9-a589-cf8c2754b6ec'', N''MVC''),
    (''39ec0665-b326-4be1-b7fd-6d55fef959c1'', CAST(0 AS bit), ''5d34c73c-1f5a-4685-ad54-bee58fbf677d'', N''Poda de árboles''),
    (''3b60fd8b-7b44-46a4-89a4-6771d4ec1995'', CAST(0 AS bit), ''7d196c04-6a14-4946-959a-17166e0ad1f2'', N''CI/CD''),
    (''3fa01924-ac0c-4276-90ab-a6c3407aa943'', CAST(1 AS bit), ''41905891-a86f-44f1-b8f4-3bdc12105fc0'', N''REST''),
    (''46f12667-a4bd-4183-bd36-d24c7c84e068'', CAST(0 AS bit), ''a4999ba8-15f3-4585-844a-0f0e1f17dcee'', N''Arreglo''),
    (''493e677c-02e7-44ee-b46f-41326ec5bcce'', CAST(1 AS bit), ''7d196c04-6a14-4946-959a-17166e0ad1f2'', N''ETL''),
    (''4aef8f31-4732-4fd0-9205-47ae7c4c6d7e'', CAST(0 AS bit), ''41905891-a86f-44f1-b8f4-3bdc12105fc0'', N''SOAP''),
    (''4c8d96ab-3e97-4d49-bc59-a404d215ada1'', CAST(0 AS bit), ''5d34c73c-1f5a-4685-ad54-bee58fbf677d'', N''Validación cruzada''),
    (''58155e79-65f0-4ecb-9f0c-bc278f2169a3'', CAST(0 AS bit), ''a4999ba8-15f3-4585-844a-0f0e1f17dcee'', N''Objeto''),
    (''583d0f0b-40f5-4a1f-b199-f8b78d60a099'', CAST(1 AS bit), ''990bd4b0-5e85-497e-a19e-2d4ef3d1263e'', N''Balanceador de carga''),
    (''5b97921c-55f9-49f4-80cf-32240dffb935'', CAST(0 AS bit), ''a4999ba8-15f3-4585-844a-0f0e1f17dcee'', N''Clase''),
    (''5d13aff7-6d70-4707-a825-408ccc728844'', CAST(1 AS bit), ''a4999ba8-15f3-4585-844a-0f0e1f17dcee'', N''Booleano''),
    (''65e31c46-cb67-4d38-9dc2-695322fc953a'', CAST(0 AS bit), ''990bd4b0-5e85-497e-a19e-2d4ef3d1263e'', N''Caché local''),
    (''6a740a2b-f51a-4927-bc7a-e4e65d875225'', CAST(0 AS bit), ''bca1ad0d-c8e0-48f7-a83e-39d71540c206'', N''Aplicación de Procesos Internos''),
    (''6c409052-d21b-4c9b-90a5-e85c76fda3eb'', CAST(0 AS bit), ''7992d28c-4399-4180-a957-8755f76e1184'', N''HTML''),
    (''6df053aa-fd38-4a71-afd0-28a320ac566a'', CAST(1 AS bit), ''2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac'', N''Git''),
    (''6ede8f53-90d2-4bf2-a12b-fa23aeb867b0'', CAST(0 AS bit), ''2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac'', N''Nginx''),
    (''7165f901-b72f-432a-a266-9cf5f9c44e62'', CAST(1 AS bit), ''a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed'', N''Transformer''),
    (''766fd5ca-8bf8-4ab0-9152-9979f44cc6af'', CAST(0 AS bit), ''b85875b3-e141-4392-9447-58d2f2efbdd9'', N''Base de datos documental''),
    (''7bec1930-def7-47ac-84b1-11de96b87c87'', CAST(1 AS bit), ''7992d28c-4399-4180-a957-8755f76e1184'', N''CSS''),
    (''7f43030c-1af6-453e-bb37-0944198860a7'', CAST(0 AS bit), ''b85875b3-e141-4392-9447-58d2f2efbdd9'', N''Base de datos clave-valor''),
    (''7f768266-e394-4fbe-9c29-55e5c6ef418b'', CAST(0 AS bit), ''45080bdc-002e-40b3-9027-15e764ff5c63'', N''Cobertura de código''),
    (''8500a296-cb9e-4e97-8c5c-b2f2d96ec446'', CAST(0 AS bit), ''3c933504-e478-4984-9044-365bd27ccb6c'', N''Un solo proceso monolítico''),
    (''8faea26f-c49a-4353-b580-36eaa695f69b'', CAST(1 AS bit), ''bca1ad0d-c8e0-48f7-a83e-39d71540c206'', N''Interfaz de Programación de Aplicaciones''),
    (''8fc0d028-64ba-48d8-8c57-b4a3b209ef91'', CAST(0 AS bit), ''095aa9ae-8d60-46a1-bba6-78e9fa6c86e7'', N''Grafo''),
    (''9825d3fd-0cf7-4148-bf91-4b5fe92a1d52'', CAST(0 AS bit), ''990bd4b0-5e85-497e-a19e-2d4ef3d1263e'', N''Contenedor de dependencias'');
    INSERT INTO [dbo].[Answers] ([Id], [IsCorrect], [QuestionId], [Text])
    VALUES (''9b91d187-6b96-483b-a3b7-ce4a51bdf5fe'', CAST(0 AS bit), ''5d34c73c-1f5a-4685-ad54-bee58fbf677d'', N''Normalización por lotes''),
    (''a4aee812-2a6e-4a17-83f5-4942c3db8f1b'', CAST(1 AS bit), ''3c933504-e478-4984-9044-365bd27ccb6c'', N''Despliegue independiente por servicio''),
    (''b209bea0-cc42-44cb-9643-ab885a46c6fa'', CAST(0 AS bit), ''427c79c8-d4b8-48c4-9308-b7eb1ddf140e'', N''Simulación de Cadenas Lineales''),
    (''b3ab0076-11ea-4df7-b14f-ebb685adad09'', CAST(0 AS bit), ''7992d28c-4399-4180-a957-8755f76e1184'', N''SQL''),
    (''b76cd511-1cbc-4ccb-9745-83f1dca17d88'', CAST(0 AS bit), ''2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac'', N''Docker''),
    (''b82290b2-ece9-4510-ab39-9043ecaa4e05'', CAST(0 AS bit), ''41905891-a86f-44f1-b8f4-3bdc12105fc0'', N''gRPC binario puro''),
    (''c28194f2-bf5e-408a-83f3-ae4abf295e2b'', CAST(1 AS bit), ''427c79c8-d4b8-48c4-9308-b7eb1ddf140e'', N''Lenguaje de Consulta Estructurado''),
    (''c3e6a42e-2860-47f9-8187-abf6f6d548e2'', CAST(1 AS bit), ''b85875b3-e141-4392-9447-58d2f2efbdd9'', N''Base de datos columnar''),
    (''c62e8a06-f2c2-4ba6-bc4c-76367d2503a6'', CAST(0 AS bit), ''a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed'', N''Red de Hopfield''),
    (''c8b6dc4e-4ec2-4c07-9e40-097c01176227'', CAST(0 AS bit), ''2a32cdc7-1c3a-4d84-b9e0-06ce7fca26ac'', N''Redis''),
    (''cfcec57d-dfb5-40a9-8be9-be3b0066fa05'', CAST(0 AS bit), ''3fa633a4-6979-45a9-a589-cf8c2754b6ec'', N''CRUD''),
    (''d0f86c95-7cb3-4e86-a922-3f8fc67cc445'', CAST(0 AS bit), ''4917bb8d-f18b-4afa-b437-4da489b75567'', N''Apache Kafka''),
    (''d10505ae-96ab-4099-9a30-3a96021658a5'', CAST(0 AS bit), ''a2e5ba2d-6dc1-47df-aeeb-e1540aa644ed'', N''Perceptrón simple''),
    (''d125b2ae-cf90-4051-9f27-c65084125f25'', CAST(1 AS bit), ''45080bdc-002e-40b3-9027-15e764ff5c63'', N''Precisión (Precision)''),
    (''d3334534-dc85-4d1e-a526-f6f8fb915895'', CAST(0 AS bit), ''7d196c04-6a14-4946-959a-17166e0ad1f2'', N''TDD''),
    (''d4db326d-da94-4123-bfde-1b73c44d51a5'', CAST(0 AS bit), ''bca1ad0d-c8e0-48f7-a83e-39d71540c206'', N''Arquitectura de Programas Integrados''),
    (''d8503a01-3339-46ae-95c6-5f713b0e84f0'', CAST(0 AS bit), ''21e727f6-2da0-401a-9211-6ca24a85648f'', N''Inversión de dependencias''),
    (''d8a06579-66d7-434e-b37d-38037079f444'', CAST(0 AS bit), ''b85875b3-e141-4392-9447-58d2f2efbdd9'', N''Base de datos de grafos''),
    (''d8e13761-ad7c-4857-9728-cfee81cc129a'', CAST(0 AS bit), ''93a07525-cd78-42dd-bead-420e1cf7db0e'', N''Observer''),
    (''d949a619-63f4-4487-879d-2bccc39118b3'', CAST(0 AS bit), ''427c79c8-d4b8-48c4-9308-b7eb1ddf140e'', N''Software de Control Lógico''),
    (''dc198a2a-0e14-4605-8540-f0cac3724a13'', CAST(0 AS bit), ''7992d28c-4399-4180-a957-8755f76e1184'', N''JSON''),
    (''e01251eb-a282-4550-9fd6-4e4731840988'', CAST(0 AS bit), ''3c933504-e478-4984-9044-365bd27ccb6c'', N''Una única base de datos compartida obligatoria''),
    (''e1532ac7-042e-4653-84d3-be7f8ea5302e'', CAST(0 AS bit), ''4917bb8d-f18b-4afa-b437-4da489b75567'', N''Redis''),
    (''e3f1ab62-6c00-4aa1-ba53-7ad08e2c09c8'', CAST(0 AS bit), ''3c933504-e478-4984-9044-365bd27ccb6c'', N''Ausencia de comunicación entre servicios''),
    (''e41bfa0b-4c9d-4698-bb4e-4f69a4ef3d3b'', CAST(0 AS bit), ''bca1ad0d-c8e0-48f7-a83e-39d71540c206'', N''Análisis de Producto Informático''),
    (''e4b1ec49-581d-47a3-8539-793fd038e771'', CAST(1 AS bit), ''21e727f6-2da0-401a-9211-6ca24a85648f'', N''Responsabilidad única''),
    (''ef215a79-7aad-4013-864e-bf764a007a9c'', CAST(1 AS bit), ''4917bb8d-f18b-4afa-b437-4da489b75567'', N''Apache Airflow''),
    (''f671855e-9d4b-4571-b6e8-e38ad8101aa1'', CAST(0 AS bit), ''45080bdc-002e-40b3-9027-15e764ff5c63'', N''Complejidad ciclomática''),
    (''fadc3e62-c0f6-4222-a183-829af84327b4'', CAST(0 AS bit), ''21e727f6-2da0-401a-9211-6ca24a85648f'', N''Abierto/Cerrado''),
    (''fe6fd12a-ce84-4c4a-82ce-a66c9f5ab717'', CAST(0 AS bit), ''990bd4b0-5e85-497e-a19e-2d4ef3d1263e'', N''Compilador'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsCorrect', N'QuestionId', N'Text') AND [object_id] = OBJECT_ID(N'[dbo].[Answers]'))
        SET IDENTITY_INSERT [dbo].[Answers] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095827_SeedInitialData'
)
BEGIN
    EXEC(N'CREATE INDEX [IX_OutboxMessages_Pending] ON [dbo].[OutboxMessages] ([ProcessedOnUtc]) WHERE [ProcessedOnUtc] IS NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095827_SeedInitialData'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909095827_SeedInitialData', N'10.0.0');
END;

COMMIT;
GO

