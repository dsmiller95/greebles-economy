﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI.Plotter.Series
{
    public interface IMultiPlottableSeries
    {
        IEnumerable<IPlottableSeries> GetPlottableSeries();
    }
}