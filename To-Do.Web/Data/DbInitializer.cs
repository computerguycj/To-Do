using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using To_Do.Models;

namespace To_Do.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ListItemContext context)
        {
            context.Database.EnsureCreated();

            if (!context.ListItems.Any())
            { SeedDummyData(context); }
        }

        private static void SeedDummyData(ListItemContext context)
        {
            context.AddRange(new[]
            {
                new ListItem { Text = "Feed dog" },
                new ListItem { Completed = true, Text = "Walk dog" },
                new ListItem { Text = "Bury dog (because I didn't feed him, but did walk him to death)" },
                new ListItem { Text = "Buy flowers for wife (because I killed dog)" }
            });
            context.SaveChanges();
        }
    }
}
