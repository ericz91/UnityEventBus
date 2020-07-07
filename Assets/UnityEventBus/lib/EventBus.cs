
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
        private Dictionary<Type, List<EventMethodMain>[]> subDictionaryMainUd;
        private Dictionary<Type, List<EventMethodMain>[]> subDictionaryMainFUd;

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
                removeEventFromContanier<EventMethod>(subDictionary, eventType, subscriber);
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
                removeEventFromContanier<EventMethodMain>(subDictionaryMainUd, eventType, subscriber);
                removeEventFromContanier<EventMethodMain>(subDictionaryMainFUd, eventType, subscriber);

            }

        }



        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="subscriberMethod"></param>
        private void subscribe(EventMethod subscriberMethod)
        {
            addSubcriberToContainer<EventMethod>(subDictionary, subscriberMethod);
        }

        /// <summary>
        /// 订阅主线程中的事件
        /// </summary>
        /// <param name="subscriberMethod"></param>
        private void subscribeMain(EventMethodMain subscriberMethod)
        {

            if (subscriberMethod.executeType == ExecuteType.UPDATE)
            {
                addSubcriberToContainer<EventMethodMain>(subDictionaryMainUd, subscriberMethod);
            }
            else if (subscriberMethod.executeType == ExecuteType.FIXEDUPDATE)
            {
                addSubcriberToContainer<EventMethodMain>(subDictionaryMainFUd, subscriberMethod);
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
                runEventCallback<EventMethod>(subDictionary, eventIns);
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
                runEventCallback<EventMethodMain>(subDictionaryMainUd, eventIns);
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
                runEventCallback<EventMethodMain>(subDictionaryMainFUd, eventIns);
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
            List<EventMethod> eventMethods = findEventMethodByAttr<EventMethod, Subscriber>(subscriber, false);
            return eventMethods;
        }


        private List<EventMethodMain> findEventMethodByAttrMain(object subscriber)
        {

            List<EventMethodMain> eventMethods = findEventMethodByAttr<EventMethodMain, SubscriberMain>(subscriber, true);
            return eventMethods;
        }


        private void removeEventFromContanier<T>(Dictionary<Type, List<T>[]> container, Type eventType, object subscriber)
        {
            if (!container.ContainsKey(eventType))
                return;


            for (int m = 0; m < container[eventType].Length; m++)
            {
                if (container[eventType][m] == null)
                    continue;

                for (int k = 0; k < container[eventType][m].Count; k++)
                {
                    if ((container[eventType][m][k] as EventMethod).subscriber.Equals(subscriber))
                    {
                        container[eventType][m].Remove(container[eventType][m][k]);
                        k--;
                        continue;
                    }
                }
            }
        }


        private void runEventCallback<T>(Dictionary<Type, List<T>[]> container, object eventdata) where T : class
        {
            object eventIns = eventdata;

            if (!container.ContainsKey(eventIns.GetType()))
                return;

            Type eventType = eventIns.GetType();

            for (int i = 0; i < container[eventType].Length; i++)
            {
                if (container[eventType][i] == null)
                    continue;

                for (int k = 0; k < container[eventType][i].Count; k++)
                {
                    if (container[eventType][i][k] == null)
                        continue;

                    if ((container[eventType][i][k] as EventMethod).subscriber == null)
                    {
                        container[eventType][i].Remove(container[eventType][i][k]);
                        k--;
                        continue;
                    }

                    EventMethod method = container[eventType][i][k] as EventMethod;
                    method.subscriber.GetType().InvokeMember(method.eventMethodName, BindingFlags.InvokeMethod, null, method.subscriber, new object[] { eventIns });

                }
            }
        }


        private List<T1> findEventMethodByAttr<T1, T2>(object subscriber, bool isMainMethod) where T1:class, new() where T2:class
        {
            List<T1> eventMethods = new List<T1>();
            Type classType = subscriber.GetType();
            MethodInfo[] infos = classType.GetMethods();

            for (int i = 0; i < infos.Length; i++)
            {
                object[] objAttrs = infos[i].GetCustomAttributes(typeof(T2), true);
                if (objAttrs.Length > 0)
                {

                    T2 stm = objAttrs[0] as T2;
                    T1 tmp = new T1();

                    if (isMainMethod)
                    {
                        SubscriberMain sm = stm as SubscriberMain;
                        EventMethodMain emm = tmp as EventMethodMain;
                        emm.eventType = infos[i].GetParameters()[0].ParameterType;
                        emm.subscriber = subscriber;
                        emm.eventMethodName = infos[i].Name;
                        emm.executePriority = sm.CallbackPriority;
                        emm.executeType = sm.CallbackExecuteType;
                        eventMethods.Add(tmp);
                    }
                    else
                    {
                        Subscriber sm = stm as Subscriber;
                        EventMethod emm = tmp as EventMethod;
                        emm.eventType = infos[i].GetParameters()[0].ParameterType;
                        emm.subscriber = subscriber;
                        emm.eventMethodName = infos[i].Name;
                        emm.executePriority = sm.CallbackPriority;
                        eventMethods.Add(tmp);

                    }
                }
            }

            return eventMethods;
        }


        private void addSubcriberToContainer<T>(Dictionary<Type, List<T>[]> container, T sMethod) where T : class
        {
            EventMethod subscriberMethod = sMethod as EventMethod;
            if (!container.ContainsKey(subscriberMethod.eventType))
            {
                List<T>[] methods = new List<T>[100];
                container.Add(subscriberMethod.eventType, methods);
            }

            if (container[subscriberMethod.eventType][subscriberMethod.executePriority] == null)
            {
                container[subscriberMethod.eventType][subscriberMethod.executePriority] = new List<T>();
            }


            for (int i = 0; i < container[subscriberMethod.eventType][subscriberMethod.executePriority].Count; i++)
            {
                EventMethod eventMethod = container[subscriberMethod.eventType][subscriberMethod.executePriority][i] as EventMethod;

                if (eventMethod.eventMethodName.Equals(subscriberMethod.eventMethodName)
                    & eventMethod.eventType.Equals(subscriberMethod.eventType)
                    & eventMethod.subscriber.Equals(subscriberMethod.subscriber)
                    )
                {
                    return;
                }
            }


            container[subscriberMethod.eventType][subscriberMethod.executePriority].Add(sMethod);


        }


    }


}
