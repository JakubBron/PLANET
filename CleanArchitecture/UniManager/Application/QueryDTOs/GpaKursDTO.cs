using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.QueryDTOs
{
    public class GpaKursDTO
    {
        public int KursId { get; set; }
        public string Nazwa { get; set; }
        public string Kod { get; set; }
        public double? SredniaOcen { get; set; }
        public int LiczbaOcenionych { get; set; }
    }
}
