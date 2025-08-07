using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.AI;

public class VehicleAI : MonoBehaviour
{
    public float speed; // �⺻ �ӵ�
    public float slowSpeed = 10f; // ������ Waypoint�� �� �� �ӵ�
    public float decelerationDistance = 3f; // �ӵ��� ���̱� ������ �Ÿ�
    [SerializeField] GameObject nextCar;
    [SerializeField] int nextCarAppearanceTime = 0;

    [Header("WayPointNav")]
    [SerializeField] private List<Transform> waypoint; // �ڵ����� �̵��� ����
    [SerializeField] private NavMeshAgent agent;
    private int currentNode = 0;
    bool isTurning = false;

    [Header("WheelAnim")]
    [SerializeField] private Animator[] anim_Wheel;
    [SerializeField] private Camera playerCamera;

    private bool isPlayerInRange = false;
    private bool isShinho = false;

    public bool offline = false; // ������ ���� �Ǻ�
    public bool loopCar = true;

    void Start()
    {
        if (offline)
        {
            OffCar();
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        // NavMeshAgent �� Waypoint �ʱ�ȭ ����
        agent = GetComponent<NavMeshAgent>();


        if (agent == null || waypoint == null || waypoint.Count == 0)
        {
            //Debug.LogError(" Waypoint�� ����� �������� �ʾҽ��ϴ�."); 

            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }


        agent.autoBraking = false;
        agent.speed = speed;
        //Debug.Log(this.name+ " => ���� ���� �ӵ� : " + speed);
        GotoNext(); // ù Waypoint�� �̵�
        SetWheelAnimation("Idle"); // �⺻ �ִϸ��̼�

        if (nextCar != null)
        {
            StartCoroutine(ActivateNextCar()); // ���� �� Ȱ��ȭ
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


    // ���� AI �Լ�
    void CheckVehicleAI()
    {
        if (offline || isPlayerInRange) return;

        // �÷��̾� ī�޶� �ȵ����� ��, ���ʿ��� �ִϸ��̼� ����X
        if (IsVisibleFromCamera(playerCamera)) // ���� ���� ��
        {
            foreach (Animator wheelAnim in anim_Wheel)
            {
                if (wheelAnim == null) continue;

                if (!wheelAnim.enabled) { wheelAnim.enabled = true; }
            }
        }
        else // �� ���� ���� ��
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

    // ���� Point ����
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


    // �̵� �Լ�
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
        //Debug.Log("���� ��ġ�� �ʱ�ȭ�մϴ�.");
        GotoNext();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (offline) return;

        //if (other.CompareTag("Player") || other.CompareTag("Shinho"))
        if (other.CompareTag("Player") || other.CompareTag("CarBehind"))
        {
            // ������Ʈ �̵� ����
            agent.speed = 0;
            agent.isStopped = true;
            agent.ResetPath(); // ���� ��� �ʱ�ȭ
            agent.velocity = Vector3.zero; // ��� ���ߵ��� �ӵ� 0���� ����
            agent.angularSpeed = 0f; // ȸ�� �ӵ� 0

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
            //Debug.Log("������");
        }
        else if (other.CompareTag("accelerationRange"))
        {
            isTurning = false;
            agent.speed = speed;
            //Debug.Log("������");
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
            agent.SetDestination(waypoint[currentNode].position); // ���� ��� �ٽ� ����

            if (other.CompareTag("CarBehind")) 
            {
                if (isTurning) { agent.speed = slowSpeed; }
                else { StartCoroutine(MoveAgain()); }
            }
            else
            {
                agent.speed = isTurning ? slowSpeed : speed;
            }

            agent.angularSpeed = 120f; // ȸ�� �ӵ� ����
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

    void OffCar()//�������� �ڵ����� ���� ���� �ִϸ��̼� ����
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
                    //Debug.LogWarning(transform.GetChild(i).name+" �� �ִϸ����� ����!");
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

    // ī�޶� �νĵǾ��� �� Ȯ���ϴ� �Լ�
    bool IsVisibleFromCamera(Camera cam, float maxDistance = 50f)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);

        // ī�޶� �þ� �ȿ� �ְ�,
        // ī�޶���� �Ÿ��� maxDistance ������ ��쿡�� true
        return viewPos.z > 0 &&
               viewPos.x >= 0 && viewPos.x <= 1 &&
               viewPos.y >= 0 && viewPos.y <= 1 &&
               Vector3.Distance(cam.transform.position, transform.position) <= maxDistance;
    }
}
