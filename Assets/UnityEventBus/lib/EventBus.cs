
using System;
using System.Collections.Generic;
using System.Reflection;

namespace cn.blockstudio.unityeventbus
{
    public class EventBus
    {

        [NonSerialized]
        public string tag = "UnityEventBus";
        private static EventBus defaultInstance;

        private object lockMutex;
        private object lockMutexMain;

        private Dictionary<Type, List<EventMethod>[]> subDictionary;

        //private Dictionary<Type, List<EventMethod>> subDictionaryHighLevel;
        //private Dictionary<Type, List<EventMethod>> subDictionaryMiddleLevel;
        //private Dictionary<Type, List<EventMethod>> subDictionaryLowLevel;

        private Dictionary<Type, List<EventMethodMain>[]> subDictionaryMainUd;
        //private Dictionary<Type, List<EventMethodMain>> subDictionaryMainHighLevelUd;
        //private Dictionary<Type, List<EventMethodMain>> subDictionaryMainMiddelLevelUd;
        //private Dictionary<Type, List<EventMethodMain>> subDictionaryMainLowLevelUd;

        private Dictionary<Type, List<EventMethodMain>[]> subDictionaryMainFUd;
        //private Dictionary<Type, List<EventMethodMain>> subDictionaryMainHighLevelFUd;
        //private Dictionary<Type, List<EventMethodMain>> subDictionaryMainMiddelLevelFUd;
        // private Dictionary<Type, List<EventMethodMain>> subDictionaryMainLowLevelFUd;

        public static EventBus getInstance()
        {
            if (defaultInstance == null)
            {
                defaultInstance = new EventBus();
            }
            return defaultInstance;
        }


        private EventBus()
        {
            initSubDir();
            initSubMainDirUd();
            initSubMainDirFUd();

            lockMutex = new object();
            lockMutexMain = new object();
        }

        private void initSubDir()
        {
            subDictionary = new Dictionary<Type, List<EventMethod>[]>();

        }

        private void initSubMainDirUd()
        {
            subDictionaryMainUd = new Dictionary<Type, List<EventMethodMain>[]>();
        }

        private void initSubMainDirFUd()
        {
            subDictionaryMainFUd = new Dictionary<Type, List<EventMethodMain>[]>();

        }



        /// <summary>
        /// 注册本类需要订阅的事件
        /// </summary>
        /// <param name="subscriber"></param>
        public void register(object subscriber)
        {
            lock (lockMutex)
            {

                List<EventMethod> eventMethodsAttr = findEventMethodByAttr(subscriber);
                foreach (EventMethod tmp in eventMethodsAttr)
                    subscribe(tmp);


            }

            lock (lockMutexMain)
            {

                List<EventMethodMain> eventMethodsAttr = findEventMethodByAttrMain(subscriber);
                foreach (EventMethodMain tmp in eventMethodsAttr)
                    subscribeMain(tmp);

            }


        }

        /// <summary>
        /// 卸载普通订阅事件
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="eventType"></param>
        public void unregister(object subscriber, Type eventType)
        {
            lock (lockMutex)
            {


                if (!subDictionary.ContainsKey(eventType))
                    return;


                for (int m = 0; m < subDictionary[eventType].Length; m++)
                {
                    if (subDictionary[eventType][m] == null)
                        continue;

                    for (int k = 0; k < subDictionary[eventType][m].Count; k++)
                    {
                        if (subDictionary[eventType][m][k].subscriber.Equals(subscriber))
                        {
                            subDictionary[eventType][m].Remove(subDictionary[eventType][m][k]);
                            k--;
                            continue;
                        }
                    }
                }



            }

        }

