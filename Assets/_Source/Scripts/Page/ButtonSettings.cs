using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
            Join(_components[0].DOLocalMoveX(0, _delay)).
            Join(_components[1].DOLocalMoveX(110, _delay)).

            //Join(_components[2].DOScaleX(0, _delay));
            Join(_slider.DOValue(0, _delay));
    }

    protected override void Show()
    {
        _sequence.Append(_canvas.DOFade(1, _delay)).

            Join(_components[0].DOLocalMoveX(110, _delay).From(0).SetEase(Ease.OutBack)).
            Join(_components[1].DOLocalMoveX(220, _delay).From(110).SetEase(Ease.OutBack)).

            //Join(_components[2].DOScaleX(3, _delay).SetEase(Ease.OutBack).From(0)).
            Join(_slider.DOValue(1, _delay)).


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