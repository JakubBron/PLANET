using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Application;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup DI
        var services = new ServiceCollection();
        services.AddApplicationServices();

        var sp = services.BuildServiceProvider();
        var generator = sp.GetRequiredService<BogusGenerator>();
        var gabinetService = sp.GetRequiredService<IGabinetService>();
        var kursService = sp.GetRequiredService<IKursService>();
        var licznikService = sp.GetRequiredService<ILicznikIndeksowService>();
        var profesorService = sp.GetRequiredService<IProfesorService>();
        var studentService = sp.GetRequiredService<IStudentService>();
        var studentStudiowMgrService = sp.GetRequiredService<IStudentStudiowMgrService>();
        var wydzialService = sp.GetRequiredService<IWydzialService>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== University Management ===");
            Console.WriteLine("1. Uruchom generator danych");
            Console.WriteLine("2. Lista studentów");
            Console.WriteLine("3. Lista profesorów");
            Console.WriteLine("4. Lista kursów");
            Console.WriteLine("5. Lista liczników");
            Console.WriteLine("6. Usuń studenta / profesora / kurs");
            Console.WriteLine("7. Kursy studenta");
            Console.WriteLine("8. Stwórz studenta");
            Console.WriteLine("0. Wyjście");
            Console.Write("Wybierz opcję: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await generator.GenerateAllAsync();
                    Console.WriteLine("Generator danych zakończył działanie. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;

                case "2":
                    var students = await studentService.GetAllStudentsAsync();
                    if (students.Count == 0)
                    {
                        Console.WriteLine("Nothing to display.");
                    }
                    foreach (var s in students)
                        Console.WriteLine($"{s.Id}: {s.Imie} {s.Nazwisko}, {s.IndeksUczelniany}, {s.AdresZamieszkania.Ulica}, {s.AdresZamieszkania.Miasto}");
                    Console.ReadKey();
                    break;

                case "3":
                    var professors = await profesorService.GetAllProfesorzyAsync();
                    foreach (var p in professors)
                        Console.WriteLine($"{p.Id}: {p.Imie} {p.Nazwisko}, {p.IndeksUczelniany}, {p.TytulNaukowy}");
                    Console.ReadKey();
                    break;

                case "4":
                    var kursy = await kursService.GetAllKursyAsync();
                    foreach (var k in kursy)
                        Console.WriteLine($"{k.Id}: {k.Nazwa} ({k.Kod}), {k.ECTS} ECTS, Wydział: {k.Wydzial?.Nazwa}, Prowadzący: {k.Prowadzacy?.Imie} {k.Prowadzacy?.Nazwisko}");
                    Console.ReadKey();
                    break;

                case "5":
                    var licznikList = await licznikService.GetAllAsync();
                    foreach (var l in licznikList)
                        Console.WriteLine($"Prefix: {l.Prefix}, AktualnaWartosc: {l.AktualnaWartosc}");
                    Console.ReadKey();
                    break;

                case "6":
                    Console.WriteLine("Podaj typ encji do usunięcia (student/profesor/kurs):");
                    var type = Console.ReadLine()?.ToLower();
                    Console.WriteLine("Podaj ID:");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        break;
                    }

                    switch (type)
                    {
                        case "student":
                            await studentService.DeleteStudentAsync(id);
                            Console.WriteLine("Student usunięty.");
                            break;
                        case "profesor":
                            await profesorService.DeleteProfesorAsync(id);
                            Console.WriteLine("Profesor usunięty.");
                            break;
                        case "kurs":
                            await kursService.DeleteKursAsync(id);
                            Console.WriteLine("Kurs usunięty.");
                            break;
                        default:
                            Console.WriteLine("Nieznany typ encji.");
                            break;
                    }
                    Console.ReadKey();
                    break;

                case "7":
                    var allStudents = await studentService.GetAllStudentsAsync();
                    if (!allStudents.Any())
                    {
                        Console.WriteLine("Brak studenta.");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine("Podaj ID studenta:");
                    foreach (var s in allStudents)
                        Console.WriteLine($"{s.Id}: {s.Imie} {s.Nazwisko}, {s.IndeksUczelniany}");

                    if (!int.TryParse(Console.ReadLine(), out int studentId))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    var studentEnrollments = await sp.GetRequiredService<IEnrollmentService>()
                        .GetAllEnrollmentsAsync();

                    var studentEnr = studentEnrollments
                        .Where(e => e.StudentId == studentId)
                        .ToList();

                    if (!studentEnr.Any())
                    {
                        Console.WriteLine("Student nie jest zapisany do żadnego kursu.");
                    }
                    else
                    {
                        Console.WriteLine($"Kursy dla studenta {studentId}:");
                        foreach (var e in studentEnr)
                        {
                            Console.WriteLine($"Kurs: {e.Kurs?.Nazwa ?? "Unknown"} ({e.Kurs?.Kod ?? "?"}), Semestr: {e.Semestr}, Ocena: {(e.Ocena.HasValue ? e.Ocena.ToString() : "N/A")}");
                        }
                    }
                    Console.ReadKey();
                    break;

                case "8":
                    Console.Write("Imię: ");
                    var imie = Console.ReadLine();

                    Console.Write("Nazwisko: ");
                    var nazwisko = Console.ReadLine();

                    Console.Write("Rok studiów (liczba): ");
                    if (!int.TryParse(Console.ReadLine(), out int rokStudiow))
                    {
                        Console.WriteLine("Niepoprawny rok studiów.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Ulica: ");
                    var ulica = Console.ReadLine();

                    Console.Write("Miasto: ");
                    var miasto = Console.ReadLine();

                    Console.Write("Kod pocztowy: ");
                    var kod = Console.ReadLine();

                    var adres = new Adres
                    {
                        Ulica = ulica,
                        Miasto = miasto,
                        KodPocztowy = kod
                    };

                    try
                    {
                        var newStudent = await studentService.CreateStudentAsync(imie, nazwisko, rokStudiow, adres);
                        Console.WriteLine($"Student utworzony! ID: {newStudent.Id}, Indeks: {newStudent.IndeksUczelniany}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd podczas tworzenia studenta: {ex.Message}");
                    }
                    Console.ReadKey();
                    break;



                case "0":
                    return;
            }
        }
    }
}
