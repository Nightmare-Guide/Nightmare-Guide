using DG.DemiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class Alex : NPC
{
    [Header("# Ray")]
    [SerializeField] private float sphereRadius = 2f;
    [SerializeField] private float findRange = 45f;
    [SerializeField] private float rayMaxDist = 2f;
    [SerializeField] private GameObject detectObj;
    [SerializeField] private LayerMask layerMask;

    [Header("# ETC")]
    public Transform[] targetTransform; // 걷고 싶은 목표 지점
    public NavMeshAgent agent;
    bool openingDoor = false;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Player", "ActiveObject");
    }

    private void Start()
    {
        col.enabled = false;
        agent.SetDestination(targetTransform[0].position);
        StartCoroutine(EnableCollider(col, 2f));
        AnimHelper.TryPlay(myAnim, "walk", 0);
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                // 도착 완료
                Debug.Log("목표 도착!");
            }
        }

        if (!inAction)
        {
            Ray();
        }
    }

    void Ray()
    {
        Vector3 rayStart = transform.position;
        Vector3 rayDir = transform.forward;

        Quaternion leftRot = Quaternion.Euler(0, -findRange * 0.5f, 0);
        Vector3 leftDir = leftRot * rayDir;
        float leftRad = Mathf.Acos(Vector3.Dot(rayDir, leftDir));
        float leftDeg = -(Mathf.Rad2Deg * leftRad);

        Quaternion rightRot = Quaternion.Euler(0, findRange * 0.5f, 0);
        Vector3 rightDir = rightRot * rayDir;
        float rightRad = Mathf.Acos(Vector3.Dot(rayDir, rightDir));
        float rightDeg = Mathf.Rad2Deg * rightRad;

        Debug.DrawRay(rayStart, rayDir * sphereRadius, Color.red);
        Debug.DrawRay(rayStart, leftDir * sphereRadius, Color.green);
        Debug.DrawRay(rayStart, rightDir * sphereRadius, Color.blue);

        RaycastHit hit;
        if (Physics.SphereCast(rayStart, sphereRadius, rayDir, out hit, rayMaxDist, layerMask))
        {
            Debug.Log("감지된 오브젝트: " + hit.collider.name);

            GameObject hitObj = hit.transform.gameObject;

            Vector3 hitDir = (hitObj.transform.position - rayStart).normalized;
            float hitRad = Mathf.Acos(Vector3.Dot(rayDir, hitDir));
            float hitDeg = Mathf.Rad2Deg * hitRad;

            if (hitDeg >= leftDeg && hitDeg <= rightDeg)
            {
                //Debug.Log(hit.transform.gameObject.name);
                detectObj = hit.transform.gameObject;

                if(detectObj.layer == LayerMask.NameToLayer("Player"))
                {
                    IsBlockedByPlayer();
                }
            }
            else
            {
                detectObj = null;
                Walk();
            }
        }
    }

    void FirstMeet()
    {
        inAction = true;
        agent.isStopped = true;
        story = "2_2_0";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        AnimHelper.TryPlay(myAnim, "talk1", 0f);
        col.enabled = false;

        LookAtPlayer();
    }

    void Walk()
    {
        inAction = false;
        agent.isStopped = false;
        AnimHelper.TryPlay(myAnim, "walk", 0f);
    }

    public void WalkToOutSide()
    {
        Walk();

        //카메라 회전 활성화
        Camera_Rt.instance.Open_Camera();

        //플레이어 컨트롤 On
        PlayerController.instance.Open_PlayerController();
    }

    void IsBlockedByPlayer()
    {
        agent.isStopped = true;
        AnimHelper.TryPlay(myAnim, "idle1", 0f);
    }

    void OpenDoor()
    {
        inAction = true;
        openingDoor = true;
        AnimHelper.TryPlay(myAnim, "idle1", 0f);
        agent.SetDestination(targetTransform[1].position);
        Debug.Log("Open Door");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            FirstMeet();
        }
    }
}
