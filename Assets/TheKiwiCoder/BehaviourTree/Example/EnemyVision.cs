using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyVision : MonoBehaviour
{
    [Header("Ž�� ����")]
    public float detectionRadius = 10f; // ���� �Ÿ�
    [Range(0, 360)] public float detectionAngle = 120f; // ���� �þ߰� (120��)

    [Header("���̾� ����")]
    public LayerMask playerLayer; // �÷��̾� ���̾�
    public LayerMask obstacleLayer; // ��ֹ� ���̾�

    [Header("�÷��̾� ����")]
    [SerializeField] private Transform player;
    private bool isDetected = false; // �÷��̾� Ž�� ����

    void Start()
    {
        player = GameManager.instance?.player; // GameManager���� �÷��̾� ���� ��������
    }

    void Update()
    {
        if (player != null)
        {
            isDetected = CheckPlayerInView(); // �÷��̾� Ž�� ���� Ȯ��
        }

        if (isDetected)
        {
            Debug.Log("�÷��̾� �߰�! �߰� ����!");
            // �߰� ���·� ��ȯ�ϴ� �ڵ� �߰� ���� (��: ���� �ӽ� ����)
        }
    }

    private bool CheckPlayerInView()
    {
        if (player == null) return false;

        // 1. �÷��̾���� �Ÿ� Ȯ��
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius) return false;

        // 2. �÷��̾ �þ߰� ���� �ִ��� Ȯ��
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > detectionAngle / 2) return false; // �þ߰� ���̸� Ž�� X

        // 3. ��ֹ� üũ (����ĳ��Ʈ�� �þ߸� �����ϴ� ������Ʈ�� �ִ��� Ȯ��)
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
        {
            return false; // ��ֹ��� ������ ������ Ž�� X
        }

        return true; // ��� ������ �����ϸ� Ž�� ����!
    }

    // �÷��̾ ������ ���¸� ��ȯ
    public bool IsPlayerDetected()
    {
        return isDetected;
    }

    // ���� �þ߸� �ð������� ǥ���ϱ� ���� �����
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
            Gizmos.DrawLine(transform.position, player.position); // Ž���� �÷��̾���� ���ἱ
        }
    }
}
