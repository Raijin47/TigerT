using UnityEngine;

public class SkinController : MonoBehaviour
{
    [SerializeField] private Material _material;

    [SerializeField] private ButtonBase _next, _preview;
    [SerializeField] private Color[] _colors;

    private readonly string PropertyName = "_EmissionColor";
    private int _current;

    private void Start()
    {
        _next.OnClick.AddListener(Next);
        _preview.OnClick.AddListener(Preview);
    }

    private void Next()
    {
        _current++;
        if (_current >= _colors.Length) _current = 0;
        Switch();
    }

    private void Preview()
    {
        _current--;
        if (_current < 0) _current = _colors.Length - 1;
        Switch();
    }

    private void Switch() => _material.SetColor(PropertyName, _colors[_current]);
}