using UnityEngine;

public class RayCast_Aim : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject click_object = hit.transform.gameObject;

                // �±װ� "maze_Btn"�̶�� Select_Btn() ȣ��
                if (click_object.CompareTag("maze_Btn"))
                {
                    Chapter1_Maze(click_object);
                }

                if (click_object.CompareTag("Door"))
                {
                    Debug.Log("Door");
                    DoorCheck(click_object);
                }

            }
        }
    }

    public void Chapter1_Maze(GameObject obj)
    {
        Maze_Button mazeButton = obj.GetComponent<Maze_Button>();

        if (mazeButton != null)
        {
            mazeButton.Select_Btn(); // Ŭ���� ������Ʈ�� Select_Btn ȣ��
            Debug.Log(obj.name + "���� ����");
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
