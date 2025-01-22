using UnityEngine;
using UnityEngine.UI;

public class KarmaCounter : MonoBehaviour
{
    private Slider _slider;

    private int _karma;
    public int Karma
    {
        get => _karma;
        set
        {
            _karma = Mathf.Clamp(value, (int)_slider.minValue, (int)_slider.maxValue);
            _slider.value = _karma;
        }
    }

    private void Start()
    {
        _slider = GetComponent<Slider>();

        Game.Action.OnEnter += Action_OnEnter;
        Game.Action.OnRestart += Action_OnEnter;
    }

    private void Action_OnEnter() => Karma = 30;
}