using UnityEngine;
using System.Collections; // �ڷ�ƾ ����� ���� ���ӽ����̽�
using UnityStandardAssets.Characters.FirstPerson; // PlayerController ������ ���� ���ӽ����̽�

public class NHSupervisor : MonoBehaviour
{

    public static NHSupervisor instance;

    [Header("Door References")]
    [SerializeField] private GameObject roomDoor; // ù ��° �� ������Ʈ (NHDoor()�� ����)
    // roomDoor2�� �� �̻� ������ �����Ƿ� ���ŵ˴ϴ�.

    private Animator anim; // NHSupervisor�� �ִϸ����� ������Ʈ
    private Transform playerTr; // �÷��̾��� Transform ������Ʈ (�÷��̾� ���� ��ɿ��� ���)

    bool talkPl = false;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� �ı�
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator ������Ʈ�� NHSupervisor ������Ʈ�� �����ϴ�.");
        }

        // PlayerController �ν��Ͻ��� �����ϰ� Transform�� ������ �� �ִ��� Ȯ���մϴ�.
        if (PlayerController.instance != null)
        {
            playerTr = PlayerController.instance.transform;
        }
        else
        {
            Debug.LogError("PlayerController �ν��Ͻ��� ã�� �� �����ϴ�. �÷��̾� ���� ����� �۵����� ���� �� �ֽ��ϴ�.");
        }
    }


    void Update()
    {
      
    }


    /*private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �������� ��
        if (other.CompareTag("Player"))
        {
            Debug.Log("NHSupervisor: �÷��̾ ������. �÷��̾� ���� �� ��ũ �ִϸ��̼� ���.");

            // �÷��̾� ����
            PlayerController.instance?.Close_PlayerController(); // PlayerController�� �ִ��� Ȯ�� �� ȣ��

            // NHSupervisor ��ũ �ִϸ��̼� ���
            anim?.SetBool("isWalk", false); // Ȥ�� �ȱ� �ִϸ��̼��� ��� ���̾��ٸ� ����ϴ�.
            anim?.SetTrigger("isTalk"); // 'isTalk' Ʈ���Ÿ� ����Ͽ� ��ũ �ִϸ��̼����� ��ȯ�մϴ�.
            if(CSVRoad_Story.instance != null)
            {
                CSVRoad_Story.instance.OnSelectChapter("2_1_0"); // ���丮 ȣ�� 
            }
            Invoke("EndStory", 22f);
            // �� Ʈ���� �̺�Ʈ�� �� ���� �߻��ϵ��� NHSupervisor �ݶ��̴��� isTrigger�� ��Ȱ��ȭ�մϴ�.
            // (�ʿ信 ���� �� ������ �����ϰų� ������ �� �ֽ��ϴ�.)
            Collider thisCollider = GetComponent<Collider>();
            if (thisCollider != null && thisCollider.isTrigger)
            {
                thisCollider.isTrigger = false;
                Debug.Log("NHSupervisor �ݶ��̴��� isTrigger�� false�� �����Ͽ� ������ ����");
            }
        }
    }*/

    public void TalkToPlayer()
    {

        if (talkPl)
        {
            return;
        }

        Debug.Log("NHSupervisor: �÷��̾ ������. �÷��̾� ���� �� ��ũ �ִϸ��̼� ���.");
        if (roomDoor.GetComponent<Door>().doorState == true)
        {
            NHDoor();
        }
      
        // �÷��̾� ����
        PlayerController.instance?.Close_PlayerController(); // PlayerController�� �ִ��� Ȯ�� �� ȣ��

        // NHSupervisor ��ũ �ִϸ��̼� ���
        anim?.SetBool("isWalk", false); // Ȥ�� �ȱ� �ִϸ��̼��� ��� ���̾��ٸ� ����ϴ�.
        anim?.SetTrigger("isTalk"); // 'isTalk' Ʈ���Ÿ� ����Ͽ� ��ũ �ִϸ��̼����� ��ȯ�մϴ�.
        if (CSVRoad_Story.instance != null)
        {
            CSVRoad_Story.instance.OnSelectChapter("2_1_0"); // ���丮 ȣ�� 
        }
        Invoke("EndStory", 22f);
        // �� Ʈ���� �̺�Ʈ�� �� ���� �߻��ϵ��� NHSupervisor �ݶ��̴��� isTrigger�� ��Ȱ��ȭ�մϴ�.
        // (�ʿ信 ���� �� ������ �����ϰų� ������ �� �ֽ��ϴ�.)
        Collider thisCollider = GetComponent<Collider>();
        if (thisCollider != null && thisCollider.isTrigger)
        {
            thisCollider.isTrigger = false;
            Debug.Log("NHSupervisor �ݶ��̴��� isTrigger�� false�� �����Ͽ� ������ ����");
        }
        talkPl = true;
    }

    public void EndStory()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.Open_PlayerController();
        }
    }

    public void StartMovement()
    {
        Debug.Log("NHSupervisor: StartMovement ȣ���. ���̵� ���·� ���� �� 2�� �� �� ����.");
        anim?.SetBool("isWalk", false); // Ȥ�� �ȱ� �ִϸ��̼��� ��� ���̾��ٸ� ����ϴ�.
        anim?.SetTrigger("isIdle"); // ���̵� �ִϸ��̼����� ��ȯ�մϴ�.

        Invoke("NHDoor", 2f);
    }



    public void NHDoor()
    {
        Debug.Log("NHSupervisor: NHDoor() ȣ��� - ù ��° ���� ���ϴ�.");
        roomDoor.GetComponent<Door>()?.Select_Door(); // Door ������Ʈ�� �ִ��� Ȯ�� �� ȣ��
    }
}