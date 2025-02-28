using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Ž�� ����")]
    public float detectionRadius = 10f; // ���� �Ÿ�
    [Range(0, 360)] public float detectionAngle = 120f; // ���� �þ߰� (120��)

    [Header("���̾� ����")]
    public LayerMask obstacleLayer; // ��ֹ� ���̾�

    [Header("�÷��̾� ����")]
    [SerializeField] private GameObject player;

    [Header("Ž�� ����")]
    [SerializeField] private bool isDetected = false; // �÷��̾� Ž�� ���� (�ν����Ϳ��� Ȯ�� ����)
    public BehaviourTree behaviourTree; // �����̺�� Ʈ�� ����
    private Blackboard blackboard; // ������ ����

    private float detectionCooldown = 1f; // ���� �� �ٽ� üũ�ϱ� ���� ��� �ð�
    private bool canDetect = true; // ���� �������� ����

    void Start()
    {
        blackboard = behaviourTree.blackboard; // ������ ��������
    }

    void Update()
    {
        if (Chapter1_Mgr.instance.player != null && player == null)
        {
            player = Chapter1_Mgr.instance.player;
            Debug.Log("�����");
        }

        if (player != null)
        {
            // �÷��̾� Ž�� ���θ� Ȯ���ϰ�, Ž���Ǿ����� �˻縦 ����
            if (canDetect)
            {
                isDetected = CheckPlayerInView(); // �÷��̾� Ž�� ���� Ȯ��
                if (isDetected)
                {
                    StartCoroutine(HandleDetectionCooldown()); // Ž�� �� ��Ÿ�� ó��
                }
            }
        }

        if (isDetected)
        {
            blackboard.isDetected = true; // ������ �� ������Ʈ
            /*Debug.Log("�÷��̾� �߰�! �߰� ����!");*/
            // �߰� ���·� ��ȯ�ϴ� �ڵ� �߰� ����
        }
        else
        {
            blackboard.isDetected = false; // �÷��̾ Ž������ ������ ������ �� �ʱ�ȭ
        }
    }

    private bool CheckPlayerInView()
    {
        if (player == null) return false;

        // 1. �Ÿ� üũ
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > detectionRadius) return false;

        // 2. �þ߰� üũ
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > detectionAngle / 2) return false;

        // 3. ��ֹ� üũ (����ĳ��Ʈ)
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
        {
            return false; // ��ֹ��� ������ ������ Ž�� X
        }

        return true; // ��� ������ �����ϸ� Ž�� ����
    }

    // Ž�� �� ���� �ð� ���� �ٽ� Ž������ �ʵ��� ����ϴ� �ڷ�ƾ
    private IEnumerator HandleDetectionCooldown()
    {
        canDetect = false; // ���� �ð� ���� Ž�� ����
        yield return new WaitForSeconds(detectionCooldown); // ��� �ð� (��: 2��)
        canDetect = true; // �ٽ� Ž�� ����
    }

    public bool IsPlayerDetected()
    {
        return isDetected;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // ���� ���� ǥ��

        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);

        if (isDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.transform.position); // Ž���� �÷��̾���� ���ἱ
        }
    }
}
