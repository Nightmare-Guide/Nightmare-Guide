using UnityEngine;
using System.Collections; // �ڷ�ƾ ����� ���� ���ӽ����̽�
using UnityStandardAssets.Characters.FirstPerson; // PlayerController ������ ���� ���ӽ����̽�
using static ProgressManager;
public class NHSupervisor : MonoBehaviour
{

    public static NHSupervisor instance;

    [Header("Door References")]
    [SerializeField] private GameObject roomDoor; // ù ��° �� ������Ʈ (NHDoor()�� ����)
    // roomDoor2�� �� �̻� ������ �����Ƿ� ���ŵ˴ϴ�.

    private Animator anim; // NHSupervisor�� �ִϸ����� ������Ʈ
    private Transform playerTr; // �÷��̾��� Transform ������Ʈ (�÷��̾� ���� ��ɿ��� ���)

    bool talkPl = false;

    [SerializeField] GameObject changetTr;//Ÿ�Ӷ��� ������ �̵� ����Ʈ
    [SerializeField] GameObject officeTr;//�������Խ� Ÿ�Ӷ��� ������ �Ϸ� ������ �繫�Ƿ� ��ġ �ʱ�ȭ
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
        if (ProgressManager.Instance != null && ProgressManager.Instance.IsActionCompleted(ActionType.BackToHospital))
        {
            this.transform.position = officeTr.transform.position;
            Debug.Log("Ÿ�Ӷ��� ���൵ ���丮1_1_5");
        }
        /* // PlayerController �ν��Ͻ��� �����ϰ� Transform�� ������ �� �ִ��� Ȯ���մϴ�.
         if (PlayerController.instance != null)
         {
             playerTr = PlayerController.instance.transform;
         }
         else
         {
             Debug.LogError("PlayerController �ν��Ͻ��� ã�� �� �����ϴ�. �÷��̾� ���� ����� �۵����� ���� �� �ֽ��ϴ�.");
         }*/
    }


    void Update()
    {
      
    }

    public void ChangeTr()
    {
        this.transform.position = changetTr.transform.position;
        this.transform.rotation = Quaternion.Euler(0, 180f, 0);
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
        // PlayerController �ν��Ͻ��� �����ϰ� Transform�� ������ �� �ִ��� Ȯ���մϴ�.
        if (PlayerController.instance != null)
        {
            playerTr = PlayerController.instance.transform;
        }
        else
        {
            Debug.LogError("PlayerController �ν��Ͻ��� ã�� �� �����ϴ�. �÷��̾� ���� ����� �۵����� ���� �� �ֽ��ϴ�.");
        }
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