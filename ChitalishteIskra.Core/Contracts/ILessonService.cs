using ChitalishteIskra.Core.DTOs.Lessons;

namespace ChitalishteIskra.Core.Contracts
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonDto>> GetAllAsync();

        Task<IEnumerable<LessonDto>> GetByTeacherIdAsync(Guid teacherId);

        Task<IEnumerable<LessonDto>> GetAssignedToTeacherAsync(Guid teacherId);

        Task CreateAsync(CreateLessonDto model);

        Task CreateForTeacherAsync(TeacherCreateLessonDto model);

        Task<LessonDto?> GetByIdAsync(Guid id);

        Task UpdateAsync(Guid id, CreateLessonDto model);

        Task DeleteAsync(Guid id);
    }
}