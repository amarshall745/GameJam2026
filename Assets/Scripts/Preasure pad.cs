using System.Collections;
using UnityEngine;

public class Preasurepafe : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string onTrigger = "on";
    [SerializeField] private string offTrigger = "off";

    [Header("Filter")]
    [SerializeField] private string weightTag = "weight";

    [Header("Timing")]
    [Tooltip("Prevents off->on flicker when the object briefly exits then re-enters (e.g. when dropping).")]
    [SerializeField] private float offGraceTime = 0.08f;

    private int weightCount = 0;
    private bool isPressed = false;
    private Coroutine offRoutine;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(weightTag)) return;

        weightCount++;

        // If we were about to turn off, cancel it
        if (offRoutine != null)
        {
            StopCoroutine(offRoutine);
            offRoutine = null;
        }

        // Only fire ON when going from 0 -> 1
        if (!isPressed)
        {
            isPressed = true;
            animator.ResetTrigger(offTrigger);
            animator.SetTrigger(onTrigger);
            Debug.Log("Pad ON");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(weightTag)) return;

        weightCount = Mathf.Max(0, weightCount - 1);

        // Only consider turning OFF when it becomes empty
        if (weightCount == 0 && isPressed)
        {
            if (offRoutine != null) StopCoroutine(offRoutine);
            offRoutine = StartCoroutine(DelayedOff());
        }
    }

    private IEnumerator DelayedOff()
    {
        yield return new WaitForSeconds(offGraceTime);

        // If still empty after the grace time, now turn off
        if (weightCount == 0 && isPressed)
        {
            isPressed = false;
            animator.ResetTrigger(onTrigger);
            animator.SetTrigger(offTrigger);
            Debug.Log("Pad OFF");
        }

        offRoutine = null;
    }
}