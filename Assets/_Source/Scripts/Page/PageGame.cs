using DG.Tweening;

public class PageGame : PanelBase
{
    protected override void Hide()
    {
        _sequence.Append(_canvas.DOFade(0, _delay)).
            Join(_components[0].DOLocalMoveY(300, _delay)).
            Join(_components[1].DOScale(0, _delay)).
            Join(_components[2].DOScale(0, _delay));
    }

    protected override void Show()
    {
        _sequence.SetDelay(_delay).
            Append(_canvas.DOFade(1, _delay)).

            Join(_components[0].DOLocalMoveY(0, _delay).From(300).SetEase(Ease.OutBack)).
            Join(_components[1].DOScale(1, _delay).From(0).SetEase(Ease.OutBack)).
            Join(_components[2].DOScale(1, _delay).From(0).SetEase(Ease.OutBack)).

            OnComplete(OnShowComplated);
    }

    protected override void Start()
    {
        base.Start();

        Game.Action.OnEnter += Enter;
        Game.Action.OnLose += Exit;
        Game.Action.OnRestart += Enter;
    }

    protected override void OnShowComplated()
    {
        base.OnShowComplated();
        Game.Action.SendStart();
    }
}