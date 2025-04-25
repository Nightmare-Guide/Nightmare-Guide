using JetBrains.Annotations;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static SchoolUIManager;
using static UnityEngine.Rendering.DebugUI;

public class RayCast_Aim : MonoBehaviour
{
    public float maxRayDistance = 5f; // 레이 길이 설정

    [Header("Locker")]
    bool locker = true;


    private void Start()
    {
        // 커서를 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // 커서를 안보이게 하기

    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.GetMask("ActiveObject")))
        {
            // UI 활성화
            CommonUIManager.instance.interactionUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.GetMask("ActiveObject")))
                {
                    GameObject click_object = hit.transform.gameObject;
                   // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); // 실제 충돌 지점까지 빨간색

                   // Debug.Log($"Object Name : {click_object.name}");
                    if (StoryCheck(click_object))
                    {
                        return;
                    }
                    // 태그가 "maze_Btn"이라면 Select_Btn() 호출
                    if (click_object.CompareTag("maze_Btn"))
                    {
                        Chapter1_Maze(click_object);
                    }
                    if (click_object.CompareTag("Locker"))
                    {
                        Locker(click_object);
                    }

                    if (click_object.CompareTag("Door"))
                    {
                     //   Debug.Log("Door");
                        DoorCheck(click_object);
                    }

                    if (click_object.CompareTag("CellPhone"))
                    {
                      //  Debug.Log("CellPhone");
                        TouchCellPhone(click_object);
                    }
                    if (click_object.CompareTag("ElevatorButton"))
                    {
                      //  Debug.Log("ElevatorButton");
                        ElevatorButton(click_object);
                    }
                }
            }
        }
        else
        {
            // UI 비 활성화
            CommonUIManager.instance.interactionUI.SetActive(false);
        }
    }

    public void ElevatorButton(GameObject obj)
    {
        Animator button_anim = obj.GetComponent<Animator>();
        obj.GetComponent<Collider>().enabled = false;

        button_anim.SetTrigger("ClickButton");
        button_anim.SetBool("On", true);
    }

    public void Chapter1_Maze(GameObject obj) //챕터 1 미로맵 탈출용 버튼 클릭
    {
        Maze_Button mazeButton = obj.GetComponent<Maze_Button>();

        if (mazeButton != null)
        {
            mazeButton.Select_Btn(); // 클릭한 오브젝트의 Select_Btn 호출
         //   Debug.Log(obj.name + "색상 변경");
        }
        else
        {
         //   Debug.Log("색상없음");
        }
    }

    public void Locker(GameObject obj)
    {

      //  Debug.Log("락커 인식" + obj.name);
        Locker lockerObj = obj.GetComponent<Locker>();
        if (lockerObj.isMovingToLocker || lockerObj.outMovingToLocker)
        {
            return;
        }

        if (locker)//문이 열리고 플레이어 이동후 문디 닫힘
        {
            lockerObj.isMovingToLocker = true;
            lockerObj.PlayerHide();
            locker = false;


        }
        else if (!locker)//플레이어 컨트롤러가 활성화되고 문이 열림
        {

            lockerObj.OpenLocker();
            // PlayerController.instance.Open_PlayerController();//플레이어 컨트롤 ON
            locker = true;
            // DoorCheck(obj);

        }


    }

    public void DoorCheck(GameObject obj)
    {
        Door door = obj.GetComponent<Door>();
        LockerRoomDoor ldoor = obj.GetComponent<LockerRoomDoor>();

        if (door != null) //일반적인 door 스크립트
        {
            door.Select_Door();
        }
        if (ldoor != null) //LockerRoomDoor 값 확인용
        {
            ldoor.OpenLockerDoor();
        }
    }

    void TouchCellPhone(GameObject obj)
    {
        // 해당 휴대폰 획득 bool 값 변경
        if (obj.name.Contains("Steven"))
        {
            CommonUIManager.instance.phoneInfos.hasPhone = true;
        }
        else
        {
            CharacterPhoneInfo targetPhone = obj.GetComponent<CellPhone>().schoolUIManager.phoneInfos
                                            .Find(info => obj.gameObject.name.Contains(info.name));

            targetPhone.hasPhone = true;
        }

        // CellPhone 위치 변경 함수 실행
        CellPhone cellPhoneLogic = obj.GetComponent<CellPhone>();

        Vector3[] cellPhoneTransform = GetCameraInfo();

        cellPhoneLogic.UpPhone(cellPhoneTransform[0], cellPhoneTransform[1]); // 카메라 앞으로 휴대폰 이동

        // 데이터 입력
        if (!obj.name.Contains("Steven"))
        {
            cellPhoneLogic.schoolUIManager.GetItem(obj);
        }

        //플레이어 컨트롤 OFF
        PlayerController.instance.Close_PlayerController();

        //카메라 회전 정지
        Camera_Rt.instance.Close_Camera();
    }

    public Vector3[] GetCameraInfo()
    {
        Vector3[] arr = new Vector3[2];

        // 카메라 포지션 값
        Vector3 cameraPos = this.GetComponent<Transform>().position;

        // 카메라 회전값
        Vector3 cameraRotate = this.GetComponent<Transform>().eulerAngles;

        // 카메라가 바라보는 방향
        Vector3 cameraForward = transform.forward;

        // 카메라 x 축 회전값에 따라 Y 위치 조정 (최대 ±0.35f 변동)
        float yOffset = Mathf.Sin(cameraRotate.x * Mathf.Deg2Rad) * Mathf.Sign(cameraRotate.x) * -0.35f;

        // 휴대폰 최종 포지션 값
        Vector3 cellPhonePos = cameraPos + new Vector3(cameraForward.x * 0.35f, yOffset, cameraForward.z * 0.35f);

        // 휴대폰 최종 회전 값
        Vector3 cellPhoneRotate = new Vector3(-cameraRotate.x, 180 + cameraRotate.y, 180 + cameraRotate.z);

        arr[0] = cellPhonePos;
        arr[1] = cellPhoneRotate;

        return arr;
    }


    public bool StoryCheck(GameObject obj)
    {
        StoryInteractable interactable = obj.GetComponent<StoryInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
            return true; // 상호작용 완료
        }
        return false; // 상호작용 없음
    }
}
