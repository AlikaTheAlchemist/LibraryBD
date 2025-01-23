using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace WebApplication2.Models
{
    
    public class orders
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int readerid { get; set; }
        [Required]
        public int bookid { get; set; }

        [Required]
        public DateOnly? orderdate { get; set; }

        [Required]
        public DateOnly? returndate { get; set; }

        // Навигационные свойства
        public readers? Reader { get; set; }
        public books? Book { get; set; }
    }

}
