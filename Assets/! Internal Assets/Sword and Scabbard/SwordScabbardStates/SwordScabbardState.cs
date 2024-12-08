using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class SwordScabbardState
{
    protected SwordAndScabbardInteractionSystem _context;

    public SwordScabbardState(SwordAndScabbardInteractionSystem context)
    {
        _context = context;
    }
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();


    protected void SwitchStateToNotHolding(SelectEnterEventArgs arg0)
    {
        _context.SwitchState(_context._bothNotHolded);
    }
    protected void SwitchStateToNotHolding(SelectExitEventArgs arg0)
    {
        _context.SwitchState(_context._bothNotHolded);
    }
    protected void SwitchStateToBothAreHolded(SelectEnterEventArgs arg0)
    {
        _context.SwitchState(_context._bothAreHolded);
    }
    protected void SwitchStateToBothAreHolded(SelectExitEventArgs arg0)
    {
        _context.SwitchState(_context._bothAreHolded);
    }
    protected void SwitchStateToOnlyScabbardHolded(SelectEnterEventArgs arg0)
    {
        _context.SwitchState(_context._onlyS�abbardHolded);
    }
    protected void SwitchStateToOnlyScabbardHolded(SelectExitEventArgs arg0)
    {
        _context.SwitchState(_context._onlyS�abbardHolded);
    }
    protected void SwitchStateToOnlySwordHolded(SelectEnterEventArgs arg0)
    {
        _context.SwitchState(_context._onlySwordHolded);
    }
    protected void SwitchStateToOnlySwordHolded(SelectExitEventArgs arg0)
    {
        _context.SwitchState(_context._onlySwordHolded);
    }

    /*private IEnumerator ReturnToStartPositionSmoothly(XRGrabInteractable grabbable)
    {
        //�� ����� ����� ���� ������ �����������. fillAmount. � �� ����� ���������� ��������
        //grabbable.angularVelocityScale = 0.05f; // ���������� ����������
        grabbable.TryGetComponent<Rigidbody>(out var rb);
        rb.maxLinearVelocity = 0.1f;
        rb.maxAngularVelocity = 0.1f;
        yield return new WaitForSeconds(2f);
        rb.maxLinearVelocity = 35f;
        rb.maxAngularVelocity = 35f;
        //grabbable.angularVelocityScale = 1f; // ������� ���������� ��������
    }*/
}


public class OnlySwordHolded : SwordScabbardState
{
    private Coroutine _returnToStartPositionRoutine;

    public OnlySwordHolded(SwordAndScabbardInteractionSystem context) : base(context)
    {
    }

    public override void EnterState()
    {
        _context._xrGrabInteractable.selectEntered.AddListener(SwitchStateToBothAreHolded);
        _context._sword.XRGrabInteractable.selectExited.AddListener(SwitchStateToNotHolding);

        _context._sword.XRGrabInteractable.trackRotation = false;//�������� ��� �����.
        //_returnToStartPositionRoutine = _context.StartCoroutine(AAA());
    }

    /*public IEnumerator AAA()
    {
        
    }*/

    public override void ExitState()
    {
        _context._xrGrabInteractable.selectEntered.RemoveListener(SwitchStateToBothAreHolded);
        _context._sword.XRGrabInteractable.selectExited.RemoveListener(SwitchStateToNotHolding);

        _context._sword.XRGrabInteractable.trackRotation = true;

        if (_returnToStartPositionRoutine != null)//� ��� ����� ����� null?
        {
            _context.StopCoroutine(_returnToStartPositionRoutine);
            _context._sword.Rigidbody.maxLinearVelocity = 35f;
            _context._sword.Rigidbody.maxAngularVelocity = 35f;
        }
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}

public class OnlySkabbardHolded : SwordScabbardState
{
    private Coroutine _returnToStartPositionRoutine;

    public OnlySkabbardHolded(SwordAndScabbardInteractionSystem context) : base(context)
    {
    }

    public override void EnterState()
    {
        _context._sword.XRGrabInteractable.selectEntered.AddListener(SwitchStateToBothAreHolded);
        _context._xrGrabInteractable.selectExited.AddListener(SwitchStateToNotHolding);

        _returnToStartPositionRoutine = _context.StartCoroutine(SmoothlyReturnToOldPosition());
    }

    private FixedJoint fixedJoint;
    public IEnumerator SmoothlyReturnToOldPosition()
    {
        fixedJoint = _context.gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = _context._sword.Rigidbody;
        //������� ����� � ���� ������
        /*_context._rigidody.maxLinearVelocity = 5f;
        _context._rigidody.maxAngularVelocity = 5f;*/
        /*_context._sword.Rigidbody.maxLinearVelocity = 5f;
        _context._sword.Rigidbody.maxAngularVelocity = 5f;*/
        yield return new WaitForSeconds(1f);

        Component.Destroy(fixedJoint);
        /*_context._sword.Rigidbody.maxLinearVelocity = 35f;
        _context._sword.Rigidbody.maxAngularVelocity = 35f;*/
    }

    public override void ExitState()
    {
        _context._sword.XRGrabInteractable.selectEntered.RemoveListener(SwitchStateToBothAreHolded);
        _context._xrGrabInteractable.selectExited.RemoveListener(SwitchStateToNotHolding);

        if (_returnToStartPositionRoutine != null)//� ��� ����� ����� null?
        {
            _context.StopCoroutine(_returnToStartPositionRoutine);
            if (fixedJoint != null)
                Component.Destroy(fixedJoint);
            _context._rigidody.maxLinearVelocity = 35f;
            _context._rigidody.maxAngularVelocity = 35f;
        }
    }

    public override void UpdateState()
    {

    }
}

public class BothAreHolded : SwordScabbardState
{
    public BothAreHolded(SwordAndScabbardInteractionSystem context) : base(context)
    {
    }

    public override void EnterState()
    {
        _context._sword.XRGrabInteractable.selectExited.AddListener(SwitchStateToOnlyScabbardHolded);
        _context._xrGrabInteractable.selectExited.AddListener(SwitchStateToOnlySwordHolded);

        _context._sword.XRGrabInteractable.trackRotation = false;

    }

    public override void ExitState()
    {
        _context._sword.XRGrabInteractable.selectExited.RemoveListener(SwitchStateToOnlyScabbardHolded);
        _context._xrGrabInteractable.selectExited.RemoveListener(SwitchStateToOnlySwordHolded);

        _context._sword.XRGrabInteractable.trackRotation = true;
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}

public class BothNotHolded : SwordScabbardState
{
    public BothNotHolded(SwordAndScabbardInteractionSystem context) : base(context)
    {
    }

    public override void EnterState()
    {
        _context._sword.XRGrabInteractable.selectEntered.AddListener(SwitchStateToOnlySwordHolded);
        _context._xrGrabInteractable.selectEntered.AddListener(SwitchStateToOnlyScabbardHolded);
    }

    public override void ExitState()
    {
        _context._sword.XRGrabInteractable.selectEntered.RemoveListener(SwitchStateToOnlySwordHolded);
        _context._xrGrabInteractable.selectEntered.RemoveListener(SwitchStateToOnlyScabbardHolded);
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}

public class NoSwordInside : SwordScabbardState
{
    public NoSwordInside(SwordAndScabbardInteractionSystem context) : base(context)
    {
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}