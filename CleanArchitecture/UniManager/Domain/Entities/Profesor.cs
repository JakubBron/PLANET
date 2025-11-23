using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Profesor
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string IndeksUczelniany { get; set; }
        public string TytulNaukowy { get; set; }
        public Adres AdresZamieszkania { get; set; }
        public Gabinet Gabinet { get; set; }
        public Wydzial Wydzial { get; set; }    // 1 profesor pracuje na 1 wydziale
        // WARNING
        // Possible bad data structure, determine if quick enough later
        public ICollection<Kurs> ProwadzoneKursy { get; set; } = new List<Kurs>();

        // FK
        public int WydzialId { get; set; }
        
    }
}
