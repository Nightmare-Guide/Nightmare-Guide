using System;
using UnityEngine;
using TMPro; // TextMeshPro ���

namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // TextMeshProUGUI�� ����
        public TextMeshProUGUI camSwitchButton;
        public GameObject[] objects;

        private int m_CurrentActiveObject;

        private void OnEnable()
        {
            // Ȱ��ȭ�� ������Ʈ�� �迭�� ù ��°�� ����
            m_CurrentActiveObject = 0;
            UpdateButtonText();
        }

        public void NextCamera()
        {
            // ���� Ȱ��ȭ�� ������Ʈ�� ���
            int nextActiveObject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

            // ��� ������Ʈ�� ��ȸ�ϸ� Ȱ��ȭ/��Ȱ��ȭ ����
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextActiveObject);
            }

            // ���� Ȱ��ȭ�� ������Ʈ �ε��� ������Ʈ
            m_CurrentActiveObject = nextActiveObject;

            // ��ư �ؽ�Ʈ ������Ʈ
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
                Debug.LogWarning("camSwitchButton�� �������� �ʾҽ��ϴ�!");
            }
        }
    }
}
