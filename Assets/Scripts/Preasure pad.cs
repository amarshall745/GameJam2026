using System;
using UnityEngine;

public class Preasurepafe : MonoBehaviour
{
    public Animator animator;
    public string boolParam = "on";

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("weight"))
        {
            animator.SetTrigger("on");
            Debug.Log("Enter");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("weight"))
        {
            animator.SetTrigger("off");
            Debug.Log("Exit");
        }
    }
}
