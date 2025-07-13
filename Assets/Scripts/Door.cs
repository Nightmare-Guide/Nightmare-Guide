using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public int lockerIndex;
    public bool doorState; // true = ¿­¸², false = ´ÝÈû
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
        doorState = false;

        // NavMeshObstacle ÀÚµ¿ ÇÒ´ç
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

        // ¹® ´Ý±â
        if (doorState)
        {
            if(this.gameObject.name.Contains("Janitor's office") || this.gameObject.name.Contains("LockerRoomDoor(Left)")) { endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y - 90, 0); }
            else { endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + 90, 0); }
            doorState = false;
            EnableObstacle(); // ¹® ´ÝÈ÷¸é NavMeshObstacle ÄÑÁü
            if(SoundManager.instance != null) { SoundManager.instance.PlayDoorClose(); }
        }
        else // ¹® ¿­±â
        {
            if (this.gameObject.name.Contains("Janitor's office") || this.gameObject.name.Contains("LockerRoomDoor(Left)")) { endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + 90, 0); }
            else { endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y - 90, 0); }
            doorState = true;
            DisableObstacle(); // ¹® ¿­¸®¸é NavMeshObstacle ²¨Áü
            if (SoundManager.instance != null) { SoundManager.instance.PlayDoorClose(); }
        }

        while (startTime < endTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, startTime / endTime);
            startTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
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
