﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Utilities.TreeEventSystem
{
    public interface ITreeEvent
    {
        TreeEventType EventType { get; }
    }
}
