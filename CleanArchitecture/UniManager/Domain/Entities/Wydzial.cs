using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Wydzial
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        // WARNING
        // Possible bad data structure, determine if quick enough later
        public ICollection<Profesor> Profesorzy { get; set; } = new List<Profesor>();   // 1 wydział ma wielu profesorów
        public ICollection<Kurs> Kursy { get; set; } = new List<Kurs>();                // 1 wydział ma wiele kursów
    }
}
