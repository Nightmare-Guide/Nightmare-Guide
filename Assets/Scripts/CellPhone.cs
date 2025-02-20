using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CellPhone : MonoBehaviour
{
    // Position 값
    public Vector3 lastPos;
    public float moveSpeed;

    // Rotatioin 값
    public Vector3 lastRotate;
    public float rotateSpeed;

    public SchoolUIManager schoolUIManager;


    public void UpPhone()
    {
        StartCoroutine(DoTween());
    }


    IEnumerator DoTween()
    {
        // InQuad : 시작할 때 빠르게 가속, 끝날 때 감속
        // OutQuad : 시작할 때 감속, 끝날 때 가속
        transform.DOMove(lastPos, moveSpeed).SetEase(Ease.InOutQuad);

        transform.DORotate(lastRotate, rotateSpeed).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(0.7f);

        schoolUIManager.OpenPhoneUI();
    }
}
