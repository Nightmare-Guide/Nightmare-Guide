using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class Supervisor : NPC
{
    public Transform hospitalroom;
    public Transform npcwalkPosition;
    public NavMeshAgent agent;
    public float talkDistance = 2.0f; //대화거리
    private bool isWalkingToPlayer = false;
    public BoxCollider nightmareEntrance;
    public bool hasStartedPlayerFollow = false;
    //private bool isWalkingToHospitalRoom = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = talkDistance;
        agent.updateRotation = true;
    }

    private void Update()
    {
        if (isWalkingToPlayer)
        {
            PlayerController.instance.Close_PlayerController();
            agent.SetDestination(playerTransform.position);
            myAnim.SetBool("isWalk", true);

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
                myAnim.SetBool("isWalk", false);
                myAnim.SetTrigger("isTalk");

                isWalkingToPlayer = false;

            }
        }
        if (story == "0_3_1" && !hasStartedPlayerFollow)
        {
            float followDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            if (followDistance > 10f)
            {
                hasStartedPlayerFollow = true;
                PlayerController.instance.GoNavposition();
            }
        }
    }
    
    public void StartWalkToPlayer(Transform player)
    {
        playerTransform = player;
        isWalkingToPlayer = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            FirstMeet();
        }
    }
    public void FirstMeet()
    {
        story = "0_3_0";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        isWalkingToPlayer = true;
        col.enabled = false;
        PlayerController.instance.agent.enabled = true;
        LookAtPlayer();
    }

    public void GoHospitalRoom() //Supervisor가 병실로
    {
        agent.SetDestination(npcwalkPosition.position);
        story = "0_3_1";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        myAnim.SetBool("isWalk", true);
    }
    public void StartSelectBox()
    {
        LookAtPlayer();
        story = "0_3_2";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
    }
    public void InHospitalRoom()
    {
        agent.SetDestination(hospitalroom.position);
        PlayerController.instance.Open_PlayerController();
        PlayerController.instance.agent.enabled = false;
    }
    public void WalktoIdle() 
    {
        myAnim.SetBool("isWalk", false);
        myAnim.SetTrigger("isIdle");
    }
    public void GoNightmare()
    {
        nightmareEntrance.enabled = true;
    }
}
