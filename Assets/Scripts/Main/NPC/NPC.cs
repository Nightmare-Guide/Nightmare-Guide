using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class NPC : MonoBehaviour
{
    public string story;
    public Collider col;
    public Transform npcTransform;
    public Transform playerTransform;
    public Animator myAnim;
    public bool inAction = false;

    public IEnumerator EnableCollider(Collider col, float time)
    {
        yield return new WaitForSeconds(time);
        col.enabled = true;
    }

    public void LookAtPlayer()
    {
        // �÷��̾�, NPC ���� ���ֺ��� �ϱ�
        StartCoroutine(SmoothLookAt(npcTransform, playerTransform, 0.35f));
        StartCoroutine(SmoothLookAt(playerTransform, npcTransform, 0.35f));

        // �÷��̾� ī�޶� ���� ����
        StartCoroutine(SmoothRotateTo(PlayerController.instance.GetPlayerCamera().transform, Vector3.zero, 0.35f));

        // �÷��̾� ��Ʈ�ѷ� ��Ȱ��ȭ
        CommonUIManager.instance.uiManager.StopPlayerController();

        CommonUIManager.instance.isTalkingWithNPC = true;
    }

    public IEnumerator SmoothLookAt(Transform me,Transform target, float duration)
    {
        Vector3 direction = target.position - me.position;
        direction.y = 0f; // ���� ���� ���� (�� ���̴� �� ����)

        Quaternion startRotation = me.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        Vector3 r = targetRotation.eulerAngles;
        r = r - new Vector3(0, 5, 0);
        targetRotation = Quaternion.Euler(r);

        float time = 0f;
        while (time < duration)
        {
            me.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        me.rotation = targetRotation; // ������ ���� ����
    }

    public IEnumerator SmoothRotateTo(Transform targetTransform, Vector3 targetEulerAngles, float duration)
    {
        Quaternion startRotation = targetTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetTransform.localRotation = targetRotation; // ������ ���� ����
    }
}
