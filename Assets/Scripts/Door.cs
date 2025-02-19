using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool doorState; //true면 닫힌상태 false면 열린상태
    public BoxCollider boxcollider;

    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Quaternion endRotation;
    private float endTime = 0.5f; //회전 시간
    private bool isRotation = false; // false면 회전 안하고있음.
    private Coroutine currentCoroutine;

    private void Start()
    {
        boxcollider = GetComponent<BoxCollider>();
    }
    public void Select_Door()
    {
        if (!isRotation)
        {
            StartCoroutine(RotationDoor());
        }
    }

    private IEnumerator RotationDoor()
    {
        isRotation = true;
        float startTime = 0f;
        startRotation = transform.rotation;

        if (doorState)
        {
            endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y+ 110 ,0);
            istrigger_on();
            Debug.Log("문이 열림");
        }
        else if (!doorState)
        {
            endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y - 110, 0);
            istrigger_on();
            Debug.Log("문이 닫힘");
        }
        while (startTime < endTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, startTime / endTime);
            startTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRotation;
        doorState = !doorState;
        currentCoroutine = null;
        isRotation = false;
        istrigger_off();
    }

    public void istrigger_on() => boxcollider.isTrigger = true;
    public void istrigger_off() => boxcollider.isTrigger = false;

}
