using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traffic : MonoBehaviour
{
    [SerializeField] int myNum;
    [SerializeField] GameObject[] offZone;
    [SerializeField] GameObject[] zone;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in offZone)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in zone)
            {
                obj.SetActive(true);
            }
        }
    }
}
