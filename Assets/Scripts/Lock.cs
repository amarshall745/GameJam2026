using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string onTrigger = "opening";

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void unLock()
    {

        animator.SetTrigger(onTrigger);
    }
}
