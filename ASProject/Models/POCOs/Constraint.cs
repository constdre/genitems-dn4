using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASProject.Models.DTO
{
    public class Constraint
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Pet")]
        public int PetId { get; set; }
        public virtual Pet Pet { get; set; }
        public string Description { get; set; }

    }
}