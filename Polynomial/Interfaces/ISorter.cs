﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public interface ISorter<T>
    {
        void Sort(ICollection<T> collection);
    }
}
