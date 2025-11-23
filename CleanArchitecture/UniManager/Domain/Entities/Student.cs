using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string IndeksUczelniany { get; set; }
        public int RokStudiow { get; set; }
        public Adres AdresZamieszkania { get; set; }
        // WARNING
        // Possible bad data structure, determine if quick enough later 
        public ICollection<Enrollment> Enrollmenty { get; set; } = new List<Enrollment>();  
    }
}
