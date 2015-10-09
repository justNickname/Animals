using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

namespace Animals.Models
{
    public class Location
    {
        public int Id { get; set; }
        [Display(Name = "Место обитания")]
        public string Name { get; set; }

     //   [ForeignKey("Region")]
        public int RegionID { get; set; }
        

        public virtual Region Region { get; set; }

    }
}