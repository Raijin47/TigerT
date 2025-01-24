using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSettings : PanelBase
{
    [SerializeField] private ButtonBase _button;
    [SerializeField] private Slider _slider;
    private bool _isShow;

    protected override void Hide()
    {
        _sequence.Append(_canvas.DOFade(0, _delay)).
            Join(_components[0].DOLocalMoveX(0, _delay).SetEase(Ease.InBack)).
            Join(_components[1].DOLocalMoveX(110, _delay).SetEase(Ease.InBack)).
            Join(_slider.DOValue(50, _delay).SetEase(Ease.InBack));
    }

    protected override void Show()
    {
        _sequence.Append(_canvas.DOFade(1, _delay)).

            Join(_components[0].DOLocalMoveX(110, _delay).SetEase(Ease.OutBack)).
            Join(_components[1].DOLocalMoveX(220, _delay).SetEase(Ease.OutBack)).
            Join(_slider.DOValue(75, _delay).SetEase(Ease.OutBack)).

            OnComplete(OnShowComplated);
    }

    protected override void Start()
    {
        base.Start();
        _button.OnClick.AddListener(Active);
        Game.Action.OnEnter += Action_OnEnter;
    }

    private void Action_OnEnter()
    {
        if (!_isShow) return;
        Exit();
        _isShow = false;
    }

    private void Active()
    {
        _isShow = !_isShow;

        if (_isShow) Enter();
        else Exit();
    }
}