        /// <summary>
        /// 卸载主线程订阅事件
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="eventType"></param>
        public void unregisterMain(object subscriber, Type eventType)
        {
            lock (lockMutexMain)
            {

                if (subDictionaryMainUd.ContainsKey(eventType))
                {
                    for (int m = 0; m < subDictionaryMainUd[eventType].Length; m++)
                    {
                        if (subDictionaryMainUd[eventType][m] == null)
                            continue;

                        for (int k = 0; k < subDictionaryMainUd[eventType][m].Count; k++)
                        {
                            if (subDictionaryMainUd[eventType][m][k].subscriber.Equals(subscriber))
                            {
                                subDictionaryMainUd[eventType][m].Remove(subDictionaryMainUd[eventType][m][k]);
                                k--;
                                continue;
                            }
                        }
                    }
                }


                if (subDictionaryMainFUd.ContainsKey(eventType))
                {
                    for (int m = 0; m < subDictionaryMainFUd[eventType].Length; m++)
                    {
                        if (subDictionaryMainFUd[eventType][m] == null)
                            continue;

                        for (int k = 0; k < subDictionaryMainFUd[eventType][m].Count; k++)
                        {
                            if (subDictionaryMainFUd[eventType][m][k].subscriber.Equals(subscriber))
                            {
                                subDictionaryMainFUd[eventType][m].Remove(subDictionaryMainFUd[eventType][m][k]);
                                k--;
                                continue;
                            }
                        }
                    }
                }


            }

        }



        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="subscriberMethod"></param>
        private void subscribe(EventMethod subscriberMethod)
        {

            if (!subDictionary.ContainsKey(subscriberMethod.eventType))
            {
                List<EventMethod>[] methods = new List<EventMethod>[100];
                subDictionary.Add(subscriberMethod.eventType, methods);
                
            }

             if(subDictionary[subscriberMethod.eventType][subscriberMethod.executePriority] == null)
            {
                subDictionary[subscriberMethod.eventType][subscriberMethod.executePriority] = new List<EventMethod>();
            }

            subDictionary[subscriberMethod.eventType][subscriberMethod.executePriority].Add(subscriberMethod);


        }

        /// <summary>
        /// 订阅主线程中的事件
        /// </summary>
        /// <param name="subscriberMethod"></param>
        private void subscribeMain(EventMethodMain subscriberMethod)
        {

            if (subscriberMethod.executeType == ExecuteType.UPDATE)
            {
                if (!subDictionaryMainUd.ContainsKey(subscriberMethod.eventType))
                {
                    List<EventMethodMain>[] methods = new List<EventMethodMain>[100];
                    subDictionaryMainUd.Add(subscriberMethod.eventType, methods);

                }

                if (subDictionaryMainUd[subscriberMethod.eventType][subscriberMethod.executePriority] == null)
                {
                    subDictionaryMainUd[subscriberMethod.eventType][subscriberMethod.executePriority] = new List<EventMethodMain>();
                }

                subDictionaryMainUd[subscriberMethod.eventType][subscriberMethod.executePriority].Add(subscriberMethod);

            }
            else if (subscriberMethod.executeType == ExecuteType.FIXEDUPDATE)
            {
                if (!subDictionaryMainFUd.ContainsKey(subscriberMethod.eventType))
                {
                    List<EventMethodMain>[] methods = new List<EventMethodMain>[100];
                    subDictionaryMainFUd.Add(subscriberMethod.eventType, methods);

                }

                if (subDictionaryMainFUd[subscriberMethod.eventType][subscriberMethod.executePriority] == null)
                {
                    subDictionaryMainFUd[subscriberMethod.eventType][subscriberMethod.executePriority] = new List<EventMethodMain>();
                }

                subDictionaryMainFUd[subscriberMethod.eventType][subscriberMethod.executePriority].Add(subscriberMethod);
            }
        }


        /// <summary>
        /// post普通事件
        /// </summary>
        /// <param name="eventIns"></param>
        public void post(object eventIns)
        {
            lock (lockMutex)
            {

                if (!subDictionary.ContainsKey(eventIns.GetType()))
                    return;

                Type eventType = eventIns.GetType();

                for (int i = 0; i < subDictionary[eventType].Length; i++)
                {
                    if (subDictionary[eventType][i] == null)
                        continue;

                    for(int k = 0; k < subDictionary[eventType][i].Count; k++)
                    {
                        if (subDictionary[eventType][i][k] == null)
                            continue;

                        if (subDictionary[eventType][i][k].subscriber == null)
                        {
                            subDictionary[eventType][i].Remove(subDictionary[eventType][i][k]);
                            k--;
                            continue;
                        }

                        EventMethod method = subDictionary[eventType][i][k];
                        method.subscriber.GetType().InvokeMember(method.eventMethodName, BindingFlags.InvokeMethod, null, method.subscriber, new object[] { eventIns });

                    }
                }

            }

        }

