using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cn.blockstudio.unityeventbus
{

    

    [AttributeUsage(AttributeTargets.Method)]
    public class SubscriberMain : Attribute
    {
        public ExecuteType CallbackExecuteType { get; set; }
        public ExecutePriority CallbackPriority { get; set; }
    }
}


