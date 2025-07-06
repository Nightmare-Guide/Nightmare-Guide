using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
    public Transform target;
    public float rotationDuration = 0.5f; //도는 시간

    public void Look()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f; // Y축만 회전하게 고정

        if (direction.sqrMagnitude < 0.001f) yield break; // 너무 가까우면 회전하지 않음

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
            yield return null;
        }
    }
}
