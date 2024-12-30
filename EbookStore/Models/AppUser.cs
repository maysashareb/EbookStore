using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client.AppConfig;
using System.ComponentModel.DataAnnotations;

namespace EbookStore.Models

{
    public class AppUser:IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

  
    
}
}
