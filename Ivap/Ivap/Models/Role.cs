using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
namespace Ivap.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Desc")]
        public string Desc { get; set; }
        public string Type { get; set; }
        public bool IsAct { get; set; }
        public string CreatedBy { get; set; }
        public int UID { get; set; }
        public DataSet DsRole { get; set; }
    }
}