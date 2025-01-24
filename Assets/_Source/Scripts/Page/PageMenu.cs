using DG.Tweening;
using UnityEngine;

public class PageMenu : PanelBase
{
    [SerializeField] private ButtonBase _buttonStart;

    protected override void Hide()
    {
        _sequence.Append(_canvas.DOFade(0, _delay)).
            Join(_components[0].DOScale(0, _delay).SetEase(Ease.InBack)).
            Join(_components[1].DOScale(0, _delay).SetEase(Ease.InBack)).
            Join(_components[2].DOLocalMoveX(-200, _delay)).
            Join(_components[3].DOLocalMoveX(200, _delay));
    }

    protected override void Show()
    {
        _sequence.SetDelay(_delay).
            Append(_canvas.DOFade(1, _delay)).

            Join(_components[0].DOScale(1, _delay).SetEase(Ease.OutBack)).
            Join(_components[1].DOScale(1, _delay).SetEase(Ease.OutBack)).
            Join(_components[2].DOLocalMoveX(0, _delay)).
            Join(_components[3].DOLocalMoveX(0, _delay)).

        OnComplete(OnShowComplated);
    }

    protected override void Start()
    {
        _canvas.alpha = 1;
        IsActive = true;

        _buttonStart.OnClick.AddListener(Game.Action.SendEnter);
        Game.Action.OnEnter += Exit;
        Game.Action.OnExit += Enter;
    }
}