using QuizGame.Domain.Categories;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Infrastructure.Persistence.Seed;

public static class QuizGameSeedData
{
    public static IReadOnlyList<Category> Categories { get; }

    public static IReadOnlyList<Question> Questions { get; }

    public static IReadOnlyList<Answer> Answers { get; }

    // HasData rejects entity instances with populated navigations, so the seed rows for each
    // table are plain projections of the scalar/FK values, not the connected aggregate graph.
    public static IReadOnlyList<object> CategoryRows { get; }

    public static IReadOnlyList<object> QuestionRows { get; }

    public static IReadOnlyList<object> AnswerRows { get; }

    static QuizGameSeedData()
    {
        List<Question> easyQuestions =
        [
            BuildQuestion(
                SeedIds.EasyQuestionIds[0], SeedIds.EasyCategoryId, SeedIds.EasyAnswerIds[0],
                "¿Qué significa la sigla API?",
                ("Aplicación de Procesos Internos", false), ("Interfaz de Programación de Aplicaciones", true),
                ("Arquitectura de Programas Integrados", false), ("Análisis de Producto Informático", false)),
            BuildQuestion(
                SeedIds.EasyQuestionIds[1], SeedIds.EasyCategoryId, SeedIds.EasyAnswerIds[1],
                "¿Cuál de las siguientes opciones es un sistema de control de versiones?",
                ("Docker", false), ("Nginx", false), ("Git", true), ("Redis", false)),
            BuildQuestion(
                SeedIds.EasyQuestionIds[2], SeedIds.EasyCategoryId, SeedIds.EasyAnswerIds[2],
                "¿Qué estructura de datos sigue el principio \"primero en entrar, primero en salir\" (FIFO)?",
                ("Cola", true), ("Pila", false), ("Árbol", false), ("Grafo", false)),
            BuildQuestion(
                SeedIds.EasyQuestionIds[3], SeedIds.EasyCategoryId, SeedIds.EasyAnswerIds[3],
                "¿Qué lenguaje se usa principalmente para dar estilo a las páginas web?",
                ("HTML", false), ("CSS", true), ("SQL", false), ("JSON", false)),
            BuildQuestion(
                SeedIds.EasyQuestionIds[4], SeedIds.EasyCategoryId, SeedIds.EasyAnswerIds[4],
                "¿Qué significa la sigla SQL?",
                ("Sistema de Consultas Lineales", false), ("Software de Control Lógico", false),
                ("Lenguaje de Consulta Estructurado", true), ("Simulación de Cadenas Lineales", false)),
            BuildQuestion(
                SeedIds.EasyQuestionIds[5], SeedIds.EasyCategoryId, SeedIds.EasyAnswerIds[5],
                "¿Cuál de estas opciones es un tipo de dato primitivo en la mayoría de lenguajes de programación?",
                ("Arreglo", false), ("Objeto", false), ("Clase", false), ("Booleano", true))
        ];

        List<Question> intermediateQuestions =
        [
            BuildQuestion(
                SeedIds.IntermediateQuestionIds[0], SeedIds.IntermediateCategoryId, SeedIds.IntermediateAnswerIds[0],
                "¿Qué patrón arquitectónico separa una aplicación en Modelo, Vista y Controlador?",
                ("CRUD", false), ("MVC", true), ("REST", false), ("SOAP", false)),
            BuildQuestion(
                SeedIds.IntermediateQuestionIds[1], SeedIds.IntermediateCategoryId, SeedIds.IntermediateAnswerIds[1],
                "¿Cuál de los siguientes es un principio de los sistemas orientados a microservicios?",
                ("Un solo proceso monolítico", false), ("Ausencia de comunicación entre servicios", false),
                ("Despliegue independiente por servicio", true), ("Una única base de datos compartida obligatoria", false)),
            BuildQuestion(
                SeedIds.IntermediateQuestionIds[2], SeedIds.IntermediateCategoryId, SeedIds.IntermediateAnswerIds[2],
                "¿Qué principio SOLID establece que una clase debe tener una única razón para cambiar?",
                ("Abierto/Cerrado", false), ("Responsabilidad única", true), ("Sustitución de Liskov", false),
                ("Inversión de dependencias", false)),
            BuildQuestion(
                SeedIds.IntermediateQuestionIds[3], SeedIds.IntermediateCategoryId, SeedIds.IntermediateAnswerIds[3],
                "¿Qué patrón de diseño garantiza que una clase tenga una única instancia global?",
                ("Factory", false), ("Observer", false), ("Singleton", true), ("Decorator", false)),
            BuildQuestion(
                SeedIds.IntermediateQuestionIds[4], SeedIds.IntermediateCategoryId, SeedIds.IntermediateAnswerIds[4],
                "¿Qué componente se utiliza típicamente para distribuir la carga entre varias instancias de un servicio?",
                ("Caché local", false), ("Compilador", false), ("Contenedor de dependencias", false),
                ("Balanceador de carga", true)),
            BuildQuestion(
                SeedIds.IntermediateQuestionIds[5], SeedIds.IntermediateCategoryId, SeedIds.IntermediateAnswerIds[5],
                "¿Qué estilo arquitectónico expone recursos mediante URIs y verbos HTTP?",
                ("REST", true), ("SOAP", false), ("gRPC binario puro", false), ("FTP", false))
        ];

        List<Question> hardQuestions =
        [
            BuildQuestion(
                SeedIds.HardQuestionIds[0], SeedIds.HardCategoryId, SeedIds.HardAnswerIds[0],
                "¿Qué proceso describe la extracción, transformación y carga de datos entre sistemas?",
                ("CRUD", false), ("TDD", false), ("ETL", true), ("CI/CD", false)),
            BuildQuestion(
                SeedIds.HardQuestionIds[1], SeedIds.HardCategoryId, SeedIds.HardAnswerIds[1],
                "¿Qué técnica de aprendizaje automático ajusta los pesos de una red neuronal usando el gradiente del error?",
                ("Normalización por lotes", false), ("Retropropagación", true), ("Poda de árboles", false),
                ("Validación cruzada", false)),
            BuildQuestion(
                SeedIds.HardQuestionIds[2], SeedIds.HardCategoryId, SeedIds.HardAnswerIds[2],
                "¿Qué tipo de base de datos está optimizada para el almacenamiento y consulta analítica de grandes " +
                "volúmenes de datos en columnas?",
                ("Base de datos de grafos", false), ("Base de datos documental", false),
                ("Base de datos clave-valor", false), ("Base de datos columnar", true)),
            BuildQuestion(
                SeedIds.HardQuestionIds[3], SeedIds.HardCategoryId, SeedIds.HardAnswerIds[3],
                "¿Qué arquitectura de red neuronal se destaca por su uso del mecanismo de atención y es la base de " +
                "los modelos de lenguaje modernos?",
                ("Perceptrón simple", false), ("Transformer", true), ("Red de Hopfield", false),
                ("Máquina de vectores de soporte", false)),
            BuildQuestion(
                SeedIds.HardQuestionIds[4], SeedIds.HardCategoryId, SeedIds.HardAnswerIds[4],
                "¿Qué herramienta se utiliza comúnmente para orquestar flujos de trabajo de datos (pipelines) por lotes?",
                ("Apache Airflow", true), ("Apache Kafka", false), ("Redis", false), ("NGINX", false)),
            BuildQuestion(
                SeedIds.HardQuestionIds[5], SeedIds.HardCategoryId, SeedIds.HardAnswerIds[5],
                "¿Qué métrica evalúa la proporción de verdaderos positivos sobre el total de positivos predichos por " +
                "un modelo de clasificación?",
                ("Latencia", false), ("Cobertura de código", false), ("Complejidad ciclomática", false),
                ("Precisión (Precision)", true))
        ];

        Category easy = BuildCategory(
            SeedIds.EasyCategoryId, "Software Development", "Foundational programming and tooling questions.", 1,
            100m, easyQuestions);
        Category intermediate = BuildCategory(
            SeedIds.IntermediateCategoryId, "Software Architecture",
            "Architectural styles, design patterns and system design questions.", 2, 500m, intermediateQuestions);
        Category hard = BuildCategory(
            SeedIds.HardCategoryId, "Data & AI Engineering",
            "Advanced data pipelines and machine learning questions for the final rounds.", 3, 2000m, hardQuestions);

        Categories = [easy, intermediate, hard];
        Questions = [.. easyQuestions, .. intermediateQuestions, .. hardQuestions];
        Answers = [.. Questions.SelectMany(question => question.Answers)];

        CategoryRows = [.. Categories.Select(category => (object)new
        {
            category.Id,
            category.Name,
            category.Description,
            category.DifficultyLevel,
            category.PrizeAmount,
            category.IsActive
        })];

        QuestionRows = [.. Questions.Select(question => (object)new
        {
            question.Id,
            question.CategoryId,
            question.Text,
            question.IsActive
        })];

        AnswerRows = [.. Answers.Select(answer => (object)new
        {
            answer.Id,
            answer.QuestionId,
            answer.Text,
            answer.IsCorrect
        })];
    }

    private static Question BuildQuestion(
        Guid questionId,
        Guid categoryId,
        IReadOnlyList<Guid> answerIds,
        string text,
        params (string Text, bool IsCorrect)[] options)
    {
        List<AnswerCandidate> candidates = new(options.Length);

        for (int index = 0; index < options.Length; index++)
        {
            candidates.Add(new AnswerCandidate(
                answerIds[index],
                AnswerText.Create(options[index].Text).Value,
                options[index].IsCorrect));
        }

        return Question.Create(questionId, categoryId, QuestionText.Create(text).Value, candidates).Value;
    }

    private static Category BuildCategory(
        Guid categoryId,
        string name,
        string description,
        int difficultyLevel,
        decimal prizeAmount,
        IReadOnlyList<Question> questions)
    {
        Category category = Category.Create(
            categoryId,
            name,
            description,
            DifficultyLevel.Create(difficultyLevel).Value,
            Prize.Create(prizeAmount).Value).Value;

        foreach (Question question in questions)
        {
            category.AddQuestion(question);
        }

        category.Activate();

        return category;
    }
}
