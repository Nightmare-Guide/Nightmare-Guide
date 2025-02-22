using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CellPhone : MonoBehaviour
{
    // Position ��
    public Vector3 finalPos;
    public float moveSpeed;

    // Rotatioin ��
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
        // InQuad : ������ �� ������ ����, ���� �� ����
        // OutQuad : ������ �� ����, ���� �� ����
        transform.DOMove(finalPos, moveSpeed).SetEase(Ease.InOutQuad);

        transform.DORotate(finalRotate, rotateSpeed).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(moveSpeed - 0.12f);

        schoolUIManager.OpenPhoneUI();

        // BoxCollider ��Ȱ��ȭ
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
