using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public string doorID;
    public int lockerIndex;
    public bool doorState; // true = ����, false = ����
    public bool inplayerTimeLine = false; // �÷��̾� Ÿ�Ӷ��� Ȯ��
    public BoxCollider boxcollider;
    public NavMeshObstacle navMeshObstacle;

    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Quaternion endRotation;
    private float endTime = 0.5f;
    public bool isRotation = false;
    public Coroutine currentCoroutine;

    private void Start()
    {
        startRotation = transform.rotation;
        boxcollider = GetComponent<BoxCollider>();

        // NavMeshObstacle �ڵ� �Ҵ�
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        if (navMeshObstacle != null)
        {
            navMeshObstacle.carving = true;
        }
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

        DisableCollider();

        // �� ����
        if (doorState)
        {
            if (doorID == "DayTimeLine" && !inplayerTimeLine)
            {
                inplayerTimeLine = true;
            }
            endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + 110, 0);
            DisableObstacle(); // �� ������ NavMeshObstacle ����
            // SoundManager.instance.PlayDoorOpen();
        }
        else // �� ����
        {
            endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y - 110, 0);
            EnableObstacle(); // �� ������ NavMeshObstacle ����
            // SoundManager.instance.PlayDoorClose();
        }

        while (startTime < endTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, startTime / endTime);
            startTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        doorState = !doorState;
        isRotation = false;

        EnableCollider();
    }

    public void DisableCollider() => boxcollider.enabled = false;
    public void EnableCollider() => boxcollider.enabled = true;

    public void istrigger_on() => boxcollider.isTrigger = true;
    public void istrigger_off() => boxcollider.isTrigger = false;

    private void EnableObstacle()
    {
        if (navMeshObstacle != null)
            navMeshObstacle.enabled = true;
    }

    private void DisableObstacle()
    {
        if (navMeshObstacle != null)
            navMeshObstacle.enabled = false;
    }
}
