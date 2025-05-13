using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PlayNexus.Pages.Admin {
    public class UsersModel : PageModel {
        private readonly PlayNexusDbContext _context;

        public List<User> Users { get; set; }

        public UsersModel(PlayNexusDbContext context) {
            _context = context;
        }

        public void OnGet() {
            // Example: get users registered in the last 7 days
            DateTime recentThreshold = DateTime.UtcNow.AddDays(-7);
            Users = _context.Users
                .Where(u => u.CreatedDate >= recentThreshold)
                .ToList();
        }
    }
}
