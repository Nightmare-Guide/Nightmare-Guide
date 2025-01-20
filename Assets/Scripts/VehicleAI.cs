using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : MonoBehaviour
{
    public float speed = 10f; // �⺻ �ӵ�
    public float slowSpeed = 2f; // ������ Waypoint�� �� �� �ӵ�
    public float decelerationDistance = 3f; // �ӵ��� ���̱� ������ �Ÿ�

    [Header("WayPointNav")]
    [SerializeField] private List<Transform> waypoint = new List<Transform>(); // �ڵ����� �̵��� ����
    private NavMeshAgent agent;
    private int currentNode = 0;

    [Header("WheelAnim")]
    [SerializeField] private Animator[] anim_Wheel;

    private bool isPlayerInRange = false;
    private bool isShinho = false;

    public bool offline = false; // ������ ���� �Ǻ�
    public bool roopCar = true;
    void Start()
    {
        // NavMeshAgent �� Waypoint �ʱ�ȭ ����
        agent = GetComponent<NavMeshAgent>();
        if (agent == null || waypoint == null || waypoint.Count == 0)
        {
            Debug.LogError("NavMeshAgent �Ǵ� Waypoint�� ����� �������� �ʾҽ��ϴ�.");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        if (offline) return; // ���� ������ �ʱ�ȭ �۾� ����

        agent.autoBraking = false;
        agent.speed = speed;
        GotoNext(); // ù Waypoint�� �̵�
        SetWheelAnimation("Idle"); // �⺻ �ִϸ��̼�
    }

    void Update()
    {
        if (offline || isPlayerInRange) return;

        HandleDeceleration();
    }

    void FixedUpdate()
    {
        if (offline || isPlayerInRange || isShinho || agent.pathPending) return;

        if (agent.remainingDistance < 2f)
        {
            GotoNext();
        }
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < waypoint.Count; i++)
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            Gizmos.DrawSphere(waypoint[i].transform.position, 2);
            Gizmos.DrawWireSphere(waypoint[i].transform.position, 20f);

            if (i < waypoint.Count - 1)
            {
                if (waypoint[i] && waypoint[i + 1])
                {
                    Gizmos.color = Color.red;
                    if (i < waypoint.Count - 1)
                        Gizmos.DrawLine(waypoint[i].position, waypoint[i + 1].position);
                    if (i < waypoint.Count - 2)
                    {
                        Gizmos.DrawLine(waypoint[waypoint.Count - 1].position, waypoint[0].position);
                    }
                }
            }
        }
    }
    private void GotoNext()
    {
        if (waypoint.Count == 0) return;

        // ������ Waypoint�� �����ϸ� �ʱ�ȭ
        if (currentNode >= waypoint.Count && !roopCar)
        {
            currentNode = 0;
            agent.speed = speed; // �ӵ� ����
        }

        // ��ȿ�� Waypoint�� �̵�
        if (waypoint[currentNode] != null)
        {
            agent.destination = waypoint[currentNode].position;
            currentNode++;
        }
    }

    private void HandleDeceleration()
    {
        if (currentNode == waypoint.Count - 1 && agent.remainingDistance <= decelerationDistance)
        {
            agent.speed = Mathf.Lerp(agent.speed, slowSpeed, Time.deltaTime);
            Invoke(nameof(ResetPosition), 1f); // 1�� �� ��ġ �ʱ�ȭ
        }
    }

    private void ResetPosition()
    {
        transform.position = waypoint[0].position;
        currentNode = 0;
        agent.speed = speed; // �⺻ �ӵ��� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (offline) return;

        if (other.CompareTag("Player") || other.CompareTag("Shinho"))
        {
            isPlayerInRange = true;
            agent.isStopped = true;
            SetWheelAnimation("Stop");
        }
        else if (other.CompareTag("LeftPoint") || other.CompareTag("RightPoint"))
        {
            HandleTurnAnimation(other.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (offline) return;

        if (other.CompareTag("Player") || other.CompareTag("Shinho"))
        {
            isPlayerInRange = false;
            agent.isStopped = false;
            agent.speed = speed; // �⺻ �ӵ��� ����
            SetWheelAnimation("Idle");
        }
    }

    private void HandleTurnAnimation(string tag)
    {
        if (tag == "LeftPoint")
        {
            SetWheelAnimation("Left");
        }
        else if (tag == "RightPoint")
        {
            SetWheelAnimation("Right");
        }
        Invoke(nameof(ResetAnimation), 2f);
    }

    private void SetWheelAnimation(string state)
    {
        foreach (var anim in anim_Wheel)
        {
            anim.SetBool("Left", state == "Left");
            anim.SetBool("Right", state == "Right");
            anim.SetBool("Stop", state == "Stop");
        }
    }

    private void ResetAnimation()
    {
        SetWheelAnimation("Idle");
    }
}
