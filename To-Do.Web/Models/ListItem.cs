using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace To_Do.Models
{
    public class ListItem
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public bool Completed { get; set; }
    }
}
