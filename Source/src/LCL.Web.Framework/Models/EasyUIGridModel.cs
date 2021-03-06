﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCL.Web.Framework.Models
{
    public class EasyUIGridModel<TModel>
    {
        public EasyUIGridModel()
        {
            total = 0;
            rows = new List<TModel>();
        }
        public int total { get; set; }
        public List<TModel> rows { get; set; }
    }
}
