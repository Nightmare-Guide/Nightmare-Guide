using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
   
    [SerializeField] GameObject[] zone;

    private void Start()
    {
        ClearZone();
    }

    public void OpenZone(int num)
    {
      
        ClearZone();

        switch (num) {
            case 1:
                zone[0].SetActive(true);
                zone[1].SetActive(true);
                zone[2].SetActive(true);
                break;
            case 2:
                zone[0].SetActive(true);
                zone[1].SetActive(true);
                zone[2].SetActive(true);
                zone[3].SetActive(true);
                break;
            case 3:
                zone[0].SetActive(true);
                zone[1].SetActive(true);
                zone[2].SetActive(true);
                zone[3].SetActive(true);
                zone[4].SetActive(true);
                break;
            case 4:
                zone[1].SetActive(true);
                zone[2].SetActive(true);
                zone[3].SetActive(true);
                break;
        }

    }
    public void ClearZone()
    {
        foreach (GameObject obj in zone)
        {
            obj.SetActive(false);
        }
    }

}
