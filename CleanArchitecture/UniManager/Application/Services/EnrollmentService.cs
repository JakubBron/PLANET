using Application.Interfaces;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class EnrollmentService: IEnrollmentService
    {
        private readonly UniversityDbContext _context;

        public EnrollmentService(UniversityDbContext context)
        {
            _context = context;
        }
        public async Task<Enrollment> EnrollStudentAsync(int studentId, int kursId, int semestr)
        {
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                KursId = kursId,
                Semestr = semestr
            };

            _context.Enrollmenty.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        // Assign or update a student's grade for a specific course
        public async Task<Enrollment?> UpdateGradeAsync(int studentId, int kursId, double ocena)
        {
            var enrollment = await _context.Enrollmenty
                .SingleOrDefaultAsync(e => e.StudentId == studentId && e.KursId == kursId);

            if (enrollment == null) return null;

            enrollment.Ocena = ocena;
            await _context.SaveChangesAsync();
            return enrollment;
        }

        // Update semester for a specific enrollment
        public async Task<Enrollment?> UpdateSemesterAsync(int enrollmentId, int semestr)
        {
            var enrollment = await _context.Enrollmenty.FindAsync(enrollmentId);
            if (enrollment == null) return null;

            enrollment.Semestr = semestr;
            await _context.SaveChangesAsync();
            return enrollment;
        }


        // Read enrollments
        public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
        {
            return await _context.Enrollmenty
                .Include(e => e.Student)
                .Include(e => e.Kurs)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentAsync(int enrollmentId)
        {
            return await _context.Enrollmenty
                .Include(e => e.Student)
                .Include(e => e.Kurs)
                .SingleOrDefaultAsync(e => e.Id == enrollmentId);
        }

        // Delete enrollment
        public async Task DeleteEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _context.Enrollmenty.FindAsync(enrollmentId);
            if (enrollment == null) return;

            _context.Enrollmenty.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}
