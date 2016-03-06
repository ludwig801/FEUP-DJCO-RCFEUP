using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{

    public void UpgradeSpeed()
    {
        Debug.Log("SPEED+++");
    }

    public void UpgradeHandling()
    {
        Debug.Log("HANDLING+++");
    }

    public void UpgradeWeight()
    {
        Debug.Log("WEIGHT+++");
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
