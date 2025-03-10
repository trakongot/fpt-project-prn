using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class ExamScheduleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ExamScheduleModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<Exam> ExamList { get; set; } = new List<Exam>();
        public int SelectedSubjectId { get; set; }
        public int StudentId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? subjectId)
        {
            StudentId = int.Parse(User.FindFirst("StudentId")?.Value ?? "0");

            Subjects = await _context.Exams
                .Where(e => e.Class.Students.Any(s => s.Id == StudentId))
                .Select(e => e.Subject)
                .Distinct()
                .ToListAsync();

            if (subjectId.HasValue)
            {
                SelectedSubjectId = subjectId.Value;
                ExamList = await _context.Exams
                    .Where(e => e.SubjectId == SelectedSubjectId && e.Class.Students.Any(s => s.Id == StudentId))
                    .Include(e => e.Room)
                    .Include(e => e.Teacher)
                    .Include(e => e.Term)
                    .ToListAsync();
            }

            return Page();
        }
    }
}
