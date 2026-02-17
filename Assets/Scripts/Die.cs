using UnityEngine;
using UnityEngine.SceneManagement;

public class Die : MonoBehaviour
{

    public bool level2;
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        if (level2)
        {
            SceneManager.LoadScene(3);
        }
    }
}
