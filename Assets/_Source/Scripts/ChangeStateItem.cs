using DG.Tweening;
using System;
using UnityEngine;

public class ChangeStateItem : MonoBehaviour
{
    public event Action OnPickUpItem;

    [SerializeField] private Transform _content;
    [SerializeField] private ParticleSystem _particle;

    private Collider _collider;
    private Sequence _sequence;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        Game.Action.OnEnter += Action_OnEnter;
    }

    private void Action_OnEnter() => transform.position = Game.Locator.Spawner.GetPath();

    private void OnTriggerEnter(Collider other)
    {
        OnPickUpItem?.Invoke();
        ChangePosition();
    }

    private void ChangePosition()
    {
        var pos = _content.position;
        _collider.enabled = false;
        _sequence?.Kill();


        _sequence = DOTween.Sequence();

        _sequence.Append(_content.DOScale(0, .2f).OnComplete(() => 
        {
            transform.position = Game.Locator.Spawner.GetPath();
            _particle.transform.position = pos;
            _particle.Play();
        })).
            Append(_content.DOScale(1, 2f).OnComplete(() => { _collider.enabled = true; }));
    }
}