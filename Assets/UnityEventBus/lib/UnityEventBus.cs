using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace cn.blockstudio.unityeventbus
{
    public class UnityEventBus : MonoBehaviour
    {

        private static EventBus eventBus = EventBus.getInstance();
        private static Queue<object> updateQueue;
        private static Queue<object> fixedQueue;
        private static object lockQueue;
        private static bool isQuit = false;

        private static UnityEventBus instance;

        
        public static UnityEventBus Instance { get {

                if (isQuit)
                    return null;

                if (instance == null)
                {
                    instance = new GameObject("UnityEventBus").AddComponent<UnityEventBus>();
                    DontDestroyOnLoad(instance.gameObject);
                }

                return instance;

            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void initBus()
        {
            var a = Instance;
        }



        // Use this for initialization
        void Awake()
        {
            isQuit = false;
            updateQueue = new Queue<object>();
            fixedQueue = new Queue<object>();
            lockQueue = new object();
        }

        private void Start()
        {
            ThreadPool.SetMaxThreads(5, 5);
        }



        //public static UnityEventBus GetInstance()
        //{
        //    if (isQuit)
        //        return null;

        //    if (instance == null)
        //    {
        //        instance = new GameObject("UnityEventBus").AddComponent<UnityEventBus>();
        //        DontDestroyOnLoad(instance.gameObject);
        //    }
                
        //    return instance;
        //}

        // 注册事件
        public static void Register(object subscriber)
        {
            eventBus.register(subscriber);
        }

        //取消注册
        public static void UnregisterAll(object subscriber, Type eventType)
        {
            eventBus.unregister(subscriber, eventType);
            eventBus.unregisterMain(subscriber, eventType);
        }

        public static void UnregisterMainThread(object subscriber, Type eventType)
        {
            eventBus.unregisterMain(subscriber, eventType);
        }

        public static void Unregister(object subscriber, Type eventType)
        {
            eventBus.unregister(subscriber, eventType);
        }


        /// <summary>
        /// 传递事件
        /// </summary>
        /// <param name="eventIns"></param>
        public void PostEvent(object eventIns)
        {


            eventBus.post(eventIns);
            postOnMainThread(eventIns);
        }

        /// <summary>
        /// 异步传递事件
        /// </summary>
        /// <param name="eventIns"></param>
        public void PostEventAsync(object eventIns)
        {

            ThreadPool.QueueUserWorkItem(PostEvent, eventIns);
        }


        private void postOnMainThread(object eventIns)
        {
            if (isQuit == true || updateQueue == null || updateQueue == null)
                return;

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



        private void OnDisable()
        {
            isQuit = true;
        }

        private void OnDestroy()
        {
            isQuit = true;
        }

        private void OnApplicationQuit()
        {
            isQuit = true;
        }


    }
}

