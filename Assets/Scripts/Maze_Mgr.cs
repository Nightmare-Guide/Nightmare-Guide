using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using UnityStandardAssets.Characters.FirstPerson;
public class Maze_Mgr : MonoBehaviour
{
    public static Maze_Mgr instance;
   
    [Header("Number")]
    // [RED=1, ORANGE=2, YELLOW=3, GREEN=4, BLUE=5, PURPLE=6, BLACK=7, WHITE=8, GRAY=9]
    [SerializeField] GameObject[] num_Obj; //머테리얼을 변경할 오브젝트 
    protected int[] num_Answer = new int[4]; // answerKey
    private int[] num = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    bool anw_Check = false;
    int check_Point = 0;

    [Header("Panel")]//패널 체크
    [SerializeField] GameObject[] btn;//자식 오젝트의 활성화 체크
    public List<int> anw;//답안지
    public int panel_Check= 0;//현재 활성화된 버튼
    bool maze_Clear = false;

    //클리어
    [SerializeField] GameObject door_Obj;

    [Header("Panel_Reset")]//패널 초기화
    //타이머
    //주인공 사망 또는 이탈
    [SerializeField] float timer;
    [SerializeField] float reset_time=180f;
    public Transform resetPoint; // 리셋 좌표
    [SerializeField] GameObject clear_Line; // 번호 초기화 영역
    public bool reStart = false;


    private void Start()
    {
        if (instance == null) { instance = this; }
        Make_Answer();

    }
    private void Update()
    {
        
        if(panel_Check == 4 && !maze_Clear)//답안 체크
        {
            Check_Answer();
            
        }
        if (reStart)//플레이어 위치 초기화 맵 시작점으로
        {
            PlayerController.instance.Close_PlayerController();
            Chapter1_Mgr.instance.player.transform.position = resetPoint.position;
            PlayerController.instance.Open_PlayerController();
            reStart = false;
        }
    }
    private void Make_Answer()
    {
        //AnswerKey 생성
        System.Random rand = new System.Random();
        var randomized = num.OrderBy(item => rand.Next());//배열의 숫자 랜덤 섞기

        foreach (var value in randomized)
        {
            if (check_Point == 4)
            {
                check_Point = 0;
                break;
            }
            num_Answer[check_Point] = value;
     
            check_Point++;
        }
        //정답키 설정후 키에 따른 오브젝트 색상 변경
        Clear_Num();
    }
    public void Clear_Num()
    {
        //맵 초기화 색상 및 정답 변경
        for (int i=0; i < num_Obj.Length; i++)
        {
            Renderer renderer = num_Obj[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                // [RED=1, ORANGE=2, YELLOW=3, GREEN=4, BLUE=5, PURPLE=6, BLACK=7, WHITE=8, GRAY=9]
                int color = num_Answer[i];
                switch (color) {
                    case 1:
                        renderer.material.color = Color.red;
                        break;
                    case 2:
                        renderer.material.color = new Color(1.0f, 0.5f, 0.0f);
                        break;
                    case 3:
                        renderer.material.color = Color.yellow;
                        break;
                    case 4:
                        renderer.material.color = Color.green;
                        break;
                    case 5:
                        renderer.material.color = Color.blue;
                        break;
                    case 6:
                        renderer.material.color = new Color(0.5f, 0.0f, 0.5f);
                        break;
                    case 7:
                        renderer.material.color = Color.black;
                        break;
                    case 8:
                        renderer.material.color = Color.white;
                        break;
                    case 9:
                        renderer.material.color = Color.gray;
                        break;
                }

                    
            }
        }
    }

    public void Check_Answer()//답안 체크
    {
        bool clear = anw.SequenceEqual(num_Answer);
        if (clear)
        {
            Debug.Log("탈출 성공");
            Door door = door_Obj.GetComponent<Door>();
            door.Select_Door();
            maze_Clear = true;
        }
        else
        {
            Btn_Clear();
            Debug.Log("다시 선택해주세요");
        }
        

    }
    public void Btn_Clear() //비밀번호 입력값 초기화
    {
        //버튼 초기화
        anw.Clear(); 
       for(int i=0; i<9; i++)
        {
            btn[i].GetComponent<Maze_Button>().Clear_Btn();
        }
        panel_Check = 0;



    }

    IEnumerator Button_Timer()//버튼을 3분이상 안누를시 비밀번호 입력값 초기화
    {
        timer = 0f; // 타이머 초기화

        while (timer < reset_time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Btn_Clear();
  
     
    }

    

}
