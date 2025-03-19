using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : MonoBehaviour
{
    public float speed ; // 기본 속도
    public float slowSpeed = 10f; // 마지막 Waypoint로 갈 때 속도
    public float decelerationDistance = 3f; // 속도를 줄이기 시작할 거리

    [Header("WayPointNav")]
    [SerializeField] private List<Transform> waypoint = new List<Transform>(); // 자동차가 이동할 라인
    [SerializeField] private NavMeshAgent agent;
    private int currentNode = 0;

    [Header("WheelAnim")]
    [SerializeField] private Animator[] anim_Wheel;

    private bool isPlayerInRange = false;
    private bool isShinho = false;

    public bool offline = false; // 주차용 차량 판별
    public bool loopCar = true;
  
    void Start()
    {
        if (offline)
        {
            enabled = false; // 스크립트 비활성화
            return;
        }

        // NavMeshAgent 및 Waypoint 초기화 검증
        agent = GetComponent<NavMeshAgent>();
      

        if (agent == null || waypoint == null || waypoint.Count == 0)
        {
            //Debug.LogError(" Waypoint가 제대로 설정되지 않았습니다."); 
             
            enabled = false; // 스크립트 비활성화
            return;
        }
        

        agent.autoBraking = false;
        agent.speed = speed;
        //Debug.Log(this.name+ " => 차량 현재 속도 : " + speed);
        GotoNext(); // 첫 Waypoint로 이동
        SetWheelAnimation("Idle"); // 기본 애니메이션
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
            Gizmos.DrawWireSphere(waypoint[i].transform.position, 10f);

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
                //Debug.Log("차량이 배회합니다.");
            
            
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
        agent.speed = speed; // 기본 속도로 복구
        //Debug.Log("차량 위치를 초기화합니다.");
        GotoNext();
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
        if (other.CompareTag("decelerationRange"))
        {
            if (!other.transform.parent.gameObject.name.Equals(waypoint[currentNode].transform.parent.name))
                return;
            agent.speed = 10f;
            //Debug.Log("감속중");
        }
        if (other.CompareTag("accelerationRange"))
        {
            agent.speed = speed;
            //Debug.Log("가속중");
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
            agent.speed = speed; // 기본 속도로 복구
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
