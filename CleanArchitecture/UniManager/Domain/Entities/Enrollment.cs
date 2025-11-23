using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public Student Student { get; set; }
        public Kurs Kurs { get; set; }
        // enrollment data
        public int Semestr { get; set; }
        public double? Ocena { get; set; }      // can be NULL when Student just enroll

        // FK
        public int StudentId { get; set; }
        public int KursId { get; set; }
    }
}