using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CellPhone : MonoBehaviour
{
    // Position ��
    public Vector3 lastPos;
    public float moveSpeed;

    // Rotatioin ��
    public Vector3 lastRotate;
    public float rotateSpeed;

    public SchoolUIManager schoolUIManager;


    public void UpPhone()
    {
        StartCoroutine(DoTween());
    }


    IEnumerator DoTween()
    {
        // InQuad : ������ �� ������ ����, ���� �� ����
        // OutQuad : ������ �� ����, ���� �� ����
        transform.DOMove(lastPos, moveSpeed).SetEase(Ease.InOutQuad);

        transform.DORotate(lastRotate, rotateSpeed).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(0.7f);

        schoolUIManager.OpenPhoneUI();
    }
}
