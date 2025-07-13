using UnityEngine;
using System.Collections; // 코루틴 사용을 위한 네임스페이스
using UnityStandardAssets.Characters.FirstPerson; // PlayerController 접근을 위한 네임스페이스

public class NHSupervisor : MonoBehaviour
{

    public static NHSupervisor instance;

    [Header("Door References")]
    [SerializeField] private GameObject roomDoor; // 첫 번째 문 오브젝트 (NHDoor()로 제어)
    // roomDoor2는 더 이상 사용되지 않으므로 제거됩니다.

    private Animator anim; // NHSupervisor의 애니메이터 컴포넌트
    private Transform playerTr; // 플레이어의 Transform 컴포넌트 (플레이어 정지 기능에만 사용)

    bool talkPl = false;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스 파괴
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator 컴포넌트가 NHSupervisor 오브젝트에 없습니다.");
        }

        // PlayerController 인스턴스가 존재하고 Transform을 가져올 수 있는지 확인합니다.
        if (PlayerController.instance != null)
        {
            playerTr = PlayerController.instance.transform;
        }
        else
        {
            Debug.LogError("PlayerController 인스턴스를 찾을 수 없습니다. 플레이어 정지 기능이 작동하지 않을 수 있습니다.");
        }
    }


    void Update()
    {
      
    }


    /*private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 접촉했을 때
        if (other.CompareTag("Player"))
        {
            Debug.Log("NHSupervisor: 플레이어가 접촉함. 플레이어 정지 및 토크 애니메이션 재생.");

            // 플레이어 정지
            PlayerController.instance?.Close_PlayerController(); // PlayerController가 있는지 확인 후 호출

            // NHSupervisor 토크 애니메이션 재생
            anim?.SetBool("isWalk", false); // 혹시 걷기 애니메이션이 재생 중이었다면 멈춥니다.
            anim?.SetTrigger("isTalk"); // 'isTalk' 트리거를 사용하여 토크 애니메이션으로 전환합니다.
            if(CSVRoad_Story.instance != null)
            {
                CSVRoad_Story.instance.OnSelectChapter("2_1_0"); // 스토리 호출 
            }
            Invoke("EndStory", 22f);
            // 이 트리거 이벤트가 한 번만 발생하도록 NHSupervisor 콜라이더의 isTrigger를 비활성화합니다.
            // (필요에 따라 이 로직을 유지하거나 제거할 수 있습니다.)
            Collider thisCollider = GetComponent<Collider>();
            if (thisCollider != null && thisCollider.isTrigger)
            {
                thisCollider.isTrigger = false;
                Debug.Log("NHSupervisor 콜라이더의 isTrigger를 false로 설정하여 재진입 방지");
            }
        }
    }*/

    public void TalkToPlayer()
    {

        if (talkPl)
        {
            return;
        }

        Debug.Log("NHSupervisor: 플레이어가 접촉함. 플레이어 정지 및 토크 애니메이션 재생.");
        if (roomDoor.GetComponent<Door>().doorState == true)
        {
            NHDoor();
        }
      
        // 플레이어 정지
        PlayerController.instance?.Close_PlayerController(); // PlayerController가 있는지 확인 후 호출

        // NHSupervisor 토크 애니메이션 재생
        anim?.SetBool("isWalk", false); // 혹시 걷기 애니메이션이 재생 중이었다면 멈춥니다.
        anim?.SetTrigger("isTalk"); // 'isTalk' 트리거를 사용하여 토크 애니메이션으로 전환합니다.
        if (CSVRoad_Story.instance != null)
        {
            CSVRoad_Story.instance.OnSelectChapter("2_1_0"); // 스토리 호출 
        }
        Invoke("EndStory", 22f);
        // 이 트리거 이벤트가 한 번만 발생하도록 NHSupervisor 콜라이더의 isTrigger를 비활성화합니다.
        // (필요에 따라 이 로직을 유지하거나 제거할 수 있습니다.)
        Collider thisCollider = GetComponent<Collider>();
        if (thisCollider != null && thisCollider.isTrigger)
        {
            thisCollider.isTrigger = false;
            Debug.Log("NHSupervisor 콜라이더의 isTrigger를 false로 설정하여 재진입 방지");
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
        Debug.Log("NHSupervisor: StartMovement 호출됨. 아이들 상태로 변경 후 2초 뒤 문 열기.");
        anim?.SetBool("isWalk", false); // 혹시 걷기 애니메이션이 재생 중이었다면 멈춥니다.
        anim?.SetTrigger("isIdle"); // 아이들 애니메이션으로 전환합니다.

        Invoke("NHDoor", 2f);
    }



    public void NHDoor()
    {
        Debug.Log("NHSupervisor: NHDoor() 호출됨 - 첫 번째 문을 엽니다.");
        roomDoor.GetComponent<Door>()?.Select_Door(); // Door 컴포넌트가 있는지 확인 후 호출
    }
}