using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class Adres
    {
        public int Id { get; set; }
        public string Ulica { get; set; }
        public string NumerDomu { get; set; }
        public string Miasto { get; set; }
        public string KodPocztowy { get; set; }
    }
}
