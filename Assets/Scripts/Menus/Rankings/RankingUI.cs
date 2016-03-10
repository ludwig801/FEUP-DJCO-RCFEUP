using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class RankingUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int Index;
    public Text Place, RacerName, RaceTime;
    public Color DefaultColor, HoverColor;
    public bool MouseHover;

    [SerializeField]
    private Image _img;
    [SerializeField]
    private bool _oldMouseHover;
    [SerializeField]
    private Color _colorTo;

    void Start()
    {
        _img = GetComponent<Image>();

        name = string.Concat("Ranking ", Index);

        Place.text = string.Concat(Index, Utils.GetOrdinalEnding(Index));
        MouseHover = false;
        _oldMouseHover = !MouseHover;
        _colorTo = DefaultColor;

        StartCoroutine(UpdateImageColor());
    }

    IEnumerator UpdateImageColor()
    {
        while (true)
        {
            yield return null;

            if (_oldMouseHover != MouseHover)
            {
                if (Utils.IsColorLike(_img.color, _colorTo))
                    _oldMouseHover = MouseHover;

                _img.color = Color.Lerp(_img.color, _colorTo, Time.deltaTime * 5f);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _oldMouseHover = false;
        MouseHover = true;
        _colorTo = HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _oldMouseHover = true;
        MouseHover = false;
        _colorTo = DefaultColor;
    }
}
