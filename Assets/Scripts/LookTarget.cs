using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
    public Transform target;
    public float rotationDuration = 0.5f; //���� �ð�

    public void Look()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f; // Y�ุ ȸ���ϰ� ����

        if (direction.sqrMagnitude < 0.001f) yield break; // �ʹ� ������ ȸ������ ����

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
