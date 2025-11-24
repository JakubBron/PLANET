using Application.QueryDTOs;
using Application.Services;

namespace Application.Interfaces;

public interface IQueryService
{
    Task<ProfesorWynikDTO?> GetProfesorZNajwiekszaLiczbaStudentow();
    Task<List<GpaKursDTO>> GetGpaDlaWydzialu(int wydzialId);
    Task<TrudnoscStudentaDTO?> GetNajtrudniejszyPlan();
}