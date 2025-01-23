using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{

    public class books
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string? title { get; set; }

        [Required]
        public int publicationyear { get; set; }

        [Required]
        public int authorid { get; set; }

        public authors? author { get; set; }
    }

}
