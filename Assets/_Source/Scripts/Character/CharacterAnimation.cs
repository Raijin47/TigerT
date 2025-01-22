using UnityEngine;

public class CharacterAnimation
{
    private readonly Animator Animator;

    private const string _locomotion = "Locomotion";
    private const string _attack = "isAttack";
    private const string _onDie = "isDeath";
    public CharacterAnimation(Animator animator)
    {
        Animator = animator;
        Game.Action.OnLose += Action_OnLose;
        Game.Action.OnRestart += Action_OnStart;
        Game.Action.OnExit += Action_OnStart;
        Game.Action.OnPause += Action_OnPause;
    }

    public void Attack() => Animator.SetTrigger(_attack);
    private void Action_OnPause(bool onPause) => Animator.enabled = !onPause;
    private void Action_OnStart() => Animator.SetBool(_onDie, false);
    private void Action_OnLose() => Animator.SetBool(_onDie, true);
    public void MovementAnimations(float moveAmount)
    {
        Animator.SetFloat(_locomotion, moveAmount, 0.1f, Time.deltaTime);
    }
}