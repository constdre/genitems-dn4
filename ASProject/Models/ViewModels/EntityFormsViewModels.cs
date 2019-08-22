using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ASProject.Models;
using ASProject.Models.DTO;

namespace ASProject.Models.ViewModels
{
    public class ManagePetStaticVM : ManagePetBaseVM
    {
        //fixed, singular fields
        [Display(Name = "Constraints")]
        public string Constraints { get; set; }

    }

    public class ManagePetDynamicVM : ManagePetBaseVM
    {
        //multiple, variable entries
        [Display(Name = "Constraints")]
        public List<Constraint> Constraints { get; set; }


    }
}