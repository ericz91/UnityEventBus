using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class UnityEventBus {

    [NonSerialized]
    public string tag = "UnityEventBus";
    private static UnityEventBus defaultInstance;
    [NonSerialized]
    public string MethodNameTag = "eventBusMethod";


    private object lockMutex;

    private Dictionary<Type, List<EventMethod>> subDictionary;

    public enum Priority
    {
        Low=0,
        Middle,
        Hight
    } 


    private UnityEventBus()
    {

        subDictionary = new Dictionary<Type, List<EventMethod>>();
        lockMutex = new object();
    }

    public static UnityEventBus getInstance()
    {
        if(defaultInstance == null)
        {
            defaultInstance = new UnityEventBus();
        }
        return defaultInstance;
    }


    /// <summary>
    /// 注册本类需要订阅的事件
    /// </summary>
    /// <param name="subscriber"></param>
    public void register(object subscriber)
    {
        register(subscriber, Priority.Middle, lockMutex);
    }

    public void register(object subscriber, Priority priority)
    {

    }

    private void  register(object subscriber, Priority priority, object lockMutex)
    {
       
        lock (lockMutex)
        {
            //遍历subscriber类里面事件的回调方法
            List<EventMethod> eventMethods = findEventMethodByClass(subscriber);

            //加入事件到订阅列表
            foreach (EventMethod tmp in eventMethods)
                subscribe(tmp);

        }
        
    }


    public void unregister(object subscriber, Type eventType){

        if (!subDictionary.ContainsKey(eventType))
            return;

        for (int i = 0; i < subDictionary[eventType].Count; i++)
        {
            if (subDictionary[eventType][i].subscriber.Equals(subscriber))
            {
                subDictionary[eventType].Remove(subDictionary[eventType][i]);
                i--;
                continue;
            }
        }

    }


    

    private void subscribe(EventMethod subscriberMethod)
    {
        if (subDictionary.ContainsKey(subscriberMethod.eventType))
        {
            subDictionary[subscriberMethod.eventType].Add(subscriberMethod);
            return;
        }

        List<EventMethod> methods = new List<EventMethod>();
        subDictionary.Add(subscriberMethod.eventType, methods);
        subDictionary[subscriberMethod.eventType].Add(subscriberMethod);
    }



    public void post(object eventIns){
        post(eventIns, Priority.Middle);
    }

    private void post(object eventIns, Priority priority){

        lock (lockMutex)
        {
            if (!subDictionary.ContainsKey(eventIns.GetType()))
                return;


            for (int i = 0; i < subDictionary[eventIns.GetType()].Count; i++)
            {
                if (subDictionary[eventIns.GetType()][i].subscriber == null)
                {
                    subDictionary[eventIns.GetType()].Remove(subDictionary[eventIns.GetType()][i]);
                    i--;
                    continue;
                }

                EventMethod method = subDictionary[eventIns.GetType()][i];
                method.subscriber.GetType().InvokeMember(method.eventMethodName, BindingFlags.Public | BindingFlags.InvokeMethod, null, method.subscriber, new object[] { eventIns });

            }
        }
      
    }


    private class EventMethod
    {
        public Type eventType;
        public object subscriber;
        public string eventMethodName;
    }



    private List<EventMethod> findEventMethodByClass(object subscriber)
    {
        List<EventMethod> eventMethods = new List<EventMethod>();
        Type classType = subscriber.GetType();
        MethodInfo[] infos = classType.GetMethods();

        for (int i = 0; i < infos.Length; i++){
            
            if (infos[i].ToString().Contains(MethodNameTag))
            {
                EventMethod tmp = new EventMethod();

                
                tmp.eventType = infos[i].GetParameters()[0].ParameterType;
                tmp.subscriber = subscriber;
                tmp.eventMethodName = infos[i].Name;
                eventMethods.Add(tmp);
            }
        }

        return eventMethods;

    }

}
