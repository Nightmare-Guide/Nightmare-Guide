using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : MonoBehaviour
{
    public float speed ; // �⺻ �ӵ�
    public float slowSpeed = 10f; // ������ Waypoint�� �� �� �ӵ�
    public float decelerationDistance = 3f; // �ӵ��� ���̱� ������ �Ÿ�

    [Header("WayPointNav")]
    [SerializeField] private List<Transform> waypoint = new List<Transform>(); // �ڵ����� �̵��� ����
    [SerializeField] private NavMeshAgent agent;
    private int currentNode = 0;

    [Header("WheelAnim")]
    [SerializeField] private Animator[] anim_Wheel;

    private bool isPlayerInRange = false;
    private bool isShinho = false;

    public bool offline = false; // ������ ���� �Ǻ�
    public bool loopCar = true;
  
    void Start()
    {
        if (offline)
        {
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        // NavMeshAgent �� Waypoint �ʱ�ȭ ����
        agent = GetComponent<NavMeshAgent>();
      

        if (agent == null || waypoint == null || waypoint.Count == 0)
        {
            Debug.LogError(" Waypoint�� ����� �������� �ʾҽ��ϴ�."); 
             
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }
        

        agent.autoBraking = false;
        agent.speed = speed;
        Debug.Log(this.name+ " => ���� ���� �ӵ� : " + speed);
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

        if (currentNode >= waypoint.Count&& loopCar)
        {
            
                currentNode = 0;
                Debug.Log("������ ��ȸ�մϴ�.");
            
            
        }
        agent.destination = waypoint[currentNode].position;
        currentNode++;
    }




    private void HandleDeceleration()
    {
        if (currentNode == waypoint.Count - 1 && agent.remainingDistance <= decelerationDistance && !loopCar)
        {
            agent.speed = Mathf.Lerp(agent.speed, slowSpeed, Time.deltaTime);
           
        }
    }

    private void ResetPosition()
    {
        transform.position = waypoint[0].position;
        currentNode = 0;
        agent.speed = speed; // �⺻ �ӵ��� ����
        Debug.Log("���� ��ġ�� �ʱ�ȭ�մϴ�.");
        GotoNext();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (offline) return;

        if (other.CompareTag("StopZone")){
            if (Stop_Car.instance.stop)
            {
                isPlayerInRange = true;
                agent.isStopped = true; // ���� ����
                SetWheelAnimation("Stop");
                
            }
            else 
            {
                isPlayerInRange = false;
                agent.isStopped = false;  // �̵� ����
                agent.speed = speed;
                SetWheelAnimation("Idle");
            }
        }

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
        if (other.CompareTag("decelerationRange"))
        {
            if (!other.transform.parent.gameObject.name.Equals(waypoint[currentNode].transform.parent.name))
                return;
            agent.speed = 10f;
            Debug.Log("������");
        }
        if (other.CompareTag("accelerationRange"))
        {
            agent.speed = speed;
            Debug.Log("������");
        }
        if (other.CompareTag("ResetPoint"))
        {
            ResetPosition();
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
