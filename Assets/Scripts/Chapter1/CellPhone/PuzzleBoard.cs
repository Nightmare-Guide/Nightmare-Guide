using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using static SchoolUIManager;
using static UnityEditor.PlayerSettings;

public class PuzzleBoard : MonoBehaviour
{
    public CellPhone cellPhone;
    public List<PuzzleTile> tileList;

    public List<float> neighborTileDistance; // 바로 옆 타일과의 거리
    public Vector3 emptyTilePosition; // 빈 공간의 포지션 값
    public bool canMovePuzzle = false; // 퍼즐을 사용가능한 상태인지 확인하는 변수
    public bool isTileMoving = false; // 타일 이동 중인지 확인하는 변수
    public GameObject suffleButton;

    private void Awake()
    {
        InitializationPuzzle();
    }

    private void Update()
    {
        // 테스트용 바로 정답 맞추기
        if(Input.GetKey(KeyCode.Keypad8)) 
        {

            suffleButton.SetActive(false);
            // 휴대폰 시간, 날짜, 슬라이드바 는 비활성화
            cellPhone.sliderUI.SetActive(false);
            cellPhone.timeText.gameObject.SetActive(false);
            cellPhone.dateText.gameObject.SetActive(false);

            foreach (PuzzleTile puzzle in tileList)
            {
                puzzle.GetComponent<RectTransform>().anchoredPosition = puzzle.corretPos;
            }
            tileList.ForEach(tile => tile.isCorrected = true);

            UnLockedPhone();
        }
    }

    // 퍼즐 초기화 함수
    public void InitializationPuzzle()
    {
        emptyTilePosition = tileList[tileList.Count - 1].GetComponent<RectTransform>().anchoredPosition;

        // 타일 정답 확인용 bool 값 초기화
        tileList.ForEach(tile => tile.isCorrected = false);
        tileList[^1].isCorrected = true; // 마지막 index 는 시작부터 true 값
        tileList[^1].gameObject.SetActive(false); // 활성화 상태일 경우 섞는 과정에서 오류 발생.

        foreach (PuzzleTile puzzle in tileList)
        {
            puzzle.GetComponent<RectTransform>().anchoredPosition = puzzle.corretPos;
        }

        canMovePuzzle = false;
        isTileMoving = false;
        suffleButton.SetActive(true);
    }

    public void IsMoveTile(PuzzleTile tile)
    {
        if (!canMovePuzzle)
            return;

        isTileMoving = true; // 퍼즐 동시 이동 방지

        // 빈 자리와의 거리
        float dist = Vector3.Distance(emptyTilePosition, tile.GetComponent<RectTransform>().anchoredPosition);
        dist = Mathf.Round(dist * 1000f) / 1000f; // 소수점 3째 자리에서 반올림

        if (neighborTileDistance.Contains(dist))
        {
            Vector3 goalPosition = emptyTilePosition;

            emptyTilePosition = tile.GetComponent<RectTransform>().anchoredPosition;

            tile.OnMoveTo(goalPosition);
        }

    }

    private IEnumerator OnSuffle()
    {
        float current = 0;
        float percent = 0;
        float time = 1.5f;

        // 타일이 이동 중이 아니면 계속 진행
        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            for (int i = Random.Range(0, tileList.Count - 1); i < tileList.Count - 1; i++)
            {
                float dist = Vector3.Distance(emptyTilePosition, tileList[i].GetComponent<RectTransform>().anchoredPosition);

                if (cellPhone.schoolUIManager.ApproximatelyEqual(dist, neighborTileDistance[0]) || cellPhone.schoolUIManager.ApproximatelyEqual(dist, neighborTileDistance[1]))
                {
                    isTileMoving = true;

                    Vector2 goalPosition = emptyTilePosition;

                    emptyTilePosition = tileList[i].GetComponent<RectTransform>().anchoredPosition;

                    if (cellPhone.gameObject.name.Contains("Canvas"))
                    {
                        // emptyTilePosition 값 보정
                        float[] possibleX = { 231.75f, 139.05f, 46.35f };
                        float[] possibleY = { -531.3f, -379.5f, -227.7f, -75.9f };

                        float correctedX = emptyTilePosition.x;
                        float correctedY = emptyTilePosition.y;

                        // X축 보정
                        foreach (float x in possibleX)
                        {
                            if (Mathf.Abs(emptyTilePosition.x - x) <= 3f)
                            {
                                correctedX = x;
                                break;
                            }
                        }

                        // Y축 보정
                        foreach (float y in possibleY)
                        {
                            if (Mathf.Abs(emptyTilePosition.y - y) <= 3f)
                            {
                                correctedY = y;
                                break;
                            }
                        }

                        // 최종 보정 위치
                        emptyTilePosition = new Vector2(correctedX, correctedY);
                    }

                    // 타일 이동
                    tileList[i].OnMoveTo(goalPosition);

                    break;
                }
            }
            yield return null;
        }

        // 퍼즐 사용 가능
        canMovePuzzle = true;
    }

    public void SufflePuzzle() // 버튼에서 사용
    {
        suffleButton.SetActive(false);
        StartCoroutine("OnSuffle");

        // 휴대폰 시간, 날짜, 슬라이드바 는 비활성화
        cellPhone.sliderUI.SetActive(false);
        cellPhone.timeText.gameObject.SetActive(false);
        cellPhone.dateText.gameObject.SetActive(false);
    }

    // 잠금 해제 후 변화
    public void UnLockedPhone()
    {
        canMovePuzzle = false;

        // 해당 휴대폰 잠금해제 여부 bool 값 변경
        CharacterPhoneInfo targetPhone = cellPhone.schoolUIManager.phoneInfos
                                            .Find(info => this.cellPhone.gameObject.name.Contains(info.name));

        targetPhone.isUnlocked = true;

        // 마지막 퍼즐 투명도 조절
        cellPhone.schoolUIManager.SetUIOpacity(cellPhone.puzzleUI[cellPhone.puzzleUI.Length - 1], true, 1f, 0.2f); // 마지막 퍼즐 조각 투명도 조절

        // 퍼즐 UI 천천히 사라짐
        for (int i = 0; i < cellPhone.puzzleUI.Length; i++)
        {
            cellPhone.schoolUIManager.SetUIOpacity(cellPhone.puzzleUI[i], false, 0.5f, 1f);
        }

        // App Screen UI 활성화
        cellPhone.appScreenUI.SetActive(true);

        foreach (Image img in cellPhone.appScreenImgs)
        {
            cellPhone.schoolUIManager.SetUIOpacity(img, true, 0.5f, 1.1f);
        }
        foreach (TextMeshProUGUI text in cellPhone.appScreenTexts)
        {
            cellPhone.schoolUIManager.SetUIOpacity(text, true, 0.5f, 1.1f);
        }
    }
}
