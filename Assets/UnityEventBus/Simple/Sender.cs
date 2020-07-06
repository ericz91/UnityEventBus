using cn.blockstudio.unityeventbus;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Sender : MonoBehaviour
{

    Thread thread;
    public UnityEventBus bus;
    bool isRunning;

    // Use this for initialization
    void Start()
    {
        bus = UnityEventBus.GetInstance();
        isRunning = true;
        thread = new Thread(sendMainThread);
        thread.Start();
        StartCoroutine(se());
    }

    void sendMainThread()
    {
        while (isRunning)
        {
            EventAA e = new EventAA();
            EventBB e2 = new EventBB();
            e.a = 10;
            e2.a = 20;
            bus.postEvent(e);
            bus.postEventAsync(e);
            bus.postEventAsync(e2);
            bus.postEvent(e2);
            Thread.Sleep(1000);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator se()
    {
        while (true)
        {
            EventAA e = new EventAA();
            e.a = 30;
            bus.postEventAsync(e);
            yield return new WaitForSeconds(2.5f);
        }
    }

    private void OnDestroy()
    {
        isRunning = false;
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
    }

}


