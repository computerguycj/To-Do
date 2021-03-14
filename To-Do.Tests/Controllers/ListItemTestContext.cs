using Microsoft.EntityFrameworkCore;
using System;
using To_Do.Data;

namespace To_Do.Controllers.Tests
{
    public class ListItemTestContext : ListItemContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ToDoTest;Trusted_Connection=True;MultipleActiveResultSets=true");
            base.OnConfiguring(optionsBuilder);
        }
    }
}