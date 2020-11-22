using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClonaTwitter.Models
{
    public class ReportModel
    {
        [Required(ErrorMessage ="This field is required.")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Details { get; set; }
    }
}
