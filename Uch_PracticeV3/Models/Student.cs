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

    public partial class Student
    {
        [Display(Name = "Код")]
        [Required]
        [MaxLength(20)]
        public string Id { get; set; }

        [Display(Name = "Фамилия")]
        [Required]
        [MaxLength(30)]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "Отчество")]
        [MaxLength(30)]
        public string Patronymic { get; set; }

        [Display(Name = "Код группы")]
        [Required]
        public int GroupId { get; set; }

        [Display(Name = "Код договора")]
        [Required]
        public int ContractId { get; set; }

        [Display(Name = "Код руководителя")]
        [Required]
        public int LeaderId { get; set; }

        [Display(Name = "Название отчета")]
        [MaxLength(50)]
        public string FileNaming { get; set; }

        [Display(Name = "Отчет")]
        public string FileData { get; set; }

        [Display(Name = "Результат")]
        public Nullable<int> Result { get; set; } = 0;
    
        public virtual Contract Contract { get; set; }
        public virtual Group Group { get; set; }
        public virtual Leader Leader { get; set; }
    }
}
