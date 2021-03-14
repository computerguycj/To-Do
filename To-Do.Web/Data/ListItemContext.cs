using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using To_Do.Models;

namespace To_Do.Data
{
    public class ListItemContext : DbContext
    {
        public ListItemContext()
        {
        }

        public ListItemContext(DbContextOptions<ListItemContext> options)
            : base(options)
        {
        }

        public DbSet<ListItem> ListItems { get; set; }
    }
}
