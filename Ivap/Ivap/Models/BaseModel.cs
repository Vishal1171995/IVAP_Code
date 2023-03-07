using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ivap.Models
{
    public class BaseModel
    {
        [Required]
        public bool IsActive { set; get; }

        [Required]
        public int CreatedBy { set; get; }

        public int EID { set; get; }
    }
}