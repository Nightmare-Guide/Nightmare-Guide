using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string story;
    public Collider col;
    public Animator myAnim;

    public IEnumerator EnableCollider(Collider col, float time)
    {
        yield return new WaitForSeconds(time);
        col.enabled = true;
    }
}
