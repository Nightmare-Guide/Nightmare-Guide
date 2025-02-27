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
    public BehaviourTree behaviourTree;

    void Start()
    {

    }

    void Update()
    {
        if (Chapter1_Mgr.instance.player != null&& player == null)
        {
            player = Chapter1_Mgr.instance.player;
            Debug.Log("�����");
        }

        if (player != null)
        {
            isDetected = CheckPlayerInView(); // �÷��̾� Ž�� ���� Ȯ��
        }

        if (isDetected)
        {
            Debug.Log("�÷��̾� �߰�! �߰� ����!");
            // �߰� ���·� ��ȯ�ϴ� �ڵ� �߰� ����
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
