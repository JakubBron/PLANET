using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Kurs
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Kod { get; set; }
        public int ECTS { get; set; }
        public Profesor Prowadzacy { get; set; }
        public Wydzial Wydzial { get; set; }
        // WARNING
        // Possible bad data structure, determine if quick enough later 
        public ICollection<Enrollment> Enrollmenty { get; set; } = new List<Enrollment>();
        public ICollection<Kurs> Prerequisites { get; set; } = new List<Kurs>();    // wymogi dla MNIE
        public ICollection<Kurs> IsPrerequisiteFor { get; set; } = new List<Kurs>(); // jestem wymagany DLA

        // FK
        public int ProfesorId { get; set; }     // 1 kurs jest prowadzony przez 1 profesora
        public int WydzialId { get; set; }      // 1 kurs jest na 1 wydziale
    }
}
