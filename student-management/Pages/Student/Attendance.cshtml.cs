using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class AttendanceModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AttendanceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<AttendanceSummaryViewModel> AttendanceSummary { get; set; } = new List<AttendanceSummaryViewModel>();
        public int SelectedSubjectId { get; set; }
        public int StudentId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? subjectId)
        {
            StudentId = int.Parse(User.FindFirst("StudentId")?.Value ?? "0");

            Subjects = await _context.Scores
                .Where(s => s.StudentId == StudentId)
                .Select(s => s.Subject)
                .Distinct()
                .ToListAsync();

            if (subjectId.HasValue)
            {
                SelectedSubjectId = subjectId.Value;
                await LoadAttendanceSummary(subjectId.Value);
            }

            return Page();
        }

        private async Task LoadAttendanceSummary(int subjectId)
        {
            var attendanceRecords = await _context.Attendances
                .Where(a => a.StudentId == StudentId && a.Schedule.SubjectId == subjectId)
                .Include(a => a.Schedule)
                .ToListAsync();

            AttendanceSummary = attendanceRecords
                .GroupBy(a => a.Status)
                .Select(g => new AttendanceSummaryViewModel
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                    SubjectName = g.First().Schedule.Subject.SubjectName
                }).ToList();
        }
    }

    public class AttendanceSummaryViewModel
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public string SubjectName { get; set; }
    }
}
