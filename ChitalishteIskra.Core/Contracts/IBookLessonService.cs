using ChitalishteIskra.Core.DTOs.BookLessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Contracts
{
    public interface IBookLessonService
    {
        Task<IEnumerable<BookLessonIndexDto>> GetAllAsync(Guid currentUserId, bool isAdmin, bool isTeacher, bool isStudent);

        Task<BookLessonCreatePageDto> GetCreatePageDataAsync();

        Task<BookLessonTeacherInfoDto> GetTeacherBookingDataAsync(Guid teacherId, DateOnly date);

        Task CreateAsync(CreateBookLessonDto model);
        Task CreateGroupAsync(CreateGroupLessonDto model);

        Task<IEnumerable<GroupLessonInvitationDto>> GetStudentGroupLessonsAsync(Guid studentId);
        Task RespondToGroupLessonAsync(Guid bookLessonId, Guid studentId, bool isAccepted);
    }
}
