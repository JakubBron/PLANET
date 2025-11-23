using Domain.Entities;

namespace Application.Interfaces;

public interface IEnrollmentService
{
    Task<Enrollment?> UpdateGradeAsync(int studentId, int kursId, double ocena);
    Task<Enrollment?> UpdateSemesterAsync(int enrollmentId, int semestr);
    Task<Enrollment> EnrollStudentAsync(int studentId, int kursId, int semestr);
    Task<List<Enrollment>> GetAllEnrollmentsAsync();
    Task<Enrollment?> GetEnrollmentAsync(int enrollmentId);
    Task DeleteEnrollmentAsync(int enrollmentId);
}