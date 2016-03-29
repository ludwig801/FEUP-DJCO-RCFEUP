using UnityEngine;
using UnityEngine.UI;

public class UpgradeLevelHandler : MonoBehaviour
{
    private const int MaxLevel = 5;

    public Image[] Levels;
    public Color ActiveColor;
    public int CurrentLevel = 0;

    public void IncrementLevelIfNotMax()
    {
        if (CurrentLevel < MaxLevel)
        {
            Levels[CurrentLevel].color = ActiveColor;
            CurrentLevel++;
        }
    }

    public void SetLevelTo(int levelToSetTo)
    {
        CurrentLevel = levelToSetTo;

        for (int i = 0; i < levelToSetTo; i++)
            Levels[i].color = ActiveColor;
    }
}
