using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CellPhone : MonoBehaviour
{
    // Position 값
    public Vector3 finalPos;
    public float moveSpeed;

    // Rotatioin 값
    public Vector3 finalRotate;
    public float rotateSpeed;

    public SchoolUIManager schoolUIManager;

    public void UpPhone(Vector3 pos, Vector3 rotate)
    {
        finalPos = pos;
        finalRotate = rotate;
        StartCoroutine(DoTween());
    }


    IEnumerator DoTween()
    {
        // InQuad : 시작할 때 빠르게 가속, 끝날 때 감속
        // OutQuad : 시작할 때 감속, 끝날 때 가속
        transform.DOMove(finalPos, moveSpeed).SetEase(Ease.InOutQuad);

        transform.DORotate(finalRotate, rotateSpeed).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(moveSpeed - 0.12f);

        schoolUIManager.OpenPhoneUI();

        // BoxCollider 비활성화
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
