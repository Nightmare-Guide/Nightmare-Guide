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
            PlayerController.instance.Close_PlayerController(); // �÷��̾� ���� ����
            //npc.StartWalkToPlayer(player.transform); // NPC �̵� ����
            gameObject.SetActive(false); // Ʈ���� �ߺ� ����
        }
    }
}
