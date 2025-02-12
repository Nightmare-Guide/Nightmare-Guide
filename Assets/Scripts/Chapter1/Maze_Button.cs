using UnityEngine;
using UnityEditor;

public class Maze_Button : MonoBehaviour
{
    public Color selectedColor = Color.white; // �⺻ ����
    private Material originalMaterial; // ���� ���׸��� ����
    [SerializeField] Material changeMaterial; // ������ ����� ���׸���
    private bool isSelect = false; // ������ �����ߴ��� Ȯ��


    private void OnEnable()
    {
        
        ApplyColor();
    }

    public void ApplyColor()
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
            this.GetComponent<Renderer>().material = changeMaterial;
            isSelect = true;
        }
        else
        {
            this.GetComponent<Renderer>().material = originalMaterial;
            ApplyColor();
            isSelect = false;
        }
            
    }
    public void Clear_Btn()
    {
       
        this.GetComponent<Renderer>().material = originalMaterial;
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
