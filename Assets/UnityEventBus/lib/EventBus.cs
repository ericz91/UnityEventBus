
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

        private Dictionary<Type, List<EventMethod>>[] subDictionary;
        private Dictionary<Type, List<EventMethod>> subDictionaryHighLevel;
        private Dictionary<Type, List<EventMethod>> subDictionaryMiddleLevel;
        private Dictionary<Type, List<EventMethod>> subDictionaryLowLevel;

        private Dictionary<Type, List<EventMethodMain>>[] subDictionaryMainUd;
        private Dictionary<Type, List<EventMethodMain>> subDictionaryMainHighLevelUd;
        private Dictionary<Type, List<EventMethodMain>> subDictionaryMainMiddelLevelUd;
        private Dictionary<Type, List<EventMethodMain>> subDictionaryMainLowLevelUd;

        private Dictionary<Type, List<EventMethodMain>>[] subDictionaryMainFUd;
        private Dictionary<Type, List<EventMethodMain>> subDictionaryMainHighLevelFUd;
        private Dictionary<Type, List<EventMethodMain>> subDictionaryMainMiddelLevelFUd;
        private Dictionary<Type, List<EventMethodMain>> subDictionaryMainLowLevelFUd;

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
            subDictionaryHighLevel = new Dictionary<Type, List<EventMethod>>();
            subDictionaryMiddleLevel = new Dictionary<Type, List<EventMethod>>();
            subDictionaryLowLevel = new Dictionary<Type, List<EventMethod>>();
            subDictionary = new Dictionary<Type, List<EventMethod>>[3];
            subDictionary[0] = subDictionaryHighLevel;
            subDictionary[1] = subDictionaryMiddleLevel;
            subDictionary[2] = subDictionaryLowLevel;
        }

        private void initSubMainDirUd()
        {
            subDictionaryMainUd = new Dictionary<Type, List<EventMethodMain>>[3];
            subDictionaryMainHighLevelUd = new Dictionary<Type, List<EventMethodMain>>();
            subDictionaryMainMiddelLevelUd = new Dictionary<Type, List<EventMethodMain>>();
            subDictionaryMainLowLevelUd = new Dictionary<Type, List<EventMethodMain>>();
            subDictionaryMainUd[0] = subDictionaryMainHighLevelUd;
            subDictionaryMainUd[1] = subDictionaryMainMiddelLevelUd;
            subDictionaryMainUd[2] = subDictionaryMainLowLevelUd;
        }

        private void initSubMainDirFUd()
        {
            subDictionaryMainFUd = new Dictionary<Type, List<EventMethodMain>>[3];
            subDictionaryMainHighLevelFUd = new Dictionary<Type, List<EventMethodMain>>();
            subDictionaryMainMiddelLevelFUd = new Dictionary<Type, List<EventMethodMain>>();
            subDictionaryMainLowLevelFUd = new Dictionary<Type, List<EventMethodMain>>();
            subDictionaryMainFUd[0] = subDictionaryMainHighLevelFUd;
            subDictionaryMainFUd[1] = subDictionaryMainMiddelLevelFUd;
            subDictionaryMainFUd[2] = subDictionaryMainLowLevelFUd;
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

                for (int i = 0; i < 3; i++)
                {
                    if (!subDictionary[i].ContainsKey(eventType))
                        continue;

                    for (int m = 0; m < subDictionary[i][eventType].Count; m++)
                    {
                        if (subDictionary[i][eventType][m].subscriber.Equals(subscriber))
                        {
                            subDictionary[i][eventType].Remove(subDictionary[i][eventType][m]);
                            m--;
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

                for (int i = 0; i < 3; i++)
                {

                    if (!subDictionaryMainUd[i].ContainsKey(eventType))
                        continue;

                    for (int m = 0; m < subDictionaryMainUd[i][eventType].Count; m++)
                    {
                        if (subDictionaryMainUd[i][eventType][m].subscriber.Equals(subscriber))
                        {
                            subDictionaryMainUd[i][eventType].Remove(subDictionaryMainUd[i][eventType][m]);
                            m--;
                            continue;
                        }
                    }
                }


                for (int i = 0; i < 3; i++)
                {

                    if (!subDictionaryMainFUd[i].ContainsKey(eventType))
                        continue;

                    for (int m = 0; m < subDictionaryMainFUd[i][eventType].Count; m++)
                    {
                        if (subDictionaryMainFUd[i][eventType][m].subscriber.Equals(subscriber))
                        {
                            subDictionaryMainFUd[i][eventType].Remove(subDictionaryMainFUd[i][eventType][m]);
                            m--;
                            continue;
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

            int p = 0;

            switch (subscriberMethod.executePriority)
            {
                case ExecutePriority.HIGH:p = 0;break;
                case ExecutePriority.MIDDLE:p = 1;break;
                case ExecutePriority.LOW:p = 2;break;
                default:break;
            }

            if (subDictionary[p].ContainsKey(subscriberMethod.eventType))
            {
                subDictionary[p][subscriberMethod.eventType].Add(subscriberMethod);
                return;
            }


            List<EventMethod> methods = new List<EventMethod>();
            subDictionary[p].Add(subscriberMethod.eventType, methods);
            subDictionary[p][subscriberMethod.eventType].Add(subscriberMethod);
        }

        /// <summary>
        /// 订阅主线程中的事件
        /// </summary>
        /// <param name="subscriberMethod"></param>
        private void subscribeMain(EventMethodMain subscriberMethod)
        {


            if(subscriberMethod.executeType== ExecuteType.UPDATE)
            {
                int p = 0;
                switch (subscriberMethod.executePriority)
                {
                    case ExecutePriority.HIGH: p = 0; break;
                    case ExecutePriority.MIDDLE: p = 1; break;
                    case ExecutePriority.LOW: p = 2; break;
                    default: break;
                }
                if (subDictionaryMainUd[p].ContainsKey(subscriberMethod.eventType))
                {
                    subDictionaryMainUd[p][subscriberMethod.eventType].Add(subscriberMethod);
                    return;
                }
                List<EventMethodMain> methods = new List<EventMethodMain>();
                subDictionaryMainUd[p].Add(subscriberMethod.eventType, methods);
                subDictionaryMainUd[p][subscriberMethod.eventType].Add(subscriberMethod);

            } else if(subscriberMethod.executeType == ExecuteType.FIXEDUPDATE)
            {
                int p = 0;
                switch (subscriberMethod.executePriority)
                {
                    case ExecutePriority.HIGH: p = 0; break;
                    case ExecutePriority.MIDDLE: p = 1; break;
                    case ExecutePriority.LOW: p = 2; break;
                    default: break;
                }
                if (subDictionaryMainFUd[p].ContainsKey(subscriberMethod.eventType))
                {
                    subDictionaryMainFUd[p][subscriberMethod.eventType].Add(subscriberMethod);
                    return;
                }
                List<EventMethodMain> methods = new List<EventMethodMain>();
                subDictionaryMainFUd[p].Add(subscriberMethod.eventType, methods);
                subDictionaryMainFUd[p][subscriberMethod.eventType].Add(subscriberMethod);
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
                for(int m =0; m < 3; m++)
                {
                    if (!subDictionary[m].ContainsKey(eventIns.GetType()))
                        continue;


                    for (int i = 0; i < subDictionary[m][eventIns.GetType()].Count; i++)
                    {
                        if (subDictionary[m][eventIns.GetType()][i].subscriber == null)
                        {
                            subDictionary[m][eventIns.GetType()].Remove(subDictionary[m][eventIns.GetType()][i]);
                            i--;
                            continue;
                        }
                        EventMethod method = subDictionary[m][eventIns.GetType()][i];
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
                {
                    for (int m = 0; m < 3; m++)
                    {
                        if (!subDictionaryMainUd[m].ContainsKey(eventIns.GetType()))
                            continue;


                        for (int i = 0; i < subDictionaryMainUd[m][eventIns.GetType()].Count; i++)
                        {
                            if (subDictionaryMainUd[m][eventIns.GetType()][i].subscriber == null)
                            {
                                subDictionaryMainUd[m][eventIns.GetType()].Remove(subDictionaryMainUd[m][eventIns.GetType()][i]);
                                i--;
                                continue;
                            }
                            EventMethod method = subDictionaryMainUd[m][eventIns.GetType()][i];
                            method.subscriber.GetType().InvokeMember(method.eventMethodName, BindingFlags.InvokeMethod, null, method.subscriber, new object[] { eventIns });
                        }

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
                {
                    for (int m = 0; m < 3; m++)
                    {
                        if (!subDictionaryMainFUd[m].ContainsKey(eventIns.GetType()))
                            continue;


                        for (int i = 0; i < subDictionaryMainFUd[m][eventIns.GetType()].Count; i++)
                        {
                            if (subDictionaryMainFUd[m][eventIns.GetType()][i].subscriber == null)
                            {
                                subDictionaryMainFUd[m][eventIns.GetType()].Remove(subDictionaryMainFUd[m][eventIns.GetType()][i]);
                                i--;
                                continue;
                            }
                            EventMethod method = subDictionaryMainFUd[m][eventIns.GetType()][i];
                            method.subscriber.GetType().InvokeMember(method.eventMethodName, BindingFlags.InvokeMethod, null, method.subscriber, new object[] { eventIns });
                        }

                    }
                }
            }
        }


        private class EventMethod
        {
            public Type eventType;
            public object subscriber;
            public string eventMethodName;
            public ExecutePriority executePriority;
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
