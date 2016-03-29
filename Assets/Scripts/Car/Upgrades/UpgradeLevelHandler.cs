using UnityEngine;
using UnityEngine.UI;

public class UpgradeLevelHandler : MonoBehaviour
{

    private const int MaxLevel = 5;

    public Image[] Levels;

    public Color ActiveColor;

    private int _currentLevel = 0;

    public void IncrementLevelIfNotMax()
    {
        if (_currentLevel < MaxLevel)
        {
            Levels[_currentLevel].color = ActiveColor;
            _currentLevel++;
        }
    }

    public void SetLevelTo(int levelToSetTo)
    {
        _currentLevel = levelToSetTo;

        for (int i = 0; i < levelToSetTo; i++)
            Levels[i].color = ActiveColor;
    }
}
