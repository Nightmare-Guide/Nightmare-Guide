using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public Door door;
    public bool inplayer = false;
    public BoxCollider col;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&& !door.doorState)
        {
            inplayer = true;
            door.Select_Door();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DisableCollider();
        }
    }


    public void DisableCollider()
    {
        col.enabled = false;
    }
    public void EnableCollider()
    {
        col.enabled = true;
    }
}
