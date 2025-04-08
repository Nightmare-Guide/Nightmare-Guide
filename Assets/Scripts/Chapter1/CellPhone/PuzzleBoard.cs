using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using static SchoolUIManager;

public class PuzzleBoard : MonoBehaviour
{
    public CellPhone cellPhone;
    public List<PuzzleTile> tileList;

    public List<float> neighborTileDistance; // �ٷ� �� Ÿ�ϰ��� �Ÿ�
    public Vector3 emptyTilePosition; // �� ������ ������ ��
    public bool canMovePuzzle = false; // ������ ��밡���� �������� Ȯ���ϴ� ����
    public bool isTileMoving = false; // Ÿ�� �̵� ������ Ȯ���ϴ� ����
    public GameObject suffleButton;

    private void Awake()
    {
        InitializationPuzzle();
    }

    private void Update()
    {
        // �׽�Ʈ�� �ٷ� ���� ���߱�
        if(Input.GetKey(KeyCode.Keypad8)) 
        {

            suffleButton.SetActive(false);
            // �޴��� �ð�, ��¥, �����̵�� �� ��Ȱ��ȭ
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

    // ���� �ʱ�ȭ �Լ�
    public void InitializationPuzzle()
    {
        emptyTilePosition = tileList[tileList.Count - 1].GetComponent<RectTransform>().anchoredPosition;

        // Ÿ�� ���� Ȯ�ο� bool �� �ʱ�ȭ
        tileList.ForEach(tile => tile.isCorrected = false);
        tileList[^1].isCorrected = true; // ������ index �� ���ۺ��� true ��
        tileList[^1].gameObject.SetActive(false); // Ȱ��ȭ ������ ��� ���� �������� ���� �߻�.

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

        Debug.Log("1");
        isTileMoving = true; // ���� ���� �̵� ����

        // �� �ڸ����� �Ÿ�
        float dist = Vector3.Distance(emptyTilePosition, tile.GetComponent<RectTransform>().anchoredPosition);
        dist = Mathf.Round(dist * 1000f) / 1000f; // �Ҽ��� 3° �ڸ����� �ݿø�
        Debug.Log($"dist : {dist}");

        if (neighborTileDistance.Contains(dist))
        {
            Debug.Log("2");
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

        // Ÿ���� �̵� ���� �ƴϸ� ��� ����
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

                    tileList[i].OnMoveTo(goalPosition);

                    break;
                }
            }
            yield return null;
        }

        // ���� ��� ����
        canMovePuzzle = true;
    }

    public void SufflePuzzle() // ��ư���� ���
    {
        suffleButton.SetActive(false);
        StartCoroutine("OnSuffle");

        // �޴��� �ð�, ��¥, �����̵�� �� ��Ȱ��ȭ
        cellPhone.sliderUI.SetActive(false);
        cellPhone.timeText.gameObject.SetActive(false);
        cellPhone.dateText.gameObject.SetActive(false);
    }

    // ��� ���� �� ��ȭ
    public void UnLockedPhone()
    {
        canMovePuzzle = false;

        // �ش� �޴��� ������� ���� bool �� ����
        CharacterPhoneInfo targetPhone = SchoolUIManager.instance.phoneInfos
                                            .Find(info => this.cellPhone.gameObject.name.Contains(info.name));

        targetPhone.isUnlocked = true;

        // ������ ���� ���� ����
        SchoolUIManager.instance.SetUIOpacity(cellPhone.puzzleUI[cellPhone.puzzleUI.Length - 1], true, 1f, 0.2f); // ������ ���� ���� ���� ����

        // ���� UI õõ�� �����
        for (int i = 0; i < cellPhone.puzzleUI.Length; i++)
        {
            SchoolUIManager.instance.SetUIOpacity(cellPhone.puzzleUI[i], false, 0.5f, 1f);
        }

        // App Screen UI Ȱ��ȭ
        cellPhone.appScreenUI.SetActive(true);

        foreach (Image img in cellPhone.appScreenImgs)
        {
            SchoolUIManager.instance.SetUIOpacity(img, true, 0.5f, 1.1f);
        }
        foreach (TextMeshProUGUI text in cellPhone.appScreenTexts)
        {
            SchoolUIManager.instance.SetUIOpacity(text, true, 0.5f, 1.1f);
        }
    }
}
