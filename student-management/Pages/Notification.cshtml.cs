using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Pages.Student
{
    public class NotificationModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public NotificationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public async Task OnGetAsync()
        {
            int studentId = int.Parse(User.FindFirst("StudentId")?.Value ?? "0");

            Notifications = await _context.Notifications
                .Where(n => n.ReceiverId == studentId)
                .Include(n => n.Sender)
                .ToListAsync();
        }
    }
}
