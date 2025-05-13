using Microsoft.AspNetCore.Identity;
using System;

namespace PlayNexus.Models {
    public class User : IdentityUser {
        // Additional properties can be added here if needed
        public DateTime CreatedDate { get; set; }
    }
}
