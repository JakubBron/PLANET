using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LicznikIndeksow
    {
        public string Prefix { get; set; } = null!;

        public int AktualnaWartosc { get; set; }
    }
}
