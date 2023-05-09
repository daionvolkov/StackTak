using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace StackTak.Models
{

    [Table("User_List")]
    public class User_List 
    {
        [Key]
        public int ID { get; set; }
        //public string FirstName {get; set; } 
        //public string LastName {get; set; }    
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
        //public string Email {get; set; }


        public int AccessRights { get; set; }

    }
}