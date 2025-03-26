using System;
using UnityEngine;
using TMPro; // TextMeshPro 사용

namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // TextMeshProUGUI로 변경
        public TextMeshProUGUI camSwitchButton;
        public GameObject[] objects;

        private int m_CurrentActiveObject;

        private void OnEnable()
        {
            // 활성화된 오브젝트를 배열의 첫 번째로 설정
            m_CurrentActiveObject = 0;
            UpdateButtonText();
        }

        public void NextCamera()
        {
            // 다음 활성화할 오브젝트를 계산
            int nextActiveObject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

            // 모든 오브젝트를 순회하며 활성화/비활성화 설정
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextActiveObject);
            }

            // 현재 활성화된 오브젝트 인덱스 업데이트
            m_CurrentActiveObject = nextActiveObject;

            // 버튼 텍스트 업데이트
            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            if (camSwitchButton != null)
            {
                camSwitchButton.text = objects[m_CurrentActiveObject].name;
            }
            else
            {
                Debug.LogWarning("camSwitchButton이 설정되지 않았습니다!");
            }
        }
    }
}
