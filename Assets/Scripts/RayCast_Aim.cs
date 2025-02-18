using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class RayCast_Aim : MonoBehaviour
{
    public float maxRayDistance = 1.1f; // ���� ���� ����

    [Header("Locker")]
    bool locker = true;
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
        if (locker)
        {
            PlayerController.instance.Close_PlayerController();//�÷��̾� ��Ʈ�� OFF
            Locker lockerObj = obj.GetComponent<Locker>();
            lockerObj.PlayerHide();
            locker = false;
        }else if (!locker)
        {
            PlayerController.instance.Open_PlayerController();//�÷��̾� ��Ʈ�� ON
            locker = true;
            DoorCheck(obj);
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
}
