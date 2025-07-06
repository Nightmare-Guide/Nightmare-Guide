using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityStandardAssets.Characters.FirstPerson;

public class TalkTrigger : MonoBehaviour
{
    public GameObject player;
    public Supervisor npc;
    public PlayableDirector timeline;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.Close_PlayerController(); // 플레이어 조작 멈춤
            //npc.StartWalkToPlayer(player.transform); // NPC 이동 시작
            gameObject.SetActive(false); // 트리거 중복 방지
        }
    }
}
