using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CardReader : XRSocketInteractor
{
    [Header("Lock Data")]
    [SerializeField]
    private GameObject _doorLock;
    [SerializeField]
    private DoorHandle _doorHandle;

    [Header("Reader Data")]
    [SerializeField]
    private Renderer _greenLight;
    [SerializeField]
    private Renderer _redLight;

    private bool _isSnapped = false;
    private float _cardEnterPosition = 0;
    private float _startTime;
    private int _emissionColorID;
    private XRDirectInteractor _hand;
    private XRGrabInteractable _card;

    private const float _maxHandDistance = 0.5f;
    private const float _minSwipeTime = 0.4f;
    private const float _maxSwipeTime = 1.1f;
    private const float _validSwipeDistance = -0.3f;
    private const float _lampOffDelay = 2f;

    protected override void Awake()
    {
        base.Awake();
        _emissionColorID = Shader.PropertyToID("_EmissionColor");
        hoverSocketSnapping = false;
    }

    public void LateUpdate()
    {
        if (_isSnapped)
        {
            float handDistance = Vector3.Distance(_hand.transform.position, transform.position);
            if (handDistance < _maxHandDistance)
            {
                attachTransform.position =
                    new Vector3(attachTransform.position.x, _hand.transform.position.y, attachTransform.position.z);
            }
            else
            {
                PrepareReleaseCard();
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(HoverStarted);
        hoverExited.AddListener(HoverFinished);
    }

    protected override void OnDisable()
    {
        hoverEntered.RemoveListener(HoverStarted);
        hoverExited.RemoveListener(HoverFinished);
        base.OnDisable();
    }

    private void HoverStarted(HoverEnterEventArgs args)
    {
        var transform = args.interactableObject.transform;
        if (transform.CompareTag("Keycard"))
        {
            _card = transform.gameObject.GetComponent<XRGrabInteractable>();
            _hand = _card.interactorsSelecting[0] as XRDirectInteractor;
            _startTime = Time.timeSinceLevelLoad;
            TurnLampsOff();
            _cardEnterPosition = transform.position.y;
            StartSocketSnapping(_card);
            _isSnapped = true;
        }
    }

    private void HoverFinished(HoverExitEventArgs args) => PrepareReleaseCard();

    private void PrepareReleaseCard()
    {
        EndSocketSnapping(_card);
        _isSnapped = false;
        ReleaseCard();
    }

    private void ReleaseCard()
    {
        if (IsValidSwipeDistance() && IsSwipeTimeInRange())
        {
            TurnOnLamp(_greenLight, Color.green);
            Destroy(_doorLock);
            _doorHandle.enabled = true;
        }
        else
        {
            TurnOnLamp(_redLight, Color.red);
        }
        _hand = null;
    }

    private bool IsValidSwipeDistance() => _card.gameObject.transform.position.y - _cardEnterPosition <= _validSwipeDistance;

    private bool IsSwipeTimeInRange()
    {
        var swipeTotalTime = Time.timeSinceLevelLoad - _startTime;
        return swipeTotalTime >= _minSwipeTime && swipeTotalTime <= _maxSwipeTime;
    }

    private void TurnOnLamp(Renderer lamp, Color color)
    {
        lamp.material.SetColor(_emissionColorID, color);
        StartCoroutine(TurnLampsOffTimer());
    }

    private IEnumerator TurnLampsOffTimer()
    {
        yield return new WaitForSeconds(_lampOffDelay);
        TurnLampsOff();
    }

    private void TurnLampsOff()
    {
        _greenLight.material.SetColor(_emissionColorID, Color.black);
        _redLight.material.SetColor(_emissionColorID, Color.black);
    }
}
