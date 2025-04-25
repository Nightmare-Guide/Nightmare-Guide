using JetBrains.Annotations;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static SchoolUIManager;
using static UnityEngine.Rendering.DebugUI;

public class RayCast_Aim : MonoBehaviour
{
    public float maxRayDistance = 5f; // ���� ���� ����

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.GetMask("ActiveObject")))
        {
            // UI Ȱ��ȭ
            CommonUIManager.instance.interactionUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.GetMask("ActiveObject")))
                {
                    GameObject click_object = hit.transform.gameObject;
                   // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); // ���� �浹 �������� ������

                   // Debug.Log($"Object Name : {click_object.name}");
                    if (StoryCheck(click_object))
                    {
                        return;
                    }
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
            // UI �� Ȱ��ȭ
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

    public void Chapter1_Maze(GameObject obj) //é�� 1 �̷θ� Ż��� ��ư Ŭ��
    {
        Maze_Button mazeButton = obj.GetComponent<Maze_Button>();

        if (mazeButton != null)
        {
            mazeButton.Select_Btn(); // Ŭ���� ������Ʈ�� Select_Btn ȣ��
         //   Debug.Log(obj.name + "���� ����");
        }
        else
        {
         //   Debug.Log("�������");
        }
    }

    public void Locker(GameObject obj)
    {

      //  Debug.Log("��Ŀ �ν�" + obj.name);
        Locker lockerObj = obj.GetComponent<Locker>();
        if (lockerObj.isMovingToLocker || lockerObj.outMovingToLocker)
        {
            return;
        }

        if (locker)//���� ������ �÷��̾� �̵��� ���� ����
        {
            lockerObj.isMovingToLocker = true;
            lockerObj.PlayerHide();
            locker = false;


        }
        else if (!locker)//�÷��̾� ��Ʈ�ѷ��� Ȱ��ȭ�ǰ� ���� ����
        {

            lockerObj.OpenLocker();
            // PlayerController.instance.Open_PlayerController();//�÷��̾� ��Ʈ�� ON
            locker = true;
            // DoorCheck(obj);

        }


    }

    public void DoorCheck(GameObject obj)
    {
        Door door = obj.GetComponent<Door>();
        LockerRoomDoor ldoor = obj.GetComponent<LockerRoomDoor>();

        if (door != null) //�Ϲ����� door ��ũ��Ʈ
        {
            door.Select_Door();
        }
        if (ldoor != null) //LockerRoomDoor �� Ȯ�ο�
        {
            ldoor.OpenLockerDoor();
        }
    }

    void TouchCellPhone(GameObject obj)
    {
        // �ش� �޴��� ȹ�� bool �� ����
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

        // CellPhone ��ġ ���� �Լ� ����
        CellPhone cellPhoneLogic = obj.GetComponent<CellPhone>();

        Vector3[] cellPhoneTransform = GetCameraInfo();

        cellPhoneLogic.UpPhone(cellPhoneTransform[0], cellPhoneTransform[1]); // ī�޶� ������ �޴��� �̵�

        // ������ �Է�
        if (!obj.name.Contains("Steven"))
        {
            cellPhoneLogic.schoolUIManager.GetItem(obj);
        }

        //�÷��̾� ��Ʈ�� OFF
        PlayerController.instance.Close_PlayerController();

        //ī�޶� ȸ�� ����
        Camera_Rt.instance.Close_Camera();
    }

    public Vector3[] GetCameraInfo()
    {
        Vector3[] arr = new Vector3[2];

        // ī�޶� ������ ��
        Vector3 cameraPos = this.GetComponent<Transform>().position;

        // ī�޶� ȸ����
        Vector3 cameraRotate = this.GetComponent<Transform>().eulerAngles;

        // ī�޶� �ٶ󺸴� ����
        Vector3 cameraForward = transform.forward;

        // ī�޶� x �� ȸ������ ���� Y ��ġ ���� (�ִ� ��0.35f ����)
        float yOffset = Mathf.Sin(cameraRotate.x * Mathf.Deg2Rad) * Mathf.Sign(cameraRotate.x) * -0.35f;

        // �޴��� ���� ������ ��
        Vector3 cellPhonePos = cameraPos + new Vector3(cameraForward.x * 0.35f, yOffset, cameraForward.z * 0.35f);

        // �޴��� ���� ȸ�� ��
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
            return true; // ��ȣ�ۿ� �Ϸ�
        }
        return false; // ��ȣ�ۿ� ����
    }
}
