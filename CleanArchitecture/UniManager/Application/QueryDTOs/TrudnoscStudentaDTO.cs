using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.QueryDTOs
{
    public class TrudnoscStudentaDTO
    {
        public int StudentId { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int EctsKursow { get; set; }
        public int EctsPrerekwizytow { get; set; }

        public int Trudnosc => EctsKursow + EctsPrerekwizytow;
    }
}
