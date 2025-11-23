using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StudentStudiowMgr: Student
    {
        public string TematPracyDyplomowej { get; set; }
        public Profesor Promotor { get; set; }

        // FK
        public int? PromotorId { get; set; }
        
    }
}
