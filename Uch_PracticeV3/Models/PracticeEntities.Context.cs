﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UCH_PracticeEntities : DbContext
    {
        public UCH_PracticeEntities()
            : base("name=UCH_PracticeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Leader> Leaders { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Rank> Ranks { get; set; }
        public virtual DbSet<Sector> Sectors { get; set; }
        public virtual DbSet<Specialty> Specialties { get; set; }
        public virtual DbSet<Student> Students { get; set; }
    }
}