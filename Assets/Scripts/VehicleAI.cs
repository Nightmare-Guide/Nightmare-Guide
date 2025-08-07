using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : MonoBehaviour
{
    public float speed; // 기본 속도
    public float slowSpeed = 10f; // 마지막 Waypoint로 갈 때 속도
    public float decelerationDistance = 3f; // 속도를 줄이기 시작할 거리
    [SerializeField] GameObject nextCar;
    [SerializeField] int nextCarAppearanceTime = 0;

    [Header("WayPointNav")]
    [SerializeField] private List<Transform> waypoint; // 자동차가 이동할 라인
    [SerializeField] private NavMeshAgent agent;
    private int currentNode = 0;
    bool isTurning = false;

    [Header("WheelAnim")]
    [SerializeField] private Animator[] anim_Wheel;
    [SerializeField] private Camera playerCamera;

    private bool isPlayerInRange = false;
    private bool isShinho = false;

    public bool offline = false; // 주차용 차량 판별
    public bool loopCar = true;

    void Start()
    {
        if (offline)
        {
            OffCar();
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

        if (nextCar != null)
        {
            StartCoroutine(ActivateNextCar()); // 다음 차 활성화
        }
    }

    private void Update()
    {
        CheckVehicleAI();
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


    // 메인 AI 함수
    void CheckVehicleAI()
    {
        if (offline || isPlayerInRange) return;

        // 플레이어 카메라에 안들어왔을 때, 불필요한 애니메이션 실행X
        if (IsVisibleFromCamera(playerCamera)) // 들어와 있을 때
        {
            foreach (Animator wheelAnim in anim_Wheel)
            {
                if (wheelAnim == null) continue;

                if (!wheelAnim.enabled) { wheelAnim.enabled = true; }
            }
        }
        else // 안 들어와 있을 때
        {
            foreach (Animator wheelAnim in anim_Wheel)
            {
                if (wheelAnim == null) continue;

                if (wheelAnim.enabled) { wheelAnim.enabled = false; }
            }
        }

        HandleDeceleration();

        if (isShinho || agent.pathPending) return;

        if (agent.remainingDistance < 2f)
        {
            GotoNext();
        }
    }

    // 다음 Point 설정
    private void GotoNext()
    {
        if (waypoint.Count == 0) return;

        if (currentNode >= waypoint.Count && loopCar)
        {
            currentNode = 0;
        }

        agent.destination = waypoint[currentNode].position;
        currentNode++;
    }


    // 이동 함수
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

        //if (other.CompareTag("Player") || other.CompareTag("Shinho"))
        if (other.CompareTag("Player") || other.CompareTag("CarBehind"))
        {
            // 에이전트 이동 중지
            agent.speed = 0;
            agent.isStopped = true;
            agent.ResetPath(); // 현재 경로 초기화
            agent.velocity = Vector3.zero; // 즉시 멈추도록 속도 0으로 설정
            agent.angularSpeed = 0f; // 회전 속도 0

            isPlayerInRange = true;
            SetWheelAnimation("Stop");
        }
        else if (other.CompareTag("LeftPoint") || other.CompareTag("RightPoint"))
        {
            HandleTurnAnimation(other.tag);
        }
        else if (other.CompareTag("decelerationRange"))
        {
            if (!other.transform.parent.gameObject.name.Equals(waypoint[currentNode].transform.parent.name))
                return;

            isTurning = true;
            agent.speed = slowSpeed;
            //Debug.Log("감속중");
        }
        else if (other.CompareTag("accelerationRange"))
        {
            isTurning = false;
            agent.speed = speed;
            //Debug.Log("가속중");
        }
        else if (other.CompareTag("ResetPoint"))
        {
            ResetPosition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (offline) return;

        //if (other.CompareTag("Player") || other.CompareTag("Shinho"))
        if (other.CompareTag("Player") || other.CompareTag("CarBehind"))
        {
            isPlayerInRange = false;
            agent.isStopped = false;
            agent.SetDestination(waypoint[currentNode].position); // 다음 경로 다시 복구

            if (other.CompareTag("CarBehind")) 
            {
                if (isTurning) { agent.speed = slowSpeed; }
                else { StartCoroutine(MoveAgain()); }
            }
            else
            {
                agent.speed = isTurning ? slowSpeed : speed;
            }

            agent.angularSpeed = 120f; // 회전 속도 복구
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
            if(anim.enabled == false) continue;
            anim.SetBool("Left", state == "Left");
            anim.SetBool("Right", state == "Right");
            anim.SetBool("Stop", state == "Stop");
        }
    }

    private void ResetAnimation()
    {
        SetWheelAnimation("Idle");
    }

    void OffCar()//오프라인 자동차는 하위 바퀴 애니메이션 종료
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool wheel = transform.GetChild(i).name.ToLower().Contains("wheel");
            if (wheel)
            {
                Animator anim = transform.GetChild(i).GetComponent<Animator>();
                if (anim != null)
                {
                    anim.enabled = false;
                }
                else
                {
                    //Debug.LogWarning(transform.GetChild(i).name+" 에 애니메이터 없음!");
                }
            }
        }
    }

    IEnumerator ActivateNextCar()
    {
        yield return new WaitForSeconds(nextCarAppearanceTime);

        nextCar.SetActive(true);
    }

    IEnumerator MoveAgain()
    {
        agent.speed = slowSpeed;

        yield return new WaitForSeconds(nextCarAppearanceTime / 2);

        agent.speed = speed;
    }

    // 카메라에 인식되었는 지 확인하는 함수
    bool IsVisibleFromCamera(Camera cam, float maxDistance = 50f)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);

        // 카메라 시야 안에 있고,
        // 카메라와의 거리가 maxDistance 이하일 경우에만 true
        return viewPos.z > 0 &&
               viewPos.x >= 0 && viewPos.x <= 1 &&
               viewPos.y >= 0 && viewPos.y <= 1 &&
               Vector3.Distance(cam.transform.position, transform.position) <= maxDistance;
    }
}
