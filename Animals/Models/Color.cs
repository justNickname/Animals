using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Animals.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Display(Name = "Раскраска")]
        public string Name { get; set; }

    }
}