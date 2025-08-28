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
    [SerializeField] private GameObject detectObj;
    [SerializeField] private LayerMask layerMask;

    [Header("# ETC")]
    public Transform[] targetTransform; // 걷고 싶은 목표 지점
    bool isWalking = false;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Player", "ActiveObject");
    }

    private void Start()
    {
        if (ProgressManager.Instance != null && ProgressManager.Instance.IsActionCompleted(ProgressManager.ActionType.FirstMeetAlex))
        {
            npcTransform.gameObject.SetActive(false);
        }

        col.enabled = false;
        agent.SetDestination(targetTransform[0].position);
        isWalking = true;
        StartCoroutine(EnableCollider(col, 3f));
        AnimHelper.TryPlay(myAnim, "walk", 0);
    }

    void Update()
    {
        if (isWalking)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // 도착 완료
                    npcTransform.gameObject.SetActive(false);
                }
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

        RaycastHit[] hits = Physics.SphereCastAll(rayStart, sphereRadius, rayDir, 0f, layerMask);

        if (hits.Length < 1) { detectObj = null; }

        foreach (RaycastHit hit in hits)
        {
            // Debug.Log("Hit 1 : " + hit.transform.gameObject.name);
            GameObject hitObj = hit.transform.gameObject;

            Vector3 hitDir = (hitObj.transform.position - rayStart).normalized;
            float hitRad = Mathf.Acos(Vector3.Dot(rayDir, hitDir));
            float hitDeg = Mathf.Rad2Deg * hitRad;

            if (hitDeg >= leftDeg && hitDeg <= rightDeg)
            {
                // Debug.Log("Hit 2 : " + hit.transform.gameObject.name);

                if (detectObj != null)
                {
                    float detectObjDist = Vector3.Distance(rayStart, detectObj.transform.position);
                    float hitDist = Vector3.Distance(rayStart, hitObj.transform.position);

                    if (hitDist < detectObjDist)
                    {
                        Debug.Log("change " + hitObj.gameObject.name);
                        detectObj = hitObj.gameObject;
                    }
                }
                else
                {
                    //Debug.Log("new");
                    detectObj = hit.transform.gameObject;
                }
            }
            else
            {
                detectObj = null;
            }
        }
        
        if(detectObj != null )
        {
            if (detectObj.layer == LayerMask.NameToLayer("Player"))
            {
                IsBlockedByPlayer();
            }
            else if(detectObj.layer == LayerMask.NameToLayer("ActiveObject") && detectObj.name.Contains("Door"))
            {
                OpenDoor(detectObj);
            }
        }
        else
        {
            Walk();
        }

    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position; // 살짝 뒤에서 시작
        Vector3 forward = transform.forward;

        float halfFOV = findRange * 0.5f;
        float radius = sphereRadius;

        // 중심선 (빨강)
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, forward * radius);

        // 왼쪽 시야선 (초록)
        Quaternion leftRot = Quaternion.Euler(0, -halfFOV, 0);
        Vector3 leftDir = leftRot * forward;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(origin, leftDir * radius);

        // 오른쪽 시야선 (파랑)
        Quaternion rightRot = Quaternion.Euler(0, halfFOV, 0);
        Vector3 rightDir = rightRot * forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(origin, rightDir * radius);
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

    void OpenDoor(GameObject door)
    {
        inAction = true;
        door.GetComponent<Collider>().enabled = false;
        agent.isStopped = true;
        AnimHelper.TryPlay(myAnim, "OpenDoor", 0f);

        AnimHelper.TryPlay(door.GetComponent<Animator>(), "Opening_2", 0f);

        StartCoroutine(CloseDoor(door));
    }

    IEnumerator CloseDoor(GameObject door)
    {
        yield return new WaitForSeconds(1.6f);
        agent.isStopped = false;
        AnimHelper.TryPlay(myAnim, "walk", 0f);
        yield return new WaitForSeconds(3f);
        AnimHelper.TryPlay(door.GetComponent<Animator>(), "Closing_2", 0f);
        door.GetComponent<Collider>().enabled = true;
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
