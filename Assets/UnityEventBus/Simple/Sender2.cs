using cn.blockstudio.unityeventbus;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace cn.blockstudio.unityeventbus
{
    public class Sender2 : MonoBehaviour
    {

        Thread thread;
        public EventBus bus;
        bool isRunning;

        // Use this for initialization
        void Start()
        {
            isRunning = true;
            thread = new Thread(sendMainThread);
            thread.Start();
            //StartCoroutine(se());
        }

        void sendMainThread()
        {
            while (isRunning)
            {
                Event e = new Event();
                Event e2 = new Event();
                e.a = 10;
                e2.a = 20;
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                bus.postEventAsync(e2);
                Thread.Sleep(10);

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
                Event e = new Event();
                e.a = 30;
                bus.postEventAsync(e);
                yield return new WaitForSeconds(1);
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

}
