﻿using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Models
{
    public class Attribute
    {

        public string Name;
        public int Value;

        public Attribute(string name, int value)
        {
            Name = name;
            Value = value;
        }

    }
}