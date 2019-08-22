using ASProject.Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASProject.Models
{
    public class Pet : IEntity
    {
        
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //for auto-increment
        public int Id { get; set; }
        public string UserId {get;set;} //make a foreign key
        public string Kind { get; set; }
        public string Breed { get; set; }
        public string Name { get; set; }
        public virtual List<Constraint> Constraints { get; set; }
        public virtual List<PetImage> Images { get; set; }


    }
}