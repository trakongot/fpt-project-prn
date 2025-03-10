[Authorize(Roles = "Teacher,Admin")]
public class AttendanceModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public AttendanceModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<AttendanceViewModel> AttendanceList { get; set; } = new List<AttendanceViewModel>();
    public int SelectedClassId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int TermId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var teacherId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        AttendanceList = await _context.Attendances
            .Where(a => a.Schedule.Class.TeacherId == teacherId && a.Schedule.TermId == TermId)
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ToListAsync();

        return Page();
    }

    public class AttendanceViewModel
    {
        public string StudentName { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
    }
}
