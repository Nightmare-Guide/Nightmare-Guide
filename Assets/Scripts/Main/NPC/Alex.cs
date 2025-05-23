using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class Alex : NPC
{
    [Header("# ETC")]
    public Transform[] targetTransform; // �Ȱ� ���� ��ǥ ����
    public NavMeshAgent agent;
    bool openingDoor = false;

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
                // ���� �Ϸ�
                Debug.Log("��ǥ ����!");
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

    public void WalkToOutSide()
    {
        agent.isStopped = false;

        AnimHelper.TryPlay(myAnim, "walk", 0f);

        //ī�޶� ȸ�� Ȱ��ȭ
        Camera_Rt.instance.Open_Camera();

        //�÷��̾� ��Ʈ�� On
        PlayerController.instance.Open_PlayerController();
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
