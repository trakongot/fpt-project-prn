using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class ScoreModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ScoreModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<ScoreViewModel> ScoreList { get; set; } = new List<ScoreViewModel>();

        [BindProperty(SupportsGet = true)]
        public int SelectedSubjectId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int studentId = int.Parse(User.FindFirst("StudentId")?.Value ?? "0");

            Subjects = await _context.Scores
                .Where(s => s.StudentId == studentId)
                .Select(s => s.Subject)
                .Distinct()
                .ToListAsync();

            if (SelectedSubjectId > 0)
            {
                await LoadScores(SelectedSubjectId, studentId);
            }

            return Page();
        }

        private async Task LoadScores(int subjectId, int studentId)
        {
            ScoreList = await _context.Scores
                .Where(s => s.SubjectId == subjectId && s.StudentId == studentId)
                .Include(s => s.Term)
                .Select(s => new ScoreViewModel
                {
                    StudentName = s.Student.Name,
                    ScoreValue = s.Value,
                    TermName = s.Term.TermName
                })
                .ToListAsync();
        }
    }

    public class ScoreViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public decimal ScoreValue { get; set; }
        public string TermName { get; set; } = string.Empty;
    }
}
