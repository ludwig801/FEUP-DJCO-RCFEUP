using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    private int Speed = 1;
    private int Handling = 2;
    private int Weight = 3; 

    public void UpgradeSpeed()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Speed });
    }

    public void UpgradeHandling()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Handling });
    }

    public void UpgradeWeight()
    {
        UpgradeWriter.Save(new Upgrade() { UpgradeId = Weight });
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
