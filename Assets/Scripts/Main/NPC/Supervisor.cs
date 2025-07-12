using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityStandardAssets.Characters.FirstPerson;

public class Supervisor : NPC
{
    public Transform hospitalroom;
    public Transform npcwalkPosition;

    public float talkDistance = 2.0f; //��ȭ�Ÿ�

    public BoxCollider nightmareEntrance;
    public bool hasStartedPlayerFollow = false;
    //private bool isWalkingToHospitalRoom = false;
    public GameObject[] position;

    private LookTarget look;

    //�ӽ�
    public PlayableDirector director;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = talkDistance;
        agent.updateRotation = true;
    }
    private void Awake()
    {
        look = GetComponent<LookTarget>();
        if (look != null)
        {
            look.target = PlayerController.instance.transform;
        }
    }

    private void Update()
    {

    }
    public void AutoMove()
    {
        myAnim.SetBool("isWalk", true);
        agent.SetDestination(hospitalroom.position);
        StartCoroutine(CheckPlayerFollow());
    }
    private IEnumerator CheckPlayerFollow()
    {
        yield return new WaitForSeconds(1f); // �ణ�� ������

        while (!hasStartedPlayerFollow)
        {
            float followDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

            if (followDistance > 8f)
            {
                hasStartedPlayerFollow = true;
                Debug.Log("���ΰ� �̵� ����");

                PlayerController.instance.GoNavposition(); // ������ ����
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void WalkToHospitalRoom()
    {
        myAnim.SetBool("isWalk", true);
        agent.SetDestination(npcwalkPosition.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            FirstMeet();
            PlayerController.instance.Close_PlayerController();
            //StartWalkToPlayer(playerTransform);
        }
    }

    public void FirstMeet()
    {
        //�ӽ�
        director.Play();
    }

    //public void GoHospitalRoom() //Supervisor�� ���Ƿ�
    //{
    //    agent.SetDestination(npcwalkPosition.position);
    //    story = "0_3_1";
    //    CSVRoad_Story.instance.OnSelectChapter(story, this);
    //    myAnim.SetBool("isWalk", true);
    //}
    //public void StartSelectBox()
    //{
    //    LookAtPlayer();
    //    story = "0_3_2";
    //    CSVRoad_Story.instance.OnSelectChapter(story, this);
    //}
    //public void InHospitalRoom()
    //{
    //    agent.SetDestination(hospitalroom.position);
    //    PlayerController.instance.Open_PlayerController();
    //    PlayerController.instance.agent.enabled = false;
    //}
    //public void WalktoIdle() 
    //{
    //    myAnim.SetBool("isWalk", false);
    //    myAnim.SetTrigger("isIdle");
    //}
    //public void GoNightmare()
    //{
    //    nightmareEntrance.enabled = true;
    //}

    //public void GotoPosition()
    //{
    //    agent.SetDestination(position[0].transform.position);
    //}
}
