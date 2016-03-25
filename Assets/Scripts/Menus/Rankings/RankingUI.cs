using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class RankingUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int Index;
    public Text Place, RacerName, RaceTime;
    public Color DefaultColor, HoverColor, DisabledColor, TextColor;
    public bool MouseHover, Disabled;
    [Range(1, 60)]
    public int ImageRefreshRate, DisabledRefreshRate;

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

        Place.color = TextColor;
        RacerName.color = TextColor;
        RaceTime.color = TextColor;

        StartCoroutine(UpdateDisabled());
        StartCoroutine(UpdateImageColor());
    }

    IEnumerator UpdateImageColor()
    {
        var originalRefreshRate = ImageRefreshRate;
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;
        var oldDisabled = !Disabled;

        while (true)
        {
            if (oldDisabled != Disabled)
            {
                _img.enabled = !Disabled;
                oldDisabled = Disabled;
            }
            else if (_oldMouseHover != MouseHover)
            {
                ImageRefreshRate = 60;
                _img.color = Color.Lerp(_img.color, _colorTo, Time.deltaTime * 5f);

                if (Utils.IsColorLike(_img.color, _colorTo))
                {
                    ImageRefreshRate = originalRefreshRate;
                    _oldMouseHover = MouseHover;
                }
            }

            if (oldRefreshRate != ImageRefreshRate)
            {
                oldRefreshRate = ImageRefreshRate;
                refreshRateSec = 1f / ImageRefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    IEnumerator UpdateDisabled()
    {
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        while (true)
        {
            if (!Disabled)
            {
                if (RacerName.text.Length <= 0 || RaceTime.text.Length <= 0)
                {
                    Disabled = true;
                    _colorTo = DisabledColor;
                    Place.enabled = !Disabled;
                    RacerName.enabled = !Disabled;
                    RaceTime.enabled = !Disabled;
                }
            }
            else
            {
                if (RacerName.text.Length > 0 && RaceTime.text.Length > 0)
                {
                    Disabled = false;
                    Place.enabled = !Disabled;
                    RacerName.enabled = !Disabled;
                    RaceTime.enabled = !Disabled;
                }
            }

            if (oldRefreshRate != DisabledRefreshRate)
            {
                oldRefreshRate = DisabledRefreshRate;
                refreshRateSec = 1f / DisabledRefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _oldMouseHover = false;
        MouseHover = true;

        if (!Disabled)
            _colorTo = HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _oldMouseHover = true;
        MouseHover = false;

        if (!Disabled)
            _colorTo = DefaultColor;
    }
}