        /// <summary>
        /// post update内部运行的事件
        /// </summary>
        /// <param name="eventIns"></param>
        public void postMainUd(object eventIns)
        {
            lock (lockMutexMain)
            {
                if (!subDictionaryMainUd.ContainsKey(eventIns.GetType()))
                    return;

                Type eventType = eventIns.GetType();

                for (int i = 0; i < subDictionaryMainUd[eventType].Length; i++)
                {
                    if (subDictionaryMainUd[eventType][i] == null)
                        continue;

                    for (int k = 0; k < subDictionaryMainUd[eventType][i].Count; k++)
                    {
                        if (subDictionaryMainUd[eventType][i][k] == null)
                            continue;

                        if (subDictionaryMainUd[eventType][i][k].subscriber == null)
                        {
                            subDictionaryMainUd[eventType][i].Remove(subDictionaryMainUd[eventType][i][k]);
                            k--;
                            continue;
                        }

                        EventMethod method = subDictionaryMainUd[eventType][i][k];
                        method.subscriber.GetType().InvokeMember(method.eventMethodName, BindingFlags.InvokeMethod, null, method.subscriber, new object[] { eventIns });

                    }
                }
            }
        }


        /// <summary>
        /// post fixedupdate内部运行的事件
        /// </summary>
        /// <param name="eventIns"></param>
        public void postMainFUd(object eventIns)
        {
            lock (lockMutexMain)
            {
                if (!subDictionaryMainFUd.ContainsKey(eventIns.GetType()))
                    return;

                Type eventType = eventIns.GetType();

                for (int i = 0; i < subDictionaryMainFUd[eventType].Length; i++)
                {
                    if (subDictionaryMainFUd[eventType][i] == null)
                        continue;

                    for (int k = 0; k < subDictionaryMainFUd[eventType][i].Count; k++)
                    {
                        if (subDictionaryMainFUd[eventType][i][k] == null)
                            continue;

                        if (subDictionaryMainFUd[eventType][i][k].subscriber == null)
                        {
                            subDictionaryMainFUd[eventType][i].Remove(subDictionaryMainFUd[eventType][i][k]);
                            k--;
                            continue;
                        }

                        EventMethod method = subDictionaryMainFUd[eventType][i][k];
                        method.subscriber.GetType().InvokeMember(method.eventMethodName, BindingFlags.InvokeMethod, null, method.subscriber, new object[] { eventIns });

                    }
                }
            }
        }


        private class EventMethod
        {
            public Type eventType;
            public object subscriber;
            public string eventMethodName;
            public int executePriority;
        }

        private class EventMethodMain : EventMethod
        {
            public ExecuteType executeType;
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
                    Subscriber st = objAttrs[0] as Subscriber;
                    EventMethod tmp = new EventMethod();
                    tmp.eventType = infos[i].GetParameters()[0].ParameterType;
                    tmp.subscriber = subscriber;
                    tmp.eventMethodName = infos[i].Name;
                    tmp.executePriority = st.CallbackPriority;
                    eventMethods.Add(tmp);
                }
                else
                    continue;
            }

            return eventMethods;

        }


        private List<EventMethodMain> findEventMethodByAttrMain(object subscriber)
        {
            List<EventMethodMain> eventMethods = new List<EventMethodMain>();
            Type classType = subscriber.GetType();
            MethodInfo[] infos = classType.GetMethods();

            for (int i = 0; i < infos.Length; i++)
            {
                object[] objAttrs = infos[i].GetCustomAttributes(typeof(SubscriberMain), true);
                if (objAttrs.Length > 0)
                {
                    SubscriberMain stm = objAttrs[0] as SubscriberMain;
                    EventMethodMain tmp = new EventMethodMain();
                    tmp.eventType = infos[i].GetParameters()[0].ParameterType;
                    tmp.subscriber = subscriber;
                    tmp.eventMethodName = infos[i].Name;
                    tmp.executePriority = stm.CallbackPriority;
                    tmp.executeType = stm.CallbackExecuteType;
                    eventMethods.Add(tmp);
                }
            }

            return eventMethods;

        }

    }


}
