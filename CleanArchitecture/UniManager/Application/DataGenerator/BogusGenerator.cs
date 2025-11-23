using Bogus;
using Domain.Entities;
using Domain.ValueObjects;
using Application.Services;
using Application.Interfaces;

public class BogusGenerator
{
    private readonly IStudentService _studentService;
    private readonly IStudentStudiowMgrService _studentMgrService;
    private readonly IProfesorService _profesorService;
    private readonly IWydzialService _wydzialService;
    private readonly IKursService _kursService;
    private readonly IGabinetService _gabinetService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly List<string> tytuly = new List<string>
    {
        "dr",
        "dr inż.",
        "dr hab.",
        "dr hab. inż.",
        "prof. nadzw.",
        "prof.",
        "prof. dr. hab.",
        "prof. dr. hab. inż."
    };
    private readonly List<string> nazwyKursow = new List<string>
    {
        "Podstawy Programowania",
        "Struktury Danych i Algorytmy",
        "Bazy Danych",
        "Systemy Operacyjne",
        "Sieci Komputerowe",
        "Inżynieria Oprogramowania",
        "Analiza Matematyczna",
        "Algebra Liniowa",
        "Fizyka dla Informatyków",
        "Metody Numeryczne",
        "Grafika Komputerowa",
        "Sztuczna Inteligencja",
        "Programowanie Obiektowe",
        "Projektowanie Interfejsów Użytkownika",
        "Bezpieczeństwo Komputerowe",
        "Matematyka Dyskretna",
        "Projektowanie mikroprocesorów"
    };
    private readonly List<string> nazwyWydzialow = new List<string>
    {
        "Wydział Informatyki",
        "Wydział Elektroniki",
        "Wydział Matematyki",
        "Instytut Fiyki"
    };
    private readonly int _maxSemester = 7;

    public BogusGenerator(IStudentService studentService, IStudentStudiowMgrService studentMgrService, IProfesorService profesorService, IWydzialService wydzialService, IKursService kursService, IGabinetService gabinetService, IEnrollmentService enrollmentService)
    {
        _studentService = studentService;
        _studentMgrService = studentMgrService;
        _profesorService = profesorService;
        _wydzialService = wydzialService;
        _kursService = kursService;
        _gabinetService = gabinetService;
        _enrollmentService = enrollmentService;
    }

    public async Task GenerateAllAsync(int numWydzialy = 3, int numProfesors = 5, int numStudents = 20, int numStudentsMgr = 5, int numKursy = 10)
    {
        var faker = new Faker("pl");

        // === Wydzialy ===
        var wydzialy = new List<Wydzial>();
        for (int i = 0; i < numWydzialy; i++)
        {
            var wydzial = await _wydzialService.CreateWydzialAsync(nazwyWydzialow[faker.Random.Int(0, nazwyWydzialow.Count-1)]);
            wydzialy.Add(wydzial);
        }

        // === Profesorzy ===
        var profesors = new List<Profesor>();
        for (int i = 0; i < numProfesors; i++)
        {
            var adres = new Adres
            {
                Ulica = faker.Address.StreetName(),
                NumerDomu = faker.Random.Int(1, 100).ToString(),
                Miasto = faker.Address.City(),
                KodPocztowy = faker.Address.ZipCode()
            };

            var wydzial = faker.PickRandom(wydzialy);

            var profesor = await _profesorService.CreateProfesorAsync(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                tytuly[faker.Random.Int(0, tytuly.Count-1)],
                adres,
                wydzial.Id,
                null
            );

            profesors.Add(profesor);
        }

        // === Gabinety (optional) ===
        for (int i = 0; i < numProfesors; i++)
        {
            await _gabinetService.CreateGabinetAsync($"G-{i + 1}", profesors[i].Id);
        }

        // === Studenci ===
        var students = new List<Student>();
        for (int i = 0; i < numStudents; i++)
        {
            var adres = new Adres
            {
                Ulica = faker.Address.StreetName(),
                NumerDomu = faker.Random.Int(1, 100).ToString(),
                Miasto = faker.Address.City(),
                KodPocztowy = faker.Address.ZipCode()
            };

            var student = await _studentService.CreateStudentAsync(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Random.Int(1, 3),
                adres
            );

            students.Add(student);
        }

        // === Studenci mgr ===
        for (int i = 0; i < numStudentsMgr; i++)
        {
            var adres = new Adres
            {
                Ulica = faker.Address.StreetName(),
                NumerDomu = faker.Random.Int(1, 100).ToString(),
                Miasto = faker.Address.City(),
                KodPocztowy = faker.Address.ZipCode()
            };

            var promotor = faker.PickRandom(profesors);

            await _studentMgrService.CreateStudentMgrAsync(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Random.Int(1, 3),
                adres,
                faker.Lorem.Sentence(5), // temat pracy dyplomowej
                promotor.Id
            );
        }

        // === Kursy ===
        var kursy = new List<Kurs>();
        for (int i = 0; i < numKursy; i++)
        {
            var prowadzacy = faker.PickRandom(profesors);
            var wydzial = faker.PickRandom(wydzialy);

            var kurs = await _kursService.CreateKursAsync(
                nazwyKursow[faker.Random.Int(0, nazwyKursow.Count-1)],
                $"K{i + 1:D3}",
                faker.Random.Int(1, 7),
                prowadzacy.Id,
                wydzial.Id
            );

            kursy.Add(kurs);
        }

        // === Prerequisites (random) ===
        foreach (var kurs in kursy)
        {
            var possiblePrereqs = kursy.Where(k => k.Id != kurs.Id).ToList();
            int numPrereqs = faker.Random.Int(0, Math.Min(3, possiblePrereqs.Count));
            var prereqList = faker.PickRandom(possiblePrereqs, numPrereqs).Select(k => k.Id).ToList();

            foreach (var preId in prereqList)
            {
                await _kursService.AddPrerequisiteAsync(kurs.Id, preId);
            }
        }

        // === Enrollments ===
        foreach (var student in students)
        {
            // Each student enrolls in 2-5 courses randomly
            int coursesToEnroll = faker.Random.Int(2, Math.Min(5, kursy.Count));
            var chosenCourses = faker.PickRandom(kursy, coursesToEnroll);

            foreach (var kurs in chosenCourses)
            {
                int semester = faker.Random.Int(1, _maxSemester);

                // Enroll student in course
                await _enrollmentService.EnrollStudentAsync(student.Id, kurs.Id, semester);

                // Optionally assign a random grade (simulate finished courses)
                if (faker.Random.Double(0D, 1D) < 0.6D)
                {
                    double grade = Math.Round(faker.Random.Double(2.0, 5.0), 1);
                    await _enrollmentService.UpdateGradeAsync(student.Id, kurs.Id, grade);
                }
            }
        }
    }
}
