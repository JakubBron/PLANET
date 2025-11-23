using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Gabinet
    {
        public int Id { get; set; }
        public string NumerGabinetu { get; set; }
        public Profesor? Profesor { get; set; }

        // FK
        public int? ProfesorId { get; set; }
    }
}
