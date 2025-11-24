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
        var enrollmentService = sp.GetRequiredService<IEnrollmentService>();
        var queryService = sp.GetRequiredService<IQueryService>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== University Management ===");
            Console.WriteLine("1. Uruchom generator danych");
            Console.WriteLine("2. Lista studentów");
            Console.WriteLine("\t21. Stwórz");
            Console.WriteLine("\t22. Edytuj");
            Console.WriteLine("3. Lista profesorów");
            Console.WriteLine("\t31. Stwórz");
            Console.WriteLine("\t32. Edytuj");
            Console.WriteLine("4. Lista kursów");
            Console.WriteLine("\t41. Stwórz");
            Console.WriteLine("\t42. Edytuj");
            Console.WriteLine("5. Lista liczników");
            Console.WriteLine("\t51. Stwórz");
            Console.WriteLine("\t52. Edytuj");
            Console.WriteLine("\t53. Usuń");
            Console.WriteLine("6. Lista gabinetów");
            Console.WriteLine("\t61. Stwórz");
            Console.WriteLine("\t62. Edytuj");
            Console.WriteLine("7. Lista wydziałów");
            Console.WriteLine("\t71. Stwórz");
            Console.WriteLine("\t72. Edytuj");
            Console.WriteLine("\t73. Usuń");
            Console.WriteLine("8. Lista kursów studenta");
            Console.WriteLine("\t81. Zapisz");
            Console.WriteLine("\t82. Skreśl");
            Console.WriteLine("\t83. Oceń");
            Console.WriteLine("9. Usuń studenta / profesora / gabinet / kurs");
            Console.WriteLine("  Raporty");
            Console.WriteLine("\t101. Profesor z największą liczbą studentów");
            Console.WriteLine("\t102. GPA dla wydziału");
            Console.WriteLine("\t103. Najtrudniejszy plan");
            Console.WriteLine("0. Wyjście");
            Console.Write("Wybierz opcję: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    try
                    {
                        await generator.GenerateAllAsync();
                        Console.WriteLine("Generator danych zakończył działanie. Naciśnij dowolny klawisz...");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Console.ReadKey();
                    break;

                case "2": 
                    var students = await studentService.GetAllStudentsAsync();
                    if (students.Count == 0)
                    {
                        Console.WriteLine("Nothing to display.");
                    }

                    Console.WriteLine("Wszyscy:");
                    foreach (var s in students)
                        Console.WriteLine($"{s.Id}: {s.Imie} {s.Nazwisko}, {s.IndeksUczelniany}, {s.AdresZamieszkania.Ulica}, {s.AdresZamieszkania.Miasto}");

                    var studentsMgr = await studentStudiowMgrService.GetAllStudentsMgrAsync();
                    if (studentsMgr.Count == 0)
                    {
                        Console.WriteLine("Nothing to display.");
                    }

                    Console.WriteLine("Studia II stopnia:");
                    foreach (var s in studentsMgr)
                        Console.WriteLine($"{s.Id}: {s.Imie} {s.Nazwisko}, {s.IndeksUczelniany}, {s.AdresZamieszkania.Ulica}, {s.AdresZamieszkania.Miasto} | {s.Promotor.TytulNaukowy} {s.Promotor.Nazwisko}, {s.TematPracyDyplomowej}");

                    Console.ReadKey();
                    break;

                case "21":
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
                    
                    Console.Write("Numer domu: ");
                    var numerDomu = Console.ReadLine();

                    Console.Write("Miasto: ");
                    var miasto = Console.ReadLine();

                    Console.Write("Kod pocztowy: ");
                    var kod = Console.ReadLine();

                    var adres = new Adres
                    {
                        Ulica = ulica,
                        NumerDomu = numerDomu,
                        Miasto = miasto,
                        KodPocztowy = kod
                    };

                    Console.Write("Czy student mgr? (t/n): ");
                    var isMgr = Console.ReadLine()?.ToLower() == "t";
                    if (isMgr)
                    {
                        Console.Write("Promotor (id): ");
                        if (!int.TryParse(Console.ReadLine(), out int promotorId))
                        {
                            Console.WriteLine("Niepoprawny Id promotora.");
                            Console.ReadKey();
                            break;
                        }
                        Console.Write("Temat pracy dyplomowej: ");
                        var tematPracy = Console.ReadLine();

                        try
                        {
                            var newStudentMgr = await studentStudiowMgrService.CreateStudentMgrAsync(imie, nazwisko, rokStudiow, adres, tematPracy, promotorId);
                            Console.WriteLine($"Student mgr utworzony! ID: {newStudentMgr.Id}, Indeks: {newStudentMgr.IndeksUczelniany}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Błąd podczas tworzenia studenta mgr: {ex.Message}");
                        }
                    }
                    else
                    {
                        try
                        {
                            var newStudent = await studentService.CreateStudentAsync(imie, nazwisko, rokStudiow, adres);
                            Console.WriteLine($"Student utworzony!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Błąd podczas tworzenia studenta: {ex.Message}");
                        }
                    }
                    Console.ReadKey();
                    break;

                case "22":
                    Console.Write("Podaj ID studenta do edycji: ");
                    if (!int.TryParse(Console.ReadLine(), out int sid))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    var st = await studentService.GetStudentByIdAsync(sid);
                    if (st == null)
                    {
                        Console.WriteLine("Student nie istnieje.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Nowe imię (ENTER = bez zmian): ");
                    var newImie = Console.ReadLine();

                    Console.Write("Nowe nazwisko (ENTER = bez zmian): ");
                    var newNazwisko = Console.ReadLine();

                    Console.Write("Nowy rok studiów (ENTER = bez zmian): ");
                    var rokStr = Console.ReadLine();
                    int? newRok = int.TryParse(rokStr, out var parsedRok) ? parsedRok : null;

                    Console.Write("Zmienić adres? (t/n): ");
                    var changeAddress = Console.ReadLine()?.ToLower();

                    Adres? newAdresStud = null;

                    if (changeAddress == "t")
                    {
                        Console.Write("Ulica: ");
                        var ulicaStud = Console.ReadLine();

                        Console.Write("Numer domu: ");
                        var nrStud = Console.ReadLine();

                        Console.Write("Miasto: ");
                        var miastoStud = Console.ReadLine();

                        Console.Write("Kod pocztowy: ");
                        var kodStud = Console.ReadLine();

                        newAdresStud = new Adres
                        {
                            Ulica = ulicaStud,
                            NumerDomu= nrStud,
                            Miasto = miastoStud,
                            KodPocztowy = kodStud
                        };
                    }

                    var updatedStudent = await studentService.UpdateStudentAsync(
                        sid,
                        string.IsNullOrWhiteSpace(newImie) ? null : newImie,
                        string.IsNullOrWhiteSpace(newNazwisko) ? null : newNazwisko,
                        newRok,
                        newAdresStud
                    );

                    if (updatedStudent == null)
                        Console.WriteLine("Student nie istnieje.");
                    else
                        Console.WriteLine("Student został zaktualizowany.");

                    Console.ReadKey();
                    break;

                case "3":
                    var professors = await profesorService.GetAllProfesorzyAsync();
                    foreach (var p in professors)
                        Console.WriteLine($"{p.Id}: {p.Imie} {p.Nazwisko}, {p.IndeksUczelniany}, {p.TytulNaukowy}, gabinet {p.Gabinet?.NumerGabinetu}");
                    Console.ReadKey();
                    break;

                case "31":
                    Console.Write("Imię: ");
                    var pimie = Console.ReadLine();

                    Console.Write("Nazwisko: ");
                    var pnazwisko = Console.ReadLine();

                    Console.Write("Tytuł naukowy: ");
                    var ptytul = Console.ReadLine();

                    Console.Write("Ulica: ");
                    var pulica = Console.ReadLine();

                    Console.Write("Numer domu: ");
                    var pnumerdomu = Console.ReadLine();

                    Console.Write("Miasto: ");
                    var pmiasto = Console.ReadLine();

                    Console.Write("Kod pocztowy: ");
                    var pkod = Console.ReadLine();

                    var padres = new Adres { Ulica = pulica, NumerDomu = pnumerdomu, Miasto = pmiasto, KodPocztowy = pkod };

                    Console.Write("ID wydziału: ");
                    int wydzialId = int.Parse(Console.ReadLine());

                    Console.Write("ID gabinetu (lub puste): ");
                    var ginput = Console.ReadLine();
                    int? pgabinetId = int.TryParse(ginput, out var gval) ? gval : null;

                    try
                    {
                        var newProf = await profesorService.CreateProfesorAsync(
                            pimie, pnazwisko, ptytul, padres, wydzialId, pgabinetId
                        );
                        Console.WriteLine($"Profesor utworzony! ID: {newProf.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                    }
                    Console.ReadKey();
                    break;

                case "32":
                    Console.Write("ID profesora: ");
                    if (!int.TryParse(Console.ReadLine(), out int pid))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine("ENTER = bez zmian");

                    Console.Write("Nowe imię: ");
                    var pim = Console.ReadLine();

                    Console.Write("Nowe nazwisko: ");
                    var pna = Console.ReadLine();

                    Console.Write("Nowy tytuł: ");
                    var pty = Console.ReadLine();

                    Console.Write("Nowy ID wydziału: ");
                    var winput = Console.ReadLine();
                    int? newWid = int.TryParse(winput, out var wparsed) ? wparsed : null;

                    Console.Write("Nowy ID gabinetu: ");
                    var ginput2 = Console.ReadLine();
                    int? newGid = int.TryParse(ginput2, out var gparsed) ? gparsed : null;

                    Console.Write("Zmienić adres? (t/n): ");
                    var adr = Console.ReadLine();

                    Adres? newAdres = null;
                    if (adr?.ToLower() == "t")
                    {
                        Console.Write("Ulica: ");
                        var u = Console.ReadLine();
                        Console.Write("Numer domu: ");
                        var nr = Console.ReadLine();
                        Console.Write("Miasto: ");
                        var m = Console.ReadLine();
                        Console.Write("Kod pocztowy: ");
                        var k = Console.ReadLine();

                        newAdres = new Adres { Ulica = u, NumerDomu = nr, Miasto = m, KodPocztowy = k };
                    }

                    var updatedProf = await profesorService.UpdateProfesorAsync(
                        pid,
                        string.IsNullOrWhiteSpace(pim) ? null : pim,
                        string.IsNullOrWhiteSpace(pna) ? null : pna,
                        string.IsNullOrWhiteSpace(pty) ? null : pty,
                        newAdres,
                        newWid,
                        newGid
                    );

                    Console.WriteLine(updatedProf == null ? "Profesor nie istnieje." : "Profesor zaktualizowany.");
                    Console.ReadKey();
                    break;

                case "4":
                    var kursy = await kursService.GetAllKursyAsync();
                    foreach (var k in kursy)
                    {
                        Console.WriteLine($"{k.Id}: {k.Nazwa} ({k.Kod}), {k.ECTS} ECTS, Wydział: {k.Wydzial?.Nazwa}, Prowadzący: {k.Prowadzacy?.Imie} {k.Prowadzacy?.Nazwisko}");
                        Console.Write("\twymagane: ");
                        foreach (var prereq in k.Prerequisites)
                        {
                            Console.Write($"{prereq.Nazwa} (id={prereq.Id}), ");
                        }
                        Console.WriteLine();
                    }
                    Console.ReadKey();
                    break;

                case "41":
                    Console.Write("Nazwa kursu: ");
                    var kn = Console.ReadLine();

                    Console.Write("Kod kursu: ");
                    var kcod = Console.ReadLine();

                    Console.Write("ECTS: ");
                    int ects = int.Parse(Console.ReadLine());

                    Console.Write("ID prowadzącego: ");
                    int prowId = int.Parse(Console.ReadLine());

                    Console.Write("ID wydziału: ");
                    int wydId = int.Parse(Console.ReadLine());

                    Console.Write("Lista ID kursów wymaganych (np. 1,2,3) lub puste: ");
                    var pr = Console.ReadLine();
                    List<int>? reqs = pr?.Length > 0 ? pr.Split(',').Select(int.Parse).ToList() : null;

                    var newK = await kursService.CreateKursAsync(kn, kcod, ects, prowId, wydId, reqs);
                    Console.WriteLine($"Kurs utworzony! ID = {newK.Id}");
                    Console.ReadKey();
                    break;

                case "42":
                    Console.Write("ID kursu: ");
                    if (!int.TryParse(Console.ReadLine(), out int kid))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Nowa nazwa (ENTER = bez zmian): ");
                    var kn2 = Console.ReadLine();

                    Console.Write("Nowy kod (ENTER = bez zmian): ");
                    var kc2 = Console.ReadLine();

                    Console.Write("Nowe ECTS (ENTER = bez zmian): ");
                    var e2 = Console.ReadLine();
                    int? ects2 = int.TryParse(e2, out var eparsed) ? eparsed : null;

                    Console.Write("Nowy ID prowadzącego (ENTER = bez zmian): ");
                    var pr2 = Console.ReadLine();
                    int? newPrId = int.TryParse(pr2, out var pparsed) ? pparsed : null;

                    Console.Write("Nowy ID wydziału (ENTER = bez zmian): ");
                    var w2 = Console.ReadLine();
                    int? newWId = int.TryParse(w2, out var wparsed2) ? wparsed2 : null;

                    Console.Write("Nadpisać listę prerequisites? (t/n): ");
                    var prq = Console.ReadLine();
                    List<int>? newReqs = null;
                    if (prq.ToLower() == "t")
                    {
                        Console.Write("Podaj ID prerequisite (np 1,2,3) lub puste: ");
                        var raw = Console.ReadLine();
                        newReqs = raw?.Length > 0 ? raw.Split(',').Select(int.Parse).ToList() : null;
                    }

                    var updatedKurs = await kursService.UpdateKursAsync(
                        kid,
                        string.IsNullOrWhiteSpace(kn2) ? null : kn2,
                        string.IsNullOrWhiteSpace(kc2) ? null : kc2,
                        ects2,
                        newPrId,
                        newWId,
                        newReqs
                    );

                    Console.WriteLine(updatedKurs != null ? "Kurs zaktualizowany." : "Kurs nie istnieje.");
                    Console.ReadKey();
                    break;

                case "5":
                    var licznikList = await licznikService.GetAllAsync();
                    foreach (var l in licznikList)
                        Console.WriteLine($"Prefix: {l.Prefix}, AktualnaWartosc: {l.AktualnaWartosc}");
                    Console.ReadKey();
                    break;

                case "51":
                    Console.Write("Prefix: ");
                    var lp = Console.ReadLine();

                    Console.Write("Start value: ");
                    int lv = int.Parse(Console.ReadLine());

                    var created = await licznikService.CreatePrefixAsync(lp, lv);
                    Console.WriteLine($"Licznik utworzony: {created.Prefix} = {created.AktualnaWartosc}");
                    Console.ReadKey();
                    break;

                case "52":
                    try
                    {
                        Console.Write("Prefix: ");
                        var prefix = Console.ReadLine();

                        Console.Write("Nowa wartość: ");
                        var nv = Console.ReadLine();
                        int? newVal = int.TryParse(nv, out var vparsed) ? vparsed : null;
                        if(newVal == null)
                        {
                            Console.WriteLine("Niepoprawna wartość.");
                            Console.ReadKey();
                            break;
                        }

                        var updatedL = await licznikService.UpdateAsync(prefix, vparsed);

                        if (updatedL == null)
                            Console.WriteLine("Licznik nie istnieje.");
                        else
                            Console.WriteLine("Licznik zaktualizowany.");
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    Console.ReadKey();
                    break;

                case "53":
                    Console.Write("Prefix do usunięcia: ");
                    var dp = Console.ReadLine();

                    try
                    {
                        await licznikService.DeleteAsync(dp);
                        Console.WriteLine("Licznik usunięty.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                    }

                    Console.ReadKey();
                    break;

                case "6":
                    var gabinety = await gabinetService.GetAllGabinetyAsync();
                    if (!gabinety.Any())
                    {
                        Console.WriteLine("Brak gabinetów.");
                    }
                    else
                    {
                        foreach (var g in gabinety)
                        {
                            Console.WriteLine($"{g.Id}: Gabinet {g.NumerGabinetu}, Profesor: {g.Profesor?.TytulNaukowy} {g.Profesor?.Imie} {g.Profesor?.Nazwisko ?? "Brak"}");
                        }
                    }
                    Console.ReadKey();
                    break;

                case "61":
                    Console.Write("Numer gabinetu: ");
                    var numerGabinetu = Console.ReadLine();

                    Console.Write("ID profesora (lub puste): ");
                    var profInput = Console.ReadLine();

                    int? profId = null;
                    if (int.TryParse(profInput, out int parsed))
                        profId = parsed;

                    try
                    {
                        var g = await gabinetService.CreateGabinetAsync(numerGabinetu, profId);
                        Console.WriteLine($"Gabinet utworzony! ID: {g.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                    }

                    Console.ReadKey();
                    break;

                case "62":
                    Console.Write("Podaj ID gabinetu: ");
                    if (!int.TryParse(Console.ReadLine(), out int gid))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Nowy numer gabinetu (ENTER = bez zmian): ");
                    var newNum = Console.ReadLine();

                    Console.Write("Nowy profesor ID (ENTER = bez zmian): ");
                    var newProfInput = Console.ReadLine();

                    int? newProfId = null;
                    if (int.TryParse(newProfInput, out int parsedProf))
                        newProfId = parsedProf;

                    try
                    {
                        var updated = await gabinetService.UpdateGabinetAsync(
                            gid,
                            string.IsNullOrWhiteSpace(newNum) ? null : newNum,
                            newProfInput == "" ? null : newProfId
                        );

                        if (updated == null)
                            Console.WriteLine("Gabinet nie istnieje.");
                        else
                            Console.WriteLine("Gabinet zaktualizowany.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                    }

                    Console.ReadKey();
                    break;

                case "7":
                    var wydzialy = await wydzialService.GetAllWydzialyAsync();
                    if (!wydzialy.Any()) Console.WriteLine("Nothing to show");
                    else
                    {
                        foreach (var w in wydzialy)
                        {
                            Console.WriteLine($"{w.Id}: {w.Nazwa}");
                        }
                    }
                    Console.ReadKey();
                    break;

                case "71":
                    Console.Write("Podaj nazwę wydziału: ");
                    var nazwa = Console.ReadLine();

                    var newWydzial = await wydzialService.CreateWydzialAsync(nazwa!);
                    Console.WriteLine($"Utworzono wydział.");
                    Console.ReadKey();
                    break;

                case "72":
                    Console.Write("Podaj ID wydziału do edycji: ");
                    if (!int.TryParse(Console.ReadLine(), out int editId))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Podaj nową nazwę: ");
                    var newName = Console.ReadLine();

                    var updatedWydzial = await wydzialService.UpdateWydzialAsync(editId, newName);
                    if (updatedWydzial == null) Console.WriteLine("Wydział nie istnieje.");
                    else Console.WriteLine("Wydział zaktualizowany.");
                    Console.ReadKey();
                    break;

                case "73":
                    Console.Write("Podaj ID wydziału do usunięcia: ");
                    if (!int.TryParse(Console.ReadLine(), out int delId))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    try
                    {
                        await wydzialService.DeleteWydzialAsync(delId);
                        Console.WriteLine("Wydział usunięty.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    Console.ReadKey();
                    break;

                case "8":
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

                    var studentEnrollments = await enrollmentService.GetAllEnrollmentsAsync();

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
                            Console.WriteLine($"Kurs: {e.Kurs?.Nazwa ?? "Unknown"} ({e.Kurs?.Kod ?? "?"}), Semestr: {e.Semestr}, Ocena: {(e.Ocena.HasValue ? e.Ocena.ToString() : "N/A")} | kurId {e.KursId}, studId {e.StudentId}, eId {e.Id}");
                        }
                    }
                    Console.ReadKey();
                    break;

                case "81":
                    Console.Write("Podaj ID studenta: ");
                    if (!int.TryParse(Console.ReadLine(), out int studentIdToEnroll))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Podaj ID kursu: ");
                    if (!int.TryParse(Console.ReadLine(), out int kursId))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Podaj semestr zapisu: ");
                    var input = !int.TryParse(Console.ReadLine(), out int semester);

                    try
                    {
                        var enr = await enrollmentService.EnrollStudentAsync(studentIdToEnroll, kursId, semester);
                        Console.WriteLine($"Student zapisany na kurs.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                        Console.ReadKey();
                    }
                    Console.ReadKey();
                    break;

                case "82":
                    Console.Write("Podaj ID studenta: ");
                    if (!int.TryParse(Console.ReadLine(), out int studentIdWypis))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Podaj ID kursu: ");
                    if (!int.TryParse(Console.ReadLine(), out int kursIdWypis))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        break;
                    }

                    var studentEnrollmentsDelete = await enrollmentService.GetAllEnrollmentsAsync();
                    var enrollmentId = studentEnrollmentsDelete.FirstOrDefault(e => e.StudentId == studentIdWypis && e.KursId == kursIdWypis).Id;
                    if (enrollmentId == null)
                    {
                        Console.WriteLine("Nie znaleziono tego zapisu!");
                        Console.ReadKey();
                        break;
                    }

                    try
                    {
                        await enrollmentService.DeleteEnrollmentAsync(enrollmentId);
                        Console.WriteLine("Student skreślony z kursu");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                        Console.ReadKey();
                    }
                    Console.ReadKey();
                    break;

                case "83":
                    Console.Write("Podaj ID studenta: ");
                    if (!int.TryParse(Console.ReadLine(), out int studentIdOcena))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        return;
                    }

                    Console.Write("Podaj ID kursu: ");
                    if (!int.TryParse(Console.ReadLine(), out int kursIdOcena))
                    {
                        Console.WriteLine("Niepoprawne ID.");
                        Console.ReadKey();
                        return;
                    }

                    Console.Write("Podaj ocenę (2–5): ");
                    if (!double.TryParse(Console.ReadLine(), out double grade))
                    {
                        Console.WriteLine("Niepoprawna ocena.");
                        Console.ReadKey();
                        return;
                    }

                    try
                    {
                        await enrollmentService.UpdateGradeAsync(studentIdOcena, kursIdOcena, grade);
                        Console.WriteLine("Ocena ustawiona.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                        Console.ReadKey();
                    }
                    Console.ReadKey();
                    break;

                case "9":
                    Console.WriteLine("Podaj typ encji do usunięcia (student/profesor/kurs/gabinet):");
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
                        case "gabinet":
                            await gabinetService.DeleteGabinetAsync(id);
                            Console.WriteLine("Gabinet usunięty.");
                            break;
                        default:
                            Console.WriteLine("Nieznany typ encji.");
                            break;
                    }
                    Console.ReadKey();
                    break;

                case "101":
                    var result = await queryService.GetProfesorZNajwiekszaLiczbaStudentow();

                    Console.Clear();
                    Console.WriteLine("=== Raport: Profesor z największą liczbą studentów ===");

                    if (result == null)
                    {
                        Console.WriteLine("Brak danych.");
                        Console.ReadKey();
                        break;
                    }

                    var profesor = await profesorService.GetProfesorByIdAsync(result.ProfesorId);

                    if (profesor == null)
                    {
                        Console.WriteLine($"Profesor o ID {result.ProfesorId} nie istnieje.");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine(
                        $"Profesor: {profesor.TytulNaukowy} {profesor.Imie} {profesor.Nazwisko}\n" +
                        $"Łączna liczba unikalnych studentów: {result.LiczbaStudentow}"
                    );
                    Console.ReadKey();
                    break;

                case "102":
                    Console.Clear();
                    Console.WriteLine("=== Raport: Średnie oceny kursów na wydziale ===\n");

                    var wydzialyRaport = await wydzialService.GetAllWydzialyAsync();

                    foreach (var w in wydzialyRaport)
                        Console.WriteLine($"{w.Id}. {w.Nazwa}");

                    Console.Write("\nWybierz ID wydziału: ");
                    int wId = int.Parse(Console.ReadLine()!);

                    var results = await queryService.GetGpaDlaWydzialu(wId);

                    Console.Clear();
                    Console.WriteLine($"=== Wydział: {wydzialyRaport.First(x => x.Id == wId).Nazwa} ===\n");

                    if (!results.Any())
                    {
                        Console.WriteLine("Brak kursów lub ocen.");
                        Console.ReadKey();
                        return;
                    }

                    foreach (var r in results)
                    {
                        Console.WriteLine(
                            $"{r.Nazwa} ({r.Kod})\n" +
                            $"Średnia ocen: {(r.SredniaOcen.HasValue ? r.SredniaOcen.Value.ToString("0.00") : "brak")}\n" +
                            $"Liczba ocenionych studentów: {r.LiczbaOcenionych}\n"
                        );
                    }

                    Console.ReadKey();
                    break;

                case "103":
                    Console.Clear();
                    Console.WriteLine("=== Raport: Student z najtrudniejszym planem ===\n");
                    var raport = await queryService.GetNajtrudniejszyPlan();

                    if (raport == null)
                    {
                        Console.WriteLine("Brak danych.");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine(
                        $"Student: {raport.Imie} {raport.Nazwisko} (ID: {raport.StudentId})\n" +
                        $"Łączne ECTS kursów + prerekwizytów: {raport.EctsKursow+raport.EctsPrerekwizytow} ({raport.EctsKursow} + {raport.EctsPrerekwizytow})"
                    );
                    Console.ReadKey();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Nieznana opcja. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
