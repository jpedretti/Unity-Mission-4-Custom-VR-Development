using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHandle : XRBaseInteractable
{

    [SerializeField]
    private Rigidbody _doorRigidbody;

    private Transform _doorTransform;
    private XRDirectInteractor _hand;
    private Transform _handTransform;
    private Transform _handleTranform;
    private ActionBasedController _controller;

    private const float Force = 50;
    private const float DoorOpenThreshold = -0.856f;
    private const float DoorClosedThreshold = 0;

    public void Update()
    {
        if (_hand != null)
        {
            CheckPosition();
            Vibrate();
        }
    }

    public void FixedUpdate()
    {
        if (_hand != null)
        {
            ApplyForce();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _handleTranform = transform;
        _doorTransform = _doorRigidbody.transform;
        selectEntered.AddListener(SelectStarted);
        selectExited.AddListener(SelecteEnded);
        hoverEntered.AddListener(HoverStarted);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveAllListeners();
        selectExited.RemoveAllListeners();
        hoverEntered.RemoveAllListeners();
        base.OnDisable();
    }

    private void Vibrate()
    {
        var hapticIntensity = Mathf.Lerp(0, 1, Mathf.Abs(_doorRigidbody.velocity.z * 1.3f));
        _controller.SendHapticImpulse(hapticIntensity, 0.1f);
    }

    private void ApplyForce()
    {
        var direction = _handleTranform.position - _handTransform.position;
        var dot = Vector3.Dot(direction, Vector3.back);
        _doorRigidbody.AddForce(new(0, 0, dot * Force));
    }

    private void CheckPosition()
    {
        if (_doorTransform.localPosition.x < DoorOpenThreshold || _doorTransform.localPosition.x > DoorClosedThreshold)
        {
            _doorRigidbody.velocity = Vector3.zero;
            float clampedX = Mathf.Clamp(_doorTransform.localPosition.x, DoorOpenThreshold, DoorClosedThreshold);
            _doorTransform.localPosition = new Vector3(clampedX, _doorTransform.localPosition.y, _doorTransform.localPosition.z);
        }
    }

    private void SelectStarted(SelectEnterEventArgs args)
    {
        SetHand(args.interactorObject as XRDirectInteractor, args.interactorObject.transform);
        _controller = args.interactorObject.transform.GetComponentInParent<ActionBasedController>();
        _controller.SendHapticImpulse(0.25f, 0.2f);
    }

    private void SelecteEnded(SelectExitEventArgs args)
    {
        SetHand(null, null);
        _controller = null;
    }

    private void HoverStarted(HoverEnterEventArgs args)
    {
        if (_hand == null)
        {
            var controller = args.interactorObject.transform.GetComponentInParent<ActionBasedController>();
            controller.SendHapticImpulse(0.1f, 0.1f);
        }
    }

    private void SetHand(XRDirectInteractor hand, Transform handTransform)
    {
        _hand = hand;
        _handTransform = handTransform;
    }
}
