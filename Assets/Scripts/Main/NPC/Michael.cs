using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Michael : MonoBehaviour
{
    [SerializeField] string story;
    Collider col;
    public Animator myAnim;
    public Animator broomAnim;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        col.enabled = false;
        StartCoroutine(EnableCollider(col, 2f));
        myAnim.Play("SweepBroom");
        broomAnim.Play("Sweep");
    }

    IEnumerator EnableCollider(Collider col, float time)
    {
        yield return new WaitForSeconds(time);
        col.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CSVRoad_Story.instance.OnSelectChapter(story);
            myAnim.Play("Idle_Broom");
            broomAnim.Play("Idle");
        }
    }
}
