using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public GameObject elevator;
    public GameObject elevatorLeftDoor;
    public GameObject elevatorRightDoor;
    public GameObject elevatorCloseBtn;
    public GameObject elevatorOpenBtn;

    public Animator elevator_anim;
    public Animator elevator_animLeft;
    public Animator elevator_animRight;
    public Animator elevator_animCloseBtn;
    public Animator elevator_animOpenBtn;

    public bool firstfloor;
    public bool secondfloor;
    public bool ismoving;
    private void Start() //기본 상태
    {
        firstfloor = true;
        secondfloor = false;
        ismoving = false;
    }

    public void Operate_Elevator()
    {
        // elevator_anim.SetTrigger("UpPosition");
    }

    public void Close_Elevator()
    {
        elevator_animLeft.SetTrigger("CloseTrigger");
        elevator_animRight.SetTrigger("CloseTrigger");
    }

    public void Open_Elevator()
    {
        if (!ismoving)
        {
            elevator_animLeft.SetTrigger("OpenTrigger");
            elevator_animRight.SetTrigger("OpenTrigger");
        }
    }
}
