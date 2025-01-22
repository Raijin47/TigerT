using DG.Tweening;
using UnityEngine;

public class PagePause : PanelBase
{

    protected override void Hide()
    {
        _sequence.Append(_canvas.DOFade(0, _delay));
    }

    protected override void Show()
    {
        _sequence.SetDelay(_delay).
            Append(_canvas.DOFade(1, _delay)).



            OnComplete(OnShowComplated);
    }

    protected override void Start()
    {
        base.Start();

        Game.Action.OnPause += Action_OnPause;
    }

    private void Action_OnPause(bool onPause)
    {
        if (onPause) Enter();
        else Exit();
    }
}