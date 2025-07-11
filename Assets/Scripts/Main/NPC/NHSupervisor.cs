using UnityEngine;
using System.Collections; // �ڷ�ƾ ����� ���� ���ӽ����̽�
using UnityStandardAssets.Characters.FirstPerson; // PlayerController ������ ���� ���ӽ����̽�

public class NHSupervisor : MonoBehaviour
{
    // �̱��� �ν��Ͻ�: �ٸ� ��ũ��Ʈ���� �� ��ũ��Ʈ�� ���� ������ �� �ֵ��� �մϴ�.
    public static NHSupervisor instance;

    [Header("Door References")]
    [SerializeField] private GameObject roomDoor; // ù ��° �� ������Ʈ (NHDoor()�� ����)
    // roomDoor2�� �� �̻� ������ �����Ƿ� ���ŵ˴ϴ�.

    private Animator anim; // NHSupervisor�� �ִϸ����� ������Ʈ
    private Transform playerTr; // �÷��̾��� Transform ������Ʈ (�÷��̾� ���� ��ɿ��� ���)

    /// <summary>
    /// ��ũ��Ʈ �ʱ�ȭ �� ȣ��˴ϴ�.
    /// �̱��� �ν��Ͻ� ���� �� ������Ʈ ���� ȹ���� �����մϴ�.
    /// </summary>
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

    /// <summary>
    /// �� ��ũ��Ʈ ���������� Update() �޼��忡�� ���������� ó���� ������ �����ϴ�.
    /// </summary>
    void Update()
    {
        // ���� ���������� Update���� ó���� ������ �����ϴ�.
    }

    /// <summary>
    /// �ٸ� �ݶ��̴��� NHSupervisor�� Ʈ���� �ݶ��̴��� �������� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="other">Ʈ���ſ� ������ �ٸ� �ݶ��̴�</param>
    private void OnTriggerEnter(Collider other)
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
    }
    public void EndStory()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.Open_PlayerController();
        }
    }
    /// <summary>
    /// NHSupervisor�� �������� �����մϴ�.
    /// ȣ��Ǹ� ���̵� �ִϸ��̼����� ����ǰ� 2�� �� NHDoor�� �����մϴ�.
    /// </summary>
    public void StartMovement()
    {
        Debug.Log("NHSupervisor: StartMovement ȣ���. ���̵� ���·� ���� �� 2�� �� �� ����.");
        anim?.SetBool("isWalk", false); // Ȥ�� �ȱ� �ִϸ��̼��� ��� ���̾��ٸ� ����ϴ�.
        anim?.SetTrigger("isIdle"); // ���̵� �ִϸ��̼����� ��ȯ�մϴ�.

        StartCoroutine(OpenDoorAfterDelay(2f)); // 2�� �� ���� ���� �ڷ�ƾ ����
    }

    /// <summary>
    /// ������ ������ �� NHDoor() �޼��带 ȣ���մϴ�.
    /// </summary>
    /// <param name="delay">������ �ð� (��)</param>
    private IEnumerator OpenDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NHDoor(); // ������ �� �� ����
    }

    /// <summary>
    /// ù ��° ��(roomDoor)�� ���� ���� �޼����Դϴ�.
    /// Ÿ�Ӷ��� �ñ׳� �Ǵ� ��ũ��Ʈ ���ο��� ȣ��� �� �ֽ��ϴ�.
    /// </summary>
    public void NHDoor()
    {
        Debug.Log("NHSupervisor: NHDoor() ȣ��� - ù ��° ���� ���ϴ�.");
        roomDoor.GetComponent<Door>()?.Select_Door(); // Door ������Ʈ�� �ִ��� Ȯ�� �� ȣ��
    }
}