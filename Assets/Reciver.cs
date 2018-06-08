using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Reciver : MonoBehaviour {

    // Use this for initialization

    private void Awake()
    {
        UnityEventBus.getInstance().register(this);
    }

    void Start () {
        StartCoroutine(unregister());
	}
	
	
    public void eventBusMethod_ReciverA(Sender.MailA message)
    {
        Debug.Log("SendA:" + message.a);
    }

    public void eventBusMethod_ReciverB(Sender.MailB message)
    {
        Debug.Log("SendB:" + message.b);
    }

    IEnumerator unregister()
    {

        yield return new WaitForSeconds(5);
        UnityEventBus.getInstance().unregister(this, typeof(Sender.MailB));
        
    }


}
