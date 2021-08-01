using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class Ad
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Image ID")]
        public int AdId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "File Name")]
        [StringLength(50, MinimumLength = 5)]
        public string FileName { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Image")]
        public string Url { get; set; }

    }
}
