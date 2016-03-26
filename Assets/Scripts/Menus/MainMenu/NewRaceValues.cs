using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NewRaceValues : MonoBehaviour
{
    public RectTransform NumPlayers, NumLaps;
    public List<ButtonValue> NumPlayersBtns, NumLapsBtns;
    public int Players, Laps;
    public Color ColorSelected, ColorDefault;

    void Start()
    {
        for (int i = 0; i < NumPlayersBtns.Count; i++)
        {
            var itemPlayers = NumPlayersBtns[i];
            var itemLaps = NumLapsBtns[i];
            itemPlayers.Btn.targetGraphic.color = ColorDefault;
            itemLaps.Btn.targetGraphic.color = ColorDefault;
            itemPlayers.Btn.onClick.AddListener(() =>
            {
                SelectNumPlayers(itemPlayers);
            });
            itemLaps.Btn.onClick.AddListener(() =>
            {
                SelectNumLaps(itemLaps);
            });
        }

        SelectNumPlayers(NumPlayersBtns[0]);
        SelectNumLaps(NumLapsBtns[0]);
    }

    private void SelectNumPlayers(ButtonValue bv)
    {
        bv.Btn.targetGraphic.color = ColorSelected;
        foreach (var item in NumPlayersBtns)
        {
            if (item != bv)
                item.Btn.targetGraphic.color = ColorDefault;
        }
        Players = bv.Value;
    }

    private void SelectNumLaps(ButtonValue bv)
    {
        bv.Btn.targetGraphic.color = ColorSelected;
        foreach (var item in NumLapsBtns)
        {
            if (item != bv)
                item.Btn.targetGraphic.color = ColorDefault;
        }
        Laps = bv.Value;
    }

    public void OnRacePressed()
    {
        RaceWriter.UpdateKeyValue(RaceReader.Filename, "numPlayers", Players.ToString());
        RaceWriter.UpdateKeyValue(RaceReader.Filename, "numLaps", Laps.ToString());

        SceneManager.LoadScene("GameScene");
    }

    [System.Serializable]
    public class ButtonValue
    {
        public Button Btn;
        public int Value;
    }
}
