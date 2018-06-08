using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sender : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(sendMessage());
	}
	
    public class MailA
    {
        public int a;
    }

    public class MailB
    {
       public int b;
    }

    IEnumerator sendMessage()
    {
        while (true)
        {
            MailA messageA = new MailA();
            messageA.a = 5;
            MailB messageB = new MailB();
            messageB.b = 10;
            UnityEventBus.getInstance().post(messageA);
            yield return new WaitForSeconds(2);
            UnityEventBus.getInstance().post(messageB);
            yield return new WaitForSeconds(2);

        }

    }


}
