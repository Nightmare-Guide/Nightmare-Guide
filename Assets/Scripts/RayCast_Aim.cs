using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.Characters.FirstPerson;
using static CommonUIManager;
using static ProgressManager;
using static SchoolUIManager;
using static UnityEngine.Rendering.DebugUI;

public class RayCast_Aim : MonoBehaviour
{
    public float maxRayDistance = 5f; // 레이 길이 설정
    private OutlineObject previousOutline;
    public GameObject flashlight;
    // public bool getFlashlight = false; // 테스트용

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
            OutlineObject currentOutline = hit.collider.GetComponent<OutlineObject>();

            if (currentOutline != null && !currentOutline.enabled)
            {
                currentOutline.enabled = true;
                Debug.Log("OutlineObject 활성화됨!");
            }

            if (previousOutline != null && previousOutline != currentOutline)
            {
                previousOutline.enabled = false;
                Debug.Log("이전 OutlineObject 비활성화됨!");
            }

            previousOutline = currentOutline;

            // UI 활성화
            if (CommonUIManager.instance != null)
            {
                CommonUIManager.instance.interactionUI.SetActive(true);
            }

            // 상호작용 E 키
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.GetMask("ActiveObject")))
                {
                    GameObject click_object = hit.transform.gameObject;

                    Collider objCollider = click_object.GetComponent<Collider>();
                    objCollider.enabled = false; // 중복 작동 방지

                    // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); // 실제 충돌 지점까지 빨간색

                    // Debug.Log($"Object Name : {click_object.name}");
                    if (StoryCheck(click_object))
                    {
                        click_object.GetComponent<Collider>().enabled = true;
                        return;
                    }

                    if (click_object.CompareTag("NextScene"))
                    {
                        // 플레이어 못 움직이게
                        PlayerController.instance.Close_PlayerController();
                        Camera_Rt.instance.Close_Camera();

                        NextScene next = click_object.GetComponent<NextScene>();
                        objCollider.enabled = true;
                        next.Next_Scene();

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
                        Debug.Log("Door");
                        DoorCheck(click_object);
                    }

                    if (click_object.CompareTag("SpecialDoor"))
                    {
                        Debug.Log("Special Door");
                        objCollider.enabled = true;

                        SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                        if (click_object.name.Contains("Janitor's office"))
                        {
                            if (schoolUIManager.CheckItem(schoolUIManager.items[1].name)) // 열쇠가 있으면 실행
                            {
                                DoorCheck(click_object);
                                click_object.tag = "Door";
                                schoolUIManager.UseItem(schoolUIManager.items[1]);
                                ProgressManager.Instance.CompletedAction(ActionType.EnteredControlRoom);
                            }
                            else if (ProgressManager.Instance.IsActionCompleted(ActionType.EnteredControlRoom))
                            {
                                DoorCheck(click_object);
                            }
                        }
                        else if (click_object.name.Contains("Lounge Door"))
                        {
                            if (ProgressManager.Instance.IsActionCompleted(ActionType.FirstMeetMonster))
                            {
                                Door door = click_object.GetComponent<Door>();
                                DoorCheck(click_object);

                                if (!door.doorState && schoolUIManager.enterLounge)
                                {
                                    schoolUIManager.CloseLoungeDoor();
                                }
                            }
                            else
                            {
                                DoorCheck(click_object);
                            }
                        }


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
                    if (click_object.CompareTag("HintObj"))
                    {
                        HintEvent(click_object);
                    }

                    if (click_object.CompareTag("DropItem"))
                    {
                        if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager)
                        {
                            schoolUIManager.GetItem(click_object);
                        }
                        click_object.SetActive(false);
                    }

                    if (click_object.CompareTag("EthanLocker"))
                    {
                        if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager)
                        {
                            if(schoolUIManager.CheckItem("Locker Key"))
                            {
                                CSVRoad_Story.instance.OnSelectChapter("1_0_4");
                                schoolUIManager.StopPlayerController();
                            }
                            else
                            {
                                //schoolUIManager.FirstMeetEthan(getFlashlight);
                                schoolUIManager.FirstMeetEthan(ProgressManager.Instance.IsActionCompleted(ActionType.GetFlashlight));
                            }
                        }
                    }

                    if (click_object.CompareTag("Flashlight"))
                    {
                        ProgressManager.Instance.CompletedAction(ActionType.GetFlashlight);
                        click_object.SetActive(false);
                        // getFlashlight = true;
                        if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager) { schoolUIManager.flashlightWall.SetActive(false); }
                    }
                }
            }
        }
        else
        {
            if (CommonUIManager.instance != null)
            {
                // UI 비 활성화
                CommonUIManager.instance.interactionUI.SetActive(false);
            }

            if (previousOutline != null)
            {
                previousOutline.enabled = false;
                Debug.Log("레이 미충돌 - OutlineObject 비활성화됨!");
                previousOutline = null;
            }
        }

        // 손전등 F 키
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlight == null)
                return;

            if (ProgressManager.Instance.IsActionCompleted(ActionType.GetFlashlight))
            {
                flashlight.SetActive(!flashlight.activeInHierarchy);
            }

            //if (getFlashlight)
            //{
            //    flashlight.SetActive(!flashlight.activeInHierarchy);
            //}
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

        if (locker)//문이 열리고 플레이어 이동 후 문 닫힘
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
            CommonUIManager.instance.stevenPhone.hasPhone = true;
            ProgressManager.Instance.progressData.phoneDatas[0].hasPhone = true;
            ProgressManager.Instance.progressData.storyProgress = "clear";
            ProgressManager.Instance.progressData.newGame = false;
            // CSVRoad_Story.instance.OpenQuestUI(CSVRoad_Story.instance.GetQuest("0_1_0_1"));
        }
        else
        {
            PhoneInfos targetPhone = obj.GetComponent<CellPhone>().schoolUIManager.phoneInfos
                                            .Find(info => obj.gameObject.name.Contains(info.name));

            targetPhone.hasPhone = true;
            if (targetPhone.name == "Ethan") { ProgressManager.Instance.progressData.phoneDatas[1].hasPhone = true; }
            else if (targetPhone.name == "David") { ProgressManager.Instance.progressData.phoneDatas[2].hasPhone = true; }
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
            return interactable.Interact(obj); // 상호작용 완료
        }
        return false; // 상호작용 없음
    }

    public void HintEvent(GameObject obj)
    {
        OpenHint hint_Event = obj.GetComponent<OpenHint>();
        hint_Event.HintEvent();
    }
}
