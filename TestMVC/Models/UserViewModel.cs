using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TestMVC.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

       
        [Required(ErrorMessage="Enter First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",ErrorMessage="enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; }

       [Required(ErrorMessage="Enter Password")]
        //[DataType(DataType.Password)]
       [RegularExpression(@"^((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,10})$", ErrorMessage = "Password length 8-10 with lower, high alpha and 1 digit")]
       //[Range(4,10, ErrorMessage="Password Length 4 to 10")]
        public string Password { get; set; }

        
    }
}