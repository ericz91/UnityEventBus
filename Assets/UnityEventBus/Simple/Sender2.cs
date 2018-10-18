using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Sender2 : MonoBehaviour {

    Thread thread;
    public EventBus bus;
    bool isRunning;

    // Use this for initialization
    void Start () {
        isRunning = true;
        thread = new Thread(sendMainThread);
        thread.Start();
        StartCoroutine(se());
	}

    void sendMainThread()
    {
        while (isRunning)
        {
            Event e = new Event();
            e.a = 10;
            bus.postOnMainThread(e);
            Thread.Sleep(1000);
        }
    }
	
	// Update is called once per frame
	void Update () {
        
    }


    IEnumerator se()
    {
        while (true)
        {
            Event e = new Event();
            e.a = 20;
            bus.postEvent(e);
            yield return new WaitForSeconds(1);
        }
    }

    private void OnDestroy()
    {
        isRunning = false;
    }


}
