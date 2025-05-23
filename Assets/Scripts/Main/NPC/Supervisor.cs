using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class Supervisor : NPC
{
    public Transform hospitalroom;
    public Transform workposition;
    public NavMeshAgent agent;
    public float talkDistance = 2.0f; //대화거리
    private bool isWalkingToPlayer = false;
    public NextScene nextScene;
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
    }
    
    public void StartWalkToPlayer(Transform player)
    {
        playerTransform = player;
        isWalkingToPlayer = true;
    }
    public void StartWorkPosition()
    {
        agent.SetDestination(workposition.position);
        myAnim.SetBool("isWalk", true);
    }
    public void StartHospitalRoom() //Supervisor가 병실로
    {
        PlayerController.instance.Open_PlayerController();
        agent.SetDestination(hospitalroom.position);
        myAnim.SetBool("isWalk", true);
    }
    public void WalktoIdle() 
    {
        myAnim.SetBool("isWalk", false);
        myAnim.SetTrigger("isIdle");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            FirstMeet();
            if(story != "0_3_0")
            {
                ReadyChapter1();
            }
        }
    }
    public void FirstMeet()
    {
        story = "0_3_0";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        isWalkingToPlayer = true;
        col.enabled = false;
        LookAtPlayer();
    }
    public void ReadyChapter1()
    {
        story = "0_3_3";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        isWalkingToPlayer = true;
        col.enabled = false;
        LookAtPlayer();
    }

    public void GoNightmare()
    {
        PlayerController.instance.Open_PlayerController();
        nextScene.enabled = true;
    }
}
