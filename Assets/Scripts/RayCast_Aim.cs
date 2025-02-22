using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class RayCast_Aim : MonoBehaviour
{
    public float maxRayDistance = 1.1f; // ���� ���� ����

    [Header("Locker")]
    int locker = 0;
    private void Start()
    {
        // Ŀ���� ȭ�� �߾ӿ� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // Ŀ���� �Ⱥ��̰� �ϱ�

    }
    private void Update()
    {
       
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ��
        {
         
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                GameObject click_object = hit.transform.gameObject;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); // ���� �浹 �������� ������

                // �±װ� "maze_Btn"�̶�� Select_Btn() ȣ��
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

    public void Chapter1_Maze(GameObject obj) //é�� 1 �̷θ� Ż��� ��ư Ŭ��
    {
        Maze_Button mazeButton = obj.GetComponent<Maze_Button>();

        if (mazeButton != null)
        {
            mazeButton.Select_Btn(); // Ŭ���� ������Ʈ�� Select_Btn ȣ��
            Debug.Log(obj.name + "���� ����");
        }
    }
    public void Locker(GameObject obj)
    {
        Debug.Log("��Ŀ �ν�"+obj.name);
        Locker lockerObj = obj.GetComponent<Locker>();
        if (locker ==0)//���� ������ �÷��̾� �̵��� ���� ����
        {
            PlayerController.instance.Close_PlayerController();//�÷��̾� ��Ʈ�� OFF
            Camera_Rt.instance.Close_Camera();//ī�޶� ȸ�� ���
            lockerObj.lockPr = true;
            lockerObj.PlayerHide();  
            locker = 1;
            

        }
        else if (locker==1)//�÷��̾� ��Ʈ�ѷ��� Ȱ��ȭ�ǰ� ���� ����
        {
            Camera_Rt.instance.Close_Camera();
            lockerObj.OpenLocker();
           // PlayerController.instance.Open_PlayerController();//�÷��̾� ��Ʈ�� ON
            locker = 0;
           // DoorCheck(obj);
        }else if (locker == 2) //���� ����
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

        //�÷��̾� ��Ʈ�� OFF
        PlayerController.instance.Close_PlayerController();
        //ī�޶� ȸ�� ����
        Camera_Rt.instance.Close_Camera();
        //���콺 Ŀ�� Ȱ��ȭ
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;  // Ŀ���� �Ⱥ��̰� �ϱ�
    }
}
