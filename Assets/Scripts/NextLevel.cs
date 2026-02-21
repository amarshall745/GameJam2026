using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public bool firstLevel, secondLevel, thirdLevel;
    
    public void changeLevel()
    {
        if (firstLevel)
        {
            SceneManager.LoadScene(4);
        }

        if(secondLevel) { SceneManager.LoadScene(6);}

        if(thirdLevel) { SceneManager.LoadScene(8);}
    }
}
