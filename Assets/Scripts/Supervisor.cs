using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class Supervisor : MonoBehaviour
{
    public static Supervisor instance { get; private set; }
    public Transform targetplayer; // player 위치
    public Transform hospitalroom;
    public Animator animator;
    public NavMeshAgent agent;
    public float talkDistance = 2.0f; //대화거리
    private bool isWalkingToPlayer = false;
    //private bool isWalkingToHospitalRoom = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.stoppingDistance = talkDistance;
        agent.updateRotation = true;
    }

    private void Update()
    {
        if (isWalkingToPlayer)
        {
            agent.SetDestination(targetplayer.position);
            animator.SetBool("isWalk", true);

            float distance = Vector3.Distance(transform.position, targetplayer.position);
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
                animator.SetBool("isWalk", false);
                animator.SetTrigger("isTalk");


                isWalkingToPlayer = false;

            }
        }
    }
    
    public void StartWalkToPlayer(Transform player)
    {
        targetplayer = player;
        isWalkingToPlayer = true;
    }

    public void StartHospitalRoom() //플레이어가 병실로
    {
        PlayerController.instance.Open_PlayerController();
        agent.SetDestination(hospitalroom.position);
        animator.SetBool("isWalk", true);
    }
    public void TalktoIdle() 
    {
        animator.SetBool("isWalk", false);
        animator.SetTrigger("isIdle");
    }
}
