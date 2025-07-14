using UnityEngine;
using UnityEditor;

public class Maze_Button : MonoBehaviour
{
    public static Maze_Button instance;

    public Color selectedColor = Color.white; // 기본 색상
    public Color changedColor; // 변경할 색상
    private Material originalMaterial; // 원래 마테리얼 저장
    [SerializeField] Material changeMaterial; // 선택후 변경될 마테리얼
    public bool isSelect = false; // 색상을 선택했는지 확인
    public int set_Value=0; //발송할 변수
    public float effect_time;
    private void Start()
    {
        if (instance == null) { instance = this; }
    }
    private void OnEnable()
    {
        ApplyColor();
    }

    public void ApplyColor()//색상 적용
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = selectedColor;
        }
    }

    public void Select_Btn()//aim으로 패널 체크 알고리즘
    {
        if (!isSelect)
        {
            this.GetComponent<Renderer>().material = changeMaterial; //마테리얼 변경
            //this.GetComponent<Renderer>().material.color = changedColor; //색상 초기화
            // Emission 색상 변경 (Emission 활성화 필요!)
            this.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", changedColor);
            isSelect = true;
            Maze_Mgr.instance.panel_Check++;
            Maze_Mgr.instance.anw.Add(set_Value);
            Invoke("Btn_Effect", effect_time);
            Maze_Mgr.instance.StartCoroutine("Button_Timer");// 타이머
        }
        else
        {
            
            Maze_Mgr.instance.Btn_Clear();
        }
        
    }

    public void Btn_Effect()//깜빡임
    {

        this.GetComponent<Renderer>().material = originalMaterial;//마테리얼 초기화
        //this.GetComponent<Renderer>().material.color = selectedColor;//색상 초기화

        // Emission 색상 원래대로 복구
        /*this.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        this.GetComponent<Renderer>().material.SetColor("_EmissionColor", selectedColor);*/

        
        ApplyColor();
    }

    public void Clear_Btn()//버튼 초기화
    {

        this.GetComponent<Renderer>().material = originalMaterial;//마테리얼 초기화
        //this.GetComponent<Renderer>().material.color = selectedColor;//색상 초기화

        // Emission 색상 원래대로 복구
        /*this.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        this.GetComponent<Renderer>().material.SetColor("_EmissionColor", selectedColor);*/

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
        // 색상 버튼 목록
        Color[] colors = {
            Color.red, new Color(1.0f, 0.5f, 0.0f), Color.yellow, Color.green,
            Color.blue, new Color(0.5f, 0.0f, 0.5f), Color.black, Color.white, Color.gray
        };

        string[] colorNames = { "RED", "ORANGE", "YELLOW", "GREEN", "BLUE", "PURPLE", "BLACK", "WHITE", "GRAY" };

        // 9개의 색상 버튼 생성
        for (int i = 0; i < colors.Length; i++)
        {
            if (GUILayout.Button(colorNames[i]))
            {
                colorSelector.selectedColor = colors[i]; // 버튼 클릭 시 색상 변경
                SoundManager.instance.ClickButton();
            }
        }

        // 선택된 색상으로 오브젝트 색상 적용
        if (GUILayout.Button("Apply Color"))
        {
            colorSelector.ApplyColor();
        }

        // 기본 인스펙터 렌더링 (옵션: 색상 선택기)
        DrawDefaultInspector();
    }
}
