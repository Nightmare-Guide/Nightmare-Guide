using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CSVRoad_Story : MonoBehaviour
{
    // TextMeshProUGUI ������Ʈ�� �巡�� �� ������� ����
    [SerializeField] private TextMeshProUGUI slip;

    void Start()
    {
        // CSV ���� �б�
        List<Dictionary<string, object>> data = CSVReader.Read("Story");

        // ù ��° ������ ��� (��: Chapter = 0_1, Dialogue_Korean ���)
        if (data.Count > 0)
        {
            Debug.Log(data[0]["Dialogue_Korean"]); // �ֿܼ� ���
            slip.text = data[1]["Dialogue_Korean"].ToString(); // UI�� �ؽ�Ʈ ����
        }
        else
        {
            Debug.LogWarning("CSV �����Ͱ� ��� �ֽ��ϴ�.");
            slip.text = "�����͸� ã�� �� �����ϴ�.";
        }
    }
}
