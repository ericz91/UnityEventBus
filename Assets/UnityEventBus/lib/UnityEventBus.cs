using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class UnityEventBus
{

    [NonSerialized]
    public string tag = "UnityEventBus";
    private static UnityEventBus defaultInstance;
    [NonSerialized]
    public string MethodNameTag = "EB";
    public string MethodNameTagMain = "EBM";

    private object lockMutex;
    private object lockMutexMain;

    private Dictionary<Type, List<EventMethod>> subDictionary;
    private Dictionary<Type, List<EventMethod>> subDictionaryMain;

    public enum Priority
    {
        Low = 0,
        Middle,
        Hight
    }


    private UnityEventBus()
    {
        subDictionary = new Dictionary<Type, List<EventMethod>>();
        subDictionaryMain = new Dictionary<Type, List<EventMethod>>();
        lockMutex = new object();
        lockMutexMain = new object();
    }

    public static UnityEventBus getInstance()
    {
        if (defaultInstance == null)
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



    private void register(object subscriber, Priority priority, object lockMutex)
    {

        lock (lockMutex)
        {
            //遍历subscriber类里面事件的回调方法
            List<EventMethod> eventMethods = findEventMethodByClass(subscriber);

            //加入事件到订阅列表
            foreach (EventMethod tmp in eventMethods)
                subscribe(tmp);

            List<EventMethod> eventMethodsAttr = findEventMethodByAttr(subscriber);
            foreach (EventMethod tmp in eventMethodsAttr)
                subscribe(tmp);


        }

        lock (lockMutexMain)
        {
            //遍历subscriber类里面事件的主线程回调方法
            List<EventMethod> eventMethods = findEventMethodByClassMain(subscriber);

            //加入事件到订阅列表
            foreach (EventMethod tmp in eventMethods)
                subscribeMain(tmp);

            List<EventMethod> eventMethodsAttr = findEventMethodByAttrMain(subscriber);
            foreach (EventMethod tmp in eventMethodsAttr)
                subscribeMain(tmp);

        }

    }


    public void unregister(object subscriber, Type eventType)
    {

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

    public void unregisterMain(object subscriber, Type eventType)
    {

        if (!subDictionaryMain.ContainsKey(eventType))
            return;

        for (int i = 0; i < subDictionaryMain[eventType].Count; i++)
        {
            if (subDictionaryMain[eventType][i].subscriber.Equals(subscriber))
            {
                subDictionaryMain[eventType].Remove(subDictionaryMain[eventType][i]);
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

    private void subscribeMain(EventMethod subscriberMethod)
    {
        if (subDictionaryMain.ContainsKey(subscriberMethod.eventType))
        {
            subDictionaryMain[subscriberMethod.eventType].Add(subscriberMethod);
            return;
        }

        List<EventMethod> methods = new List<EventMethod>();
        subDictionaryMain.Add(subscriberMethod.eventType, methods);
        subDictionaryMain[subscriberMethod.eventType].Add(subscriberMethod);
    }



    public void post(object eventIns)
    {
        post(eventIns, Priority.Middle);
    }

    public void postMain(object eventIns)
    {
        postMain(eventIns, Priority.Middle);
    }


    private void post(object eventIns, Priority priority)
    {

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

    private void postMain(object eventIns, Priority priority)
    {

        lock (lockMutexMain)
        {
            if (!subDictionaryMain.ContainsKey(eventIns.GetType()))
                return;


            for (int i = 0; i < subDictionaryMain[eventIns.GetType()].Count; i++)
            {
                if (subDictionaryMain[eventIns.GetType()][i].subscriber == null)
                {
                    subDictionaryMain[eventIns.GetType()].Remove(subDictionaryMain[eventIns.GetType()][i]);
                    i--;
                    continue;
                }

                EventMethod method = subDictionaryMain[eventIns.GetType()][i];
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

        for (int i = 0; i < infos.Length; i++)
        {

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

    private List<EventMethod> findEventMethodByAttr(object subscriber)
    {
        List<EventMethod> eventMethods = new List<EventMethod>();
        Type classType = subscriber.GetType();
        MethodInfo[] infos = classType.GetMethods();

        for (int i = 0; i < infos.Length; i++)
        {
            object[] objAttrs = infos[i].GetCustomAttributes(typeof(Subscriber), true);
            if (objAttrs.Length > 0)
            {
                EventMethod tmp = new EventMethod();
                tmp.eventType = infos[i].GetParameters()[0].ParameterType;
                tmp.subscriber = subscriber;
                tmp.eventMethodName = infos[i].Name;
                eventMethods.Add(tmp);
            }
            else
                continue;
        }

        return eventMethods;

    }


    private List<EventMethod> findEventMethodByClassMain(object subscriber)
    {
        List<EventMethod> eventMethods = new List<EventMethod>();
        Type classType = subscriber.GetType();
        MethodInfo[] infos = classType.GetMethods();

        for (int i = 0; i < infos.Length; i++)
        {

            if (infos[i].ToString().Contains(MethodNameTagMain))
            {
                EventMethod tmp = new EventMethod();


                tmp.eventType = infos[i].GetParameters()[0].ParameterType;
                tmp.subscriber = subscriber;
                tmp.eventMethodName = infos[i].Name;
                eventMethods.Add(tmp);
            }
            else
                continue;
        }

        return eventMethods;

    }


    private List<EventMethod> findEventMethodByAttrMain(object subscriber)
    {
        List<EventMethod> eventMethods = new List<EventMethod>();
        Type classType = subscriber.GetType();
        MethodInfo[] infos = classType.GetMethods();

        for (int i = 0; i < infos.Length; i++)
        {
            object[] objAttrs = infos[i].GetCustomAttributes(typeof(SubscriberMain), true);
            if (objAttrs.Length>0)
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
