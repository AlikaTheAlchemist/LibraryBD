using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WebApplication2.Models
{
  
    public class readers
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(20)]
        public string firstname { get; set; }

        [Required]
        [StringLength(20)]
        public string lastname { get; set; }

        [StringLength(20)]
        public string middlename { get; set; }

        [Required]
        public DateOnly birthdate { get; set; }

        [Required]
        public DateOnly registrationdate { get; set; }


        public string FullName
        {
            get
            {
                return $"{lastname} {firstname} {middlename}";
            }
        }
    }

    }
