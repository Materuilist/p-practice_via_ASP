using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uch_PracticeV3.Models.Identity
{
    public class LoginModel
    {
        [Required]
        [StringLength(40)]
        [Index(IsUnique =true)]
        public string Email { get; set; }
        [Required]
        [StringLength(30)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}