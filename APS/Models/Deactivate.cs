﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Models
{
    public class Deactivate
    {
        [Required]
        public string Password { get; set; }
    }
}
