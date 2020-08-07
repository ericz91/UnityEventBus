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
            UnityEventBus.Instance.PostEvent(e);
            UnityEventBus.Instance.PostEventAsync(e);
            UnityEventBus.Instance.PostEventAsync(e2);
            UnityEventBus.Instance.PostEvent(e2);
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
            UnityEventBus.Instance.PostEventAsync(e);
            yield return new WaitForSeconds(2.5f);
        }
    }

    private void OnDestroy()
    {
        UnityEventBus.UnregisterAll(this, typeof(EventAA));
        isRunning = false;
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
    }

}


