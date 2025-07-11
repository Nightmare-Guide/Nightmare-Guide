using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public Door door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !door.doorState)
        {
            door.Select_Door();
        }
    }
}
