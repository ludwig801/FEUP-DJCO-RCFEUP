using UnityEngine;
using UnityEngine.UI;

public class UpgradeLevelHandler : MonoBehaviour {

    private const int MaxLevel = 5;

    public Image[] Levels;

    public Color ActiveColor;

    private int _currentLevel;

	void Start () {
        _currentLevel = 0;
	}

    public void IncrementLevelIfNotMax()
    {
        if(_currentLevel < MaxLevel)
        {
            Levels[_currentLevel].color = ActiveColor;
            _currentLevel++;
        }
    }

}
