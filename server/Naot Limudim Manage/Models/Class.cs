﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Naot_Limudim_Manage.Models
{
    public class Class
    {
        public int ID { get; set; }
        [Display(Name="נושא")]
        public String Topic { get; set; }
        [Display(Name = "מטרה")]
        public String Target { get; set; }
        [Display(Name = "הערות")]
        public String Description { get; set; }
        [Display(Name = "מ-")]
        public DateTime Start { get; set; }
        [Display(Name = "עד-")]
        public DateTime End { get; set; }
        public virtual ICollection<Present> Presents { get; set; }
    }
}