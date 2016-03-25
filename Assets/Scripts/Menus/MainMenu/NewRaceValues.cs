using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NewRaceValues : MonoBehaviour
{
    public RectTransform NumPlayers, NumLaps;
    public List<ButtonValue> NumPlayersBtns, NumLapsBtns;
    public int Players, Laps;
    public Color ColorSelected, ColorDefault;

    void Start()
    {
        foreach (var btn in NumPlayersBtns)
        {
            btn.Btn.image.color = ColorDefault;
            btn.Btn.onClick.AddListener(() =>
            {
                SelectNumPlayers(btn);
            });
        }

        SelectNumPlayers(NumPlayersBtns[0]);
        SelectNumLaps(NumLapsBtns[0]);
    }

    private void SelectNumPlayers(ButtonValue bv)
    {
        bv.Btn.image.color = ColorSelected;
        foreach (var item in NumPlayersBtns)
        {
            if (item != bv)
                item.Btn.image.color = ColorDefault;
        }
        Players = bv.Value;
    }

    private void SelectNumLaps(ButtonValue bv)
    {
        bv.Btn.image.color = ColorSelected;
        foreach (var item in NumLapsBtns)
        {
            if (item != bv)
                item.Btn.image.color = ColorDefault;
        }
        Laps = bv.Value;
    }

    [System.Serializable]
    public class ButtonValue
    {
        public Button Btn;
        public int Value;
    }
}
