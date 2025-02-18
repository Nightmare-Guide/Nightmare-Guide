using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Camera_Rt : MonoBehaviour
{
    private Camera m_Camera;
    private MouseLook m_MouseLook = new MouseLook(); // 마우스 컨트롤용 객체

    private void Start()
    {
        m_Camera = Camera.main;
        m_MouseLook.Init(transform, m_Camera.transform); // 마우스 컨트롤 초기화
    }

    private void Update()
    {
        RotateView(); // 마우스 회전 실행
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }





}
