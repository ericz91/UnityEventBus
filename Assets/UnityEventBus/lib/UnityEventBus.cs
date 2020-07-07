using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace cn.blockstudio.unityeventbus
{
    public class UnityEventBus : MonoBehaviour
    {

        private EventBus eventBus;
        private Queue<object> updateQueue;
        private Queue<object> fixedQueue;
        private object lockQueue;


        private static UnityEventBus instance;

 

        // Use this for initialization
        void Awake()
        {

            eventBus = EventBus.getInstance();
            updateQueue = new Queue<object>();
            fixedQueue = new Queue<object>();
            lockQueue = new object();
        }

        private void Start()
        {
            ThreadPool.SetMaxThreads(5, 5);
        }

        public static UnityEventBus GetInstance()
        {
            if (instance == null)
            {
                instance = new GameObject("UnityEventBus").AddComponent<UnityEventBus>();
                DontDestroyOnLoad(instance.gameObject);
            }
                


            return instance;
        }

        // 注册事件
        public void register(object subscriber)
        {
            eventBus.register(subscriber);
        }

        //取消注册
        public void unregisterAll(object subscriber, Type eventType)
        {
            eventBus.unregister(subscriber, eventType);
            eventBus.unregisterMain(subscriber, eventType);
        }

        public void unregisterMainThread(object subscriber, Type eventType)
        {
            eventBus.unregisterMain(subscriber, eventType);
        }

        public void unregister(object subscriber, Type eventType)
        {
            eventBus.unregister(subscriber, eventType);
        }


        /// <summary>
        /// 传递事件
        /// </summary>
        /// <param name="eventIns"></param>
        public void postEvent(object eventIns)
        {
            eventBus.post(eventIns);
            postOnMainThread(eventIns);
        }

        /// <summary>
        /// 异步传递事件
        /// </summary>
        /// <param name="eventIns"></param>
        public void postEventAsync(object eventIns)
        {
            ThreadPool.QueueUserWorkItem(postEvent, eventIns);
        }


        private void postOnMainThread(object eventIns)
        {
            lock (lockQueue)
            {
                updateQueue.Enqueue(eventIns);
                fixedQueue.Enqueue(eventIns);
            }

        }

        private void FixedUpdate()
        {

            object tmp;
            lock (lockQueue)
            {
                if (fixedQueue.Count == 0)
                    tmp = null;
                else
                    tmp = fixedQueue.Dequeue();


            }

            if (tmp != null)
                eventBus.postMainFUd(tmp);

        }

        private void Update()
        {
            object tmp;
            lock (lockQueue)
            {
                if (updateQueue.Count == 0)
                    tmp = null;
                else
                    tmp = updateQueue.Dequeue();


            }

            if (tmp != null)
                eventBus.postMainUd(tmp);
        }






        private void OnApplicationQuit()
        {

        }


    }
}

