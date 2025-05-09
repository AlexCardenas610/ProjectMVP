using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlayNexus.Pages.Admin {
    public class UsersModel : PageModel {
        private readonly PlayNexusDbContext _context;

        public List<User> Users { get; set; }

        public UsersModel(PlayNexusDbContext context) {
            _context = context;
        }

        public void OnGet() {
            Users = _context.Users.ToList();
        }
    }
}
