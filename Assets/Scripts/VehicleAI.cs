using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

public class VehicleAI : MonoBehaviour
{
    public float speed = 10f; // �⺻ �ӵ�
    public float slowSpeed = 2f; // ������ Waypoint�� �� �� �ӵ�
    public float decelerationDistance = 3f; // �ӵ��� ���̱� ������ �Ÿ�


    [Header("WayPointNav")]
    NavMeshAgent agent;
    private int currentNode = 0;
    [SerializeField] List<Transform> waypoint = new List<Transform>(); // �ڵ����� �̵��� ����

    [Header("WheelAnim")]
    [SerializeField] Animator[] anim_Wheel;

    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ���ϴ� �÷���
    private bool isShinho = false; // ��ȣ�� �ν�

    public bool test = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = speed; // �⺻ �ӵ� ����
        isPlayerInRange = false;
        isShinho = false;
        GotoNext();
        IdleLine();
    }

    void Update()
    {
            if (!isPlayerInRange) // �÷��̾ ������ ���� ���� �̵�
            {
                HandleDeceleration();
            }
  
        CarAnim();
    }

    void FixedUpdate()
    {
        if (!isPlayerInRange && !isShinho && !agent.pathPending && agent.remainingDistance < 2f)
        {
            GotoNext(); // ���� Waypoint�� �̵�
        }
    }

    void GotoNext()
    {
        if (currentNode == waypoint.Count - 1)
        {
            currentNode = 0;
            agent.speed = speed; // �ӵ� ������� ����
        }
        else
        {
            // ���� Waypoint�� �̵�
            agent.destination = waypoint[currentNode].position;
            currentNode++;
        }
    }

    void HandleDeceleration()
    {
        
        if (currentNode == waypoint.Count - 1 && agent.remainingDistance <= decelerationDistance)
        {
            // ������ Waypoint�� ���� �� �ӵ� ���������� ���̱�
            agent.speed = Mathf.Lerp(agent.speed, slowSpeed, Time.deltaTime);
            Invoke("ClearPosition", 1f);//1�� �� ���� ��ġ�� ó�� ��ġ�� �ʱ�ȭ
        }
    }
    public void ClearPosition()//���� ��ġ �ʱ�ȭ
    {
        transform.position = waypoint[0].position;
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < waypoint.Count; i++)
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            //Gizmos.DrawSphere(waypoint[i].transform.position, 2);
            //Gizmos.DrawWireSphere(waypoint[i].transform.position, 20f);

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Shinho"))
        {
            isPlayerInRange = true;
            agent.isStopped = true; // ���� ����
            StopLine(); // ���� �ִϸ��̼�
        }
        if (other.CompareTag("LeftPoint")|| other.CompareTag("RightPoint")) // ������ ȸ���� Ÿ�ֿ̹� �ִϸ��̼� ���� 
        {
            RotationPoint(other.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Shinho"))
        {
            isPlayerInRange = false;
            agent.isStopped = false; // ���� �����
            agent.speed = speed; // �⺻ �ӵ��� ����
            IdleLine(); // �⺻ �ִϸ��̼�
        }
        
    }
    public void CarAnim()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LeftRine();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            RightLine();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            IdleLine();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StopLine();
        }
    }
    public void RotationPoint(string rot)
    {
        if (rot.Equals("LeftPoint"))
        {
            LeftRine();
            Invoke("IdleLine", 2f);
        }
        else if(rot.Equals("RightPoint"))
        {
            RightLine();
            Invoke("IdleLine", 2f);
        }
        else
        {
            StopLine();
        }
    }

    public void LeftRine()
    {
        foreach (Animator anim in anim_Wheel)
        {
            anim.SetBool("Right", false);
            anim.SetBool("Stop", false);
            anim.SetBool("Left", true);
        }
        Debug.Log("��ȸ����");
    }

    public void RightLine()
    {
        foreach (Animator anim in anim_Wheel)
        {
            anim.SetBool("Left", false);
            anim.SetBool("Stop", false);
            anim.SetBool("Right", true);
        }
        Debug.Log("��ȸ����");
    }

    public void IdleLine()
    {
        foreach (Animator anim in anim_Wheel)
        {
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
            anim.SetBool("Stop", false);
        }
    }

    public void StopLine()
    {
        foreach (Animator anim in anim_Wheel)
        {
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
            anim.SetBool("Stop", true);
        }
    }

    
}
