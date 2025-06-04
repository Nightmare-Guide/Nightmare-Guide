using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Trigger : MonoBehaviour
{
    public GameObject triggerObject;
    public Animator triggerObjectAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Trigger"))
        {
            Chapter1_Mgr.instance.ActiveTriggerAnimator(triggerObjectAnimator);
        }
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("StrangeRoom1"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom1);
        }
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("StrangeRoom2"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom2);
        }
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Teleport"))
        {
            Chapter1_Mgr.instance.Teleport_Enemy(Chapter1_Mgr.instance.Chase_Enemy);
        }
    }
}