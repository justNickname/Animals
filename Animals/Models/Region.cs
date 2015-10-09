using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Animals.Models
{
    public class Region
    {
        public int Id { get; set; }
        [Display(Name = "Область обитания")]
        public string Name { get; set; }

    }
}