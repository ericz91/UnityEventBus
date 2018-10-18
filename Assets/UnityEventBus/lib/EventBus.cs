using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour {

    private UnityEventBus eventBus;
    private Queue<object> mainQueue;
    private object lockQueue;

	// Use this for initialization
	void Awake () {
        eventBus = UnityEventBus.getInstance();
        mainQueue = new Queue<object>();
        lockQueue = new object();
	}
	
	// Update is called once per frame
	public void register(object subscriber)
    {
        eventBus.register(subscriber);
    }

    public void unregister(object subscriber, Type eventType)
    {
        eventBus.unregister(subscriber, eventType);
    }

    public void unregisterMainThread(object subscriber, Type eventType)
    {
        eventBus.unregisterMain(subscriber, eventType);
    }

    public void postEvent(object eventIns){
        eventBus.post(eventIns);
    }

    public void postOnMainThread(object eventIns)
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

}
