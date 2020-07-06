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
        private int callbackPriority;
        public int CallbackPriority { 
            
            get {
                return callbackPriority;
            
            } set {

                if (value < 0)
                    callbackPriority = 0;
                else if (value > 99)
                    callbackPriority = 99;
                else
                    callbackPriority = value;

            } 
        
        }
    }
}


