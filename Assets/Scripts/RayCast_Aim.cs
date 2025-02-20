using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class RayCast_Aim : MonoBehaviour
{
    public float maxRayDistance = 1.1f; // 레이 길이 설정

    [Header("Locker")]
    int locker = 0;
    private void Start()
    {
        // 커서를 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // 커서를 안보이게 하기

    }
    private void Update()
    {
       
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
         
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                GameObject click_object = hit.transform.gameObject;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); // 실제 충돌 지점까지 빨간색

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

                if (click_object.CompareTag("CellPhone"))
                {
                    Debug.Log("CellPhone");
                    TouchCellPhone(click_object);
                }

            }
        }
    }

    public void Chapter1_Maze(GameObject obj) //챕터 1 미로맵 탈출용 버튼 클릭
    {
        Maze_Button mazeButton = obj.GetComponent<Maze_Button>();

        if (mazeButton != null)
        {
            mazeButton.Select_Btn(); // 클릭한 오브젝트의 Select_Btn 호출
            Debug.Log(obj.name + "색상 변경");
        }
    }
    public void Locker(GameObject obj)
    {
        Debug.Log("락커 인식"+obj.name);
        if (locker ==0)//문이 열리고 플레이어 이동후 문디 닫힘
        {
            
            PlayerController.instance.Close_PlayerController();//플레이어 컨트롤 OFF
            Camera_Rt.instance.Close_Camera();//카메라 회전 잠금
            Locker lockerObj = obj.GetComponent<Locker>();
            lockerObj.PlayerHide();
            lockerObj.lockPr = true;
            locker = 1;
            

        }
        else if (locker==1)//플레이어 컨트롤러가 활성화되고 문이 열림
        {
            PlayerController.instance.Open_PlayerController();//플레이어 컨트롤 ON
            locker = 2;
            DoorCheck(obj);
        }else if (locker == 2) //문이 닫힘
        {
            DoorCheck(obj);
            locker = 0;
        }
       
     
    }

    public void DoorCheck(GameObject obj)
    {
        Door door = obj.GetComponent<Door>();
        if(door != null)
        {
            door.Select_Door();
        }
    }

    void TouchCellPhone(GameObject obj)
    {
        obj.GetComponent<CellPhone>().UpPhone();

        //플레이어 컨트롤 OFF
        PlayerController.instance.Close_PlayerController();
    }
}
