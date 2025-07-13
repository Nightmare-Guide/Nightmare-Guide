using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityStandardAssets.Characters.FirstPerson;

public class Supervisor : NPC
{
    public Transform hospitalroom;
    public Transform npcwalkPosition;
    public Transform hospitalRoomFront;
    public MainUIManager MainUIManager;


    public float talkDistance = 2.0f; //��ȭ�Ÿ�

    public BoxCollider nightmareEntrance;
    public bool hasStartedPlayerFollow = false;
    //private bool isWalkingToHospitalRoom = false;
    public GameObject[] position;

    private LookTarget look;

    //�ӽ�
    public bool firstmeet = false;

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
    public void FollowSupervisor()
    {
        StartCoroutine(CheckPlayerFollow());
    }

    public void AutoMoveHospitalRoom(Transform tf)
    {
        myAnim.SetBool("isWalk", true);
        agent.SetDestination(tf.position);
    }
    public void StopMoveToTalk()
    {
        myAnim.SetBool("isWalk", false);
        myAnim.SetTrigger("isTalk");
        agent.ResetPath();
    }
    public void SuperVisorWork()
    {
        myAnim.SetBool("isWalk", false);
        myAnim.SetTrigger("isWork");
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

                PlayerController.instance.GoNavposition(PlayerController.instance.playerwalkposition); // ������ ����
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
        if (other.CompareTag("Player") && !firstmeet)
        {
            playerTransform = other.transform;
            FirstMeet();
            PlayerController.instance.Close_PlayerController();
            //StartWalkToPlayer(playerTransform);
        }
    }
    public void EnableCollider()
    {
        col.enabled = true;
    }
    public void DisableCollider()
    {
        col.enabled = false;
    }

    public void FirstMeet()
    {
        //�ӽ�
        firstmeet = true;
        MainUIManager.DayHospitalTimeLine();
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
