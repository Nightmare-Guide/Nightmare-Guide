using UnityEngine;
using UnityEditor;

public class Maze_Button : MonoBehaviour
{
    public static Maze_Button instance;

    public Color selectedColor = Color.white; // �⺻ ����
    public Color changedColor; // ������ ����
    private Material originalMaterial; // ���� ���׸��� ����
    [SerializeField] Material changeMaterial; // ������ ����� ���׸���
    public bool isSelect = false; // ������ �����ߴ��� Ȯ��
    public int set_Value=0; //�߼��� ����
    private void Start()
    {
        if (instance == null) { instance = this; }
    }
    private void OnEnable()
    {
        ApplyColor();
    }

    public void ApplyColor()//���� ����
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = selectedColor;
        }
    }

    public void Select_Btn()//aim���� �г� üũ �˰���
    {
        if (!isSelect)
        {
            //this.GetComponent<Renderer>().material = changeMaterial; //���׸��� ����
            this.GetComponent<Renderer>().material.color = changedColor; //���� �ʱ�ȭ
            isSelect = true;
            Maze_Mgr.instance.panel_Check++;
            Maze_Mgr.instance.anw.Add(set_Value);
        }
        else
        {
            
            Maze_Mgr.instance.Btn_Clear();
        }
        
    }



    public void Clear_Btn()//��ư �ʱ�ȭ
    {

        //this.GetComponent<Renderer>().material = originalMaterial;//���׸��� �ʱ�ȭ
        this.GetComponent<Renderer>().material.color = selectedColor;//���� �ʱ�ȭ
        isSelect = false;
        ApplyColor();
    }

}

[CustomEditor(typeof(Maze_Button))]
public class Maze_ButtonEditor : Editor
{
    private Maze_Button colorSelector;

    private void OnEnable()
    {
        colorSelector = (Maze_Button)target;
    }

    public override void OnInspectorGUI()
    {
        // ���� ��ư ���
        Color[] colors = {
            Color.red, new Color(1.0f, 0.5f, 0.0f), Color.yellow, Color.green,
            Color.blue, new Color(0.5f, 0.0f, 0.5f), Color.black, Color.white, Color.gray
        };

        string[] colorNames = { "RED", "ORANGE", "YELLOW", "GREEN", "BLUE", "PURPLE", "BLACK", "WHITE", "GRAY" };

        // 9���� ���� ��ư ����
        for (int i = 0; i < colors.Length; i++)
        {
            if (GUILayout.Button(colorNames[i]))
            {
                colorSelector.selectedColor = colors[i]; // ��ư Ŭ�� �� ���� ����
            }
        }

        // ���õ� �������� ������Ʈ ���� ����
        if (GUILayout.Button("Apply Color"))
        {
            colorSelector.ApplyColor();
        }

        // �⺻ �ν����� ������ (�ɼ�: ���� ���ñ�)
        DrawDefaultInspector();
    }
}
