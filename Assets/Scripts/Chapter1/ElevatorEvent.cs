using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorEvent : MonoBehaviour
{
    public ElevatorTrigger elevatorTrigger;

    public void Operate_Elevator()
    {
        elevatorTrigger.Operate_Elevator();
    }
    public void Close_Elevator()
    {
        elevatorTrigger.Close_Elevator();
    }
    public void Open_Elevator()
    {
        elevatorTrigger.Open_Elevator();
    }
}
