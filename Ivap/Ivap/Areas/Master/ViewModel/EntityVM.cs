using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ivap.Areas.Master.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ivap.Areas.Master.ViewModel
{
    public class EntityVM
    {
        [Required]
        public EntityModel EntityModel { set; get; }
        public SelectList StateList { get; set; }
        public SelectList CountryList { get; set; }
        public SelectList CurrencyList { get; set; }
    }
}