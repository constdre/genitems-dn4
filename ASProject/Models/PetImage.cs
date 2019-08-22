using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ASProject.Models
{
    public class PetImage : IEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Pet")]
        public int PetId { get; set; }
        public virtual Pet Pet { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDisplayPic { get; set; }
        public byte[] Image { get; set; }

        public PetImage()
        {

        }



    }
}