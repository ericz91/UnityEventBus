using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EventBus : MonoBehaviour {

    private UnityEventBus eventBus;
    private Queue<object> mainQueue;
    private object lockQueue;

    private Queue<object> asyncQueue;
    private object asyncLock;
    private Thread asyncThread;

    private bool isApplicationQuit;

    // Use this for initialization
    void Awake () {
        isApplicationQuit = false;

        eventBus = UnityEventBus.getInstance();
        mainQueue = new Queue<object>();
        lockQueue = new object();

        asyncQueue = new Queue<object>();
        asyncLock = new object();

        asyncThread = new Thread(asyncTask);
	}

    private void Start()
    {
        ThreadPool.SetMaxThreads(5, 5);
        asyncThread.Start();
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
    public void postEvent(object eventIns){
        eventBus.post(eventIns);
        postOnMainThread(eventIns);
    }

    /// <summary>
    /// 异步传递事件
    /// </summary>
    /// <param name="eventIns"></param>
    public void postEventAsync(object eventIns)
    {
        lock (asyncLock)
        {
            asyncQueue.Enqueue(eventIns);
        }
    }


    private void asyncTask()
    {
        while (!isApplicationQuit)
        {
            if (asyncQueue.Count == 0)
            {
                Thread.Sleep(1);
                continue;
            }
               

            object tmp;
            lock (asyncLock)
            {
                tmp = asyncQueue.Dequeue();
            }
            ThreadPool.QueueUserWorkItem(postEvent, tmp);
        }
           
    }

    private void postOnMainThread(object eventIns)
    {
        lock (lockQueue)
        {
            mainQueue.Enqueue(eventIns);
        }
         
    }

    private void FixedUpdate()
    {

        object tmp;
        lock (lockQueue)
        {
            if (mainQueue.Count == 0)
                tmp = null;
            else
                tmp = mainQueue.Dequeue();


        }

        if (tmp != null)
            StartCoroutine(postMain(tmp));
       
    }


    IEnumerator postMain(object eventIns)
    {
        eventBus.postMain(eventIns);
        yield return 0;
    }

    private void OnDisable()
    {
        isApplicationQuit = true;
    }

    private void OnDestroy()
    {
        isApplicationQuit = true;
    }

    private void OnApplicationQuit()
    {
        isApplicationQuit = true;
    }

}
