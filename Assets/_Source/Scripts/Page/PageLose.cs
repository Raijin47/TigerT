using DG.Tweening;

public class PageLose : PanelBase
{
    protected override void Hide()
    {
        _sequence.Append(_canvas.DOFade(0, _delay)).
            Join(_components[0].DOScale(0, _delay).SetEase(Ease.InBack)).
            Join(_components[1].DOScale(0, _delay).SetEase(Ease.InBack)).
            Join(_components[2].DOScale(0, _delay).SetEase(Ease.InBack)).
            Join(_components[3].DOScale(0, _delay).SetEase(Ease.InBack));
    }

    protected override void Show()
    {
        _sequence.SetDelay(_delay).
            Append(_canvas.DOFade(1, _delay)).

            Join(_components[0].DOScale(1, _delay).From(0).SetEase(Ease.OutBack)).
            Join(_components[1].DOScale(1, _delay).From(0).SetEase(Ease.OutBack)).
            Join(_components[2].DOScale(1, _delay).From(0).SetEase(Ease.OutBack)).
            Join(_components[3].DOScale(1, _delay).From(0).SetEase(Ease.OutBack)).

            OnComplete(OnShowComplated);
    }

    protected override void Start()
    {
        base.Start();

        Game.Action.OnLose += Enter;
        Game.Action.OnExit += Exit;
        Game.Action.OnRestart += Exit;
    }
}