using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Camera_Rt : MonoBehaviour
{
    private Camera m_Camera;
    private MouseLook m_MouseLook = new MouseLook(); // ���콺 ��Ʈ�ѿ� ��ü

    private void Start()
    {
        m_Camera = Camera.main;
        m_MouseLook.Init(transform, m_Camera.transform); // ���콺 ��Ʈ�� �ʱ�ȭ
    }

    private void Update()
    {
        RotateView(); // ���콺 ȸ�� ����
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }





}
