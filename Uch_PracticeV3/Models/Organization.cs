//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Uch_PracticeV3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public partial class Organization:IValidatableObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Organization()
        {
            this.Contracts = new HashSet<Contract>();
        }
    
        [Display(Name ="ОКПО")]
        [Required]
        [StringLength(8)]
        public string Id { get; set; }
        [Display(Name = "Краткое название")]
        [MaxLength(20)]
        public string ShortNaming { get; set; }
        [Display(Name = "Полное название")]
        [Required]
        [MaxLength(30)]
        public string FullNaming { get; set; }
        [Display(Name = "Код отрасли")]
        [Required]
        [MaxLength(10)]
        public string SectorId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual Sector Sector { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (new Regex(@"^\d{8}$").Matches(this.Id).Count <= 0)
            {
                errors.Add(new ValidationResult("Код ОКПО не соответствует шаблону"));
            }

            return errors;
        }
    }
}
