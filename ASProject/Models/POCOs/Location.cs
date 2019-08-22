using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASProject.Models.DTO
{
    public class Location
    {
        [Key, ForeignKey("User")]
        public string Id { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        
        public virtual ApplicationUser User { get; set; }
        
    }
}