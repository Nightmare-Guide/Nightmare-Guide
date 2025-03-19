using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Trigger : MonoBehaviour
{
    public GameObject triggerObject;
    public Animator triggerObjectAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Chapter1_Mgr.instance.MoveWall(triggerObjectAnimator);
        }
    }
}
