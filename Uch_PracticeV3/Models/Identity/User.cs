using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Uch_PracticeV3.Models.Identity
{
    public class User:IValidatableObject
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public int RoleId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (this.Email != null && new Regex(@"^[A-Za-z]+@[A-Za-z]+\.ru$").Matches(this.Email).Count <= 0)
            {
                errors.Add(new ValidationResult("Email не соответствует шаблону"));
            }

            return errors;
        }
    }
}