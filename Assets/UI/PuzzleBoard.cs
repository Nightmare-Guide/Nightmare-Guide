using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    public List<PuzzleTile> tileList;

    public List<float> neighborTileDistance; // 바로 옆 타일과의 거리
    public Vector3 emptyTilePosition; // 빈 공간의 포지션 값
    public bool isTileMoving = false; // 타일 이동 중인지 체크하는 변수

    private void Awake()
    {
        emptyTilePosition = tileList[tileList.Count - 1].GetComponent<RectTransform>().anchoredPosition;

        // 타일 정답 확인용 bool 값 초기화
        tileList.ForEach(tile => tile.isCorrected = false);
        tileList[^1].isCorrected = true; // 마지막 index 는 시작부터 true 값
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine("OnSuffle");
        }
    }

    public void IsMoveTile(PuzzleTile tile)
    {
        // 빈 자리와의 거리
        float dist = Vector3.Distance(emptyTilePosition, tile.GetComponent<RectTransform>().anchoredPosition);

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

                if (neighborTileDistance.Contains(dist))
                {
                    // isTileMoving = true;

                    Vector3 goalPosition = emptyTilePosition;

                    emptyTilePosition = tileList[i].GetComponent<RectTransform>().anchoredPosition;

                    tileList[i].OnMoveTo(goalPosition);

                    break;
                }
            }
            yield return null;

        }

    }
}
