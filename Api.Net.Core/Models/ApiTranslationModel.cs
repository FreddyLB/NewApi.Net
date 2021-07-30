﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Net.Core.Models
{
    class ApiTranslationModel
    {
        public string Endpoint { get; set; }
        public IEnumerable<(string, string)> Controllers { get; set; } = new List<(string, string)>();
    }
}
