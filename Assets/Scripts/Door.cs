using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int lockerIndex;
    public bool doorState; //true�� �������� false�� ��������
    public BoxCollider boxcollider;

    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Quaternion endRotation;
    private float endTime = 0.5f; //ȸ�� �ð�
    public bool isRotation = false; // false�� ȸ�� ���ϰ�����.
    public Coroutine currentCoroutine;

    private void Start()
    {
        startRotation = transform.rotation;
        boxcollider = GetComponent<BoxCollider>();
    }

    public void Select_Door()
    {
        if (!isRotation)
        {
            isRotation = true;
            StartCoroutine(RotationDoor());
        }


    }

    private IEnumerator RotationDoor()
    {
     
        float startTime = 0f;
        startRotation = transform.rotation;

        if (doorState)
        {
            endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y+ 110 ,0);
            istrigger_on();
        }
        else if (!doorState)
        {
            endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y - 110, 0);
            istrigger_on();

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
