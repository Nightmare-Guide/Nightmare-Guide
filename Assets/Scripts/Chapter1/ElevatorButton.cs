using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public GameObject closeBtn;
    public GameObject openBtn;
    public GameObject firstFloorBtn;
    public GameObject secondFloorBtn;

    public Animator buttonAnimator;

    public void ClickBtn()
    {
        buttonAnimator.SetTrigger("ClickButton");
    }

}
