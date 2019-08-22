using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ASProject.Models.ViewModels
{
    public abstract class ManagePetBaseVM
    {
        [Required(ErrorMessage = "Please enter the pet's name")]
        [Display(Name="Name")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Please enter the kind of pet")]
        [Display(Name ="Kind")]
        public string Kind { get; set; }
        [Required]
        public string Breed { get; set; }
    }
}
