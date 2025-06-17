using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class NHSupervisor : MonoBehaviour
{
    // �̵��� ��ǥ Transform �迭 (�ν����Ϳ��� ����)
    [SerializeField]
    [Tooltip("ĳ���Ͱ� ���������� �̵��� ��ǥ �������Դϴ�.")]
    private Transform[] moveTr;

    // ���� �̵��� ��ǥ ������ �ε���
    private int currentTargetIndex = 0;
    // ĳ���Ͱ� ���� �̵� ������ ���θ� ��Ÿ���� �÷���
    private bool isMoving = false;
    // ���� ���� ���� �̵� �ڷ�ƾ ����
    private Coroutine currentMoveCoroutine;
    public float speed = 5f;
    private void Start()
    {
        Invoke("StartMovingSequence", 10f);
      
    }
    /// <summary>
    /// ĳ���Ͱ� ����� ����Ʈ���� ���� �̵� �������� �����ϴ� ���� �޼���.
    /// �ܺ�(��: Ʈ���� ��ũ��Ʈ)���� �� �޼��带 ȣ���Ͽ� �̵��� �����մϴ�.
    /// </summary>
    /// <param name="speed">ĳ������ �̵� �ӵ��Դϴ�.</param>
    public void StartMovingSequence()
    {
        if (isMoving)
        {
            Debug.LogWarning($"{gameObject.name} (NHSupervisor): �̹� �̵� ���Դϴ�. ���ο� �̵� �������� ������ �� �����ϴ�.");
            return;
        }

        if (moveTr == null || moveTr.Length == 0)
        {
            Debug.LogError($"{gameObject.name} (NHSupervisor): �̵��� ��ǥ Transform�� �������� �ʾҽ��ϴ�! 'Move Tr' �迭�� ������ �Ҵ����ּ���.");
            return;
        }

        if (speed <= 0f)
        {
            Debug.LogWarning($"{gameObject.name} (NHSupervisor): �̵� �ӵ�({speed})�� 0 ���Ͽ��� �̵��� ������ �� �����ϴ�. ��� ���� �Է����ּ���.");
            return;
        }

        // �迭�� ù ��° ����(index 0)���� �̵��� �����մϴ�.
        currentTargetIndex = 0;

        // ���� �ڷ�ƾ�� �ִٸ� �����ϰ� �����ϰ� ���ο� �ڷ�ƾ ����
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }
        currentMoveCoroutine = StartCoroutine(MoveThroughPoints(speed));
        Debug.Log($"{gameObject.name} (NHSupervisor): �̵� ������ ����! (�ӵ�: {speed})");
    }

    /// <summary>
    /// ������ ��� ��ǥ ������ ������� �̵��ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    private IEnumerator MoveThroughPoints(float currentMoveSpeed)
    {
        isMoving = true;

        while (currentTargetIndex < moveTr.Length)
        {
            Transform targetPoint = moveTr[currentTargetIndex];

            if (targetPoint == null)
            {
                Debug.LogWarning($"{gameObject.name} (NHSupervisor): {currentTargetIndex}�� �ε����� ��ǥ ������ null�Դϴ�. ���� �������� �ǳʶݴϴ�.");
                currentTargetIndex++;
                continue;
            }

            Debug.Log($"{gameObject.name} (NHSupervisor): {currentTargetIndex + 1}��° ��ǥ ����({targetPoint.position})���� �̵� ��.");

            // ��ǥ �������� �����ϰ� �̵��� �մϴ�. ȸ���� �ִϸ��̼����� ó���˴ϴ�.
            yield return StartCoroutine(MoveToSingleTarget(targetPoint.position, currentMoveSpeed));

            // ���� ��ǥ ���� �ε��� ����
            currentTargetIndex++;
        }

        Debug.Log($"{gameObject.name} (NHSupervisor): ��� ��ǥ ���� �̵� �Ϸ�!");
        isMoving = false;
        currentMoveCoroutine = null; // �ڷ�ƾ ���� ����
    }

    /// <summary>
    /// ���� ��ǥ �������� �̵��ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    /// <param name="targetPosition">������ ��ǥ ��ġ</param>
    /// <param name="currentMoveSpeed">������ �̵� �ӵ�</param>
    private IEnumerator MoveToSingleTarget(Vector3 targetPosition, float currentMoveSpeed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f) // 0.05f�� ���� ���� ����
        {
            // �̵� �ӵ��� 0�� ������ ���� ���� ���� �� ���
            if (currentMoveSpeed <= 0.001f)
            {
                Debug.LogWarning($"{gameObject.name} (NHSupervisor): �̵� �ӵ��� �ʹ� ����({currentMoveSpeed}) �̵��� �ߴܵ˴ϴ�. ���� ��ǥ �������� �̵��Ϸ��� �ӵ��� �����ּ���.");
                break; // ���� �ߴ�
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentMoveSpeed * Time.deltaTime);
            yield return null; // ���� �����ӱ��� ���
        }

        transform.position = targetPosition; // ��Ȯ�� ��ǥ ��ġ ���� (���� ����)
        // yield return new WaitForSeconds(0.2f); // �� ���� ���� �� ��� ��� (���� ����, �ʿ�� �ּ� ����)
    }

    // ���� ���Ǹ� ���� �ð�ȭ (�����) - Unity �����Ϳ����� ���Դϴ�.
    private void OnDrawGizmos()
    {
        if (moveTr == null || moveTr.Length == 0) return;

        // �̵� ��� �ð�ȭ
        Gizmos.color = Color.cyan;
        for (int i = 0; i < moveTr.Length; i++)
        {
            if (moveTr[i] != null)
            {
                // �� ��ǥ ���� ǥ��
                Gizmos.DrawSphere(moveTr[i].position, 0.2f);
                // ���� ��ȣ ǥ�� (������ ����)
#if UNITY_EDITOR
                UnityEditor.Handles.Label(moveTr[i].position + Vector3.up * 0.5f, $"Point {i + 1}"); // 1���� �����ϴ� ��ȣ
#endif

                // ���� ���������� �� ǥ��
                if (i < moveTr.Length - 1 && moveTr[i + 1] != null)
                {
                    Gizmos.DrawLine(moveTr[i].position, moveTr[i + 1].position);
                }
            }
        }

        // ���� ������Ʈ���� ù ��° ��ǥ ���������� �� (���� �̵� ���� ���� ��)
        if (!isMoving && moveTr.Length > 0 && moveTr[0] != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, moveTr[0].position);
        }
    }
}