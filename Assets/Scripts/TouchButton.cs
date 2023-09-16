using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TouchButton : XRBaseInteractable
{
    [SerializeField]
    private string _number;
    [SerializeField]
    private Material _normalMaterial;
    [SerializeField]
    private Material _hoveredMaterial;
    [SerializeField]
    private NumberPad _numberPad;

    private Renderer _renderer;
    private bool _isThisPressed;

    private static bool _isPressed = false;

    public void Start() => _renderer = GetComponent<Renderer>();

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(HoverStarted);
        hoverExited.AddListener(HoverEnded);
    }

    protected override void OnDisable()
    {
        hoverEntered.RemoveListener(HoverStarted);
        hoverExited.RemoveListener(HoverEnded);
        base.OnDisable();
    }

    private void HoverStarted(HoverEnterEventArgs args)
    {
        if (!_isPressed)
        {
            _isPressed = true;
            _isThisPressed = true;
            _renderer.material = _hoveredMaterial;
            _numberPad.AddDigit(_number);
        }
    }

    private void HoverEnded(HoverExitEventArgs args)
    {
        if (_isThisPressed)
        {
            _renderer.material = _normalMaterial;
            _isPressed = false;
            _isThisPressed = false;
        }
    }
}
