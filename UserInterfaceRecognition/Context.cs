using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace UserInterfaceRecognition
{
    public class Context: DbContext
    {
        public DbSet<DbRecognitionModel> DataBaseInfo { get; set; }
        public DbSet<ClassInfo> ClassLabelsInfo { get; set; }
        public Context()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ImageRecognitionDataBase;Trusted_Connection=True;");
        }
    }
}
