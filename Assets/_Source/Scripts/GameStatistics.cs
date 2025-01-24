using TMPro;
using UnityEngine;

public class GameStatistics : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public int PigKilled { get; set; }
    public int OrcKilled { get; set; }

    private int _collected;
    private float _startTime;

    private void Start()
    {
        Game.Action.OnStart += Action_OnStart;
        Game.Action.OnLose += Action_OnLose;
        Game.Locator.Change.OnPickUpItem += Change_OnPickUpItem;
    }

    private void Change_OnPickUpItem() => _collected++;

    private void Action_OnStart()
    {
        _startTime = Time.time;

        PigKilled = 0;
        OrcKilled = 0;
        _collected = 0;
    }

    private void Action_OnLose()
    {
        string time = TextUtility.FormatMinute(Time.time - _startTime);

        _text.text = $"{_collected}\n" +
            $"{OrcKilled}\n" +
            $"{PigKilled}\n" +
            $"{time}";
    }
}