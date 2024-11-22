using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO:
//Уйти от кинематических джоинтов, а сделать джоинты на корневом объекте.(они там багованно запускались почему-то)

//Дополнительные: Переопределить цент масс и tensor, вдруг это поможет с тряской

public class SwordAndScabbardInteractionSystem : MonoBehaviour
{
    public const string SwordEndTag = "SwordEnd";

    [Range(0.0f, 1.0f)]
    public float FillAmount;

    [SerializeField] private float _exitFillAmount;

    [SerializeField] private List<SwordInScabbardKeyframe> Keyframes;

    /// <summary>
    /// Mouth - устье ножен (aka место, куда заходит острие меча в самом начале)
    /// </summary>
    [SerializeField] private OnTriggerComponent MouthTriggerChecker;

    [SerializeField] private Transform _scabbardStart;
    [SerializeField] private Transform _scabbardEnd;

    /// <summary>
    /// Джоинт, ограничивающий перемещение и повороты меча, как это делали бы реальные ножны, НО дающие мечу вывалиться из конца ножен.
    /// </summary>
    [SerializeField] private ConfigurableJoint _jointScabbardSidesCosplayer;

    /// <summary>
    /// Джоинт, не дающий мечу вывалиться из конца ножен.
    /// </summary>
    [SerializeField] private ConfigurableJoint _jointScabbardEndCosplayer;

    /// <summary>
    /// Объект с которого списываются кейфрэймы меча в ножнах. Должен быть напрямую дочерним объектом ножен.
    /// </summary>
    public Sword SwordComponentForKeyframes;

    public Collider[] scabbardColliders;

    private Sword _sword;
    
    private SwordInScabbardKeyframe _currentKeyframe;
    private int _currentKeyframeIndex;

    //пизиции, относительно ScabbardEnd
    private Vector3 _firstSwordTipPosition;
    private Vector3 _lastSwordTipPosition;

    public void Awake()
    {
        MouthTriggerChecker.OnEnter += OnMouthTriggerEnter;

        _firstSwordTipPosition = Keyframes[0].SwordTipPosition;
        _lastSwordTipPosition = Keyframes[^1].SwordTipPosition;
    }

    private void OnMouthTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(SwordEndTag))
            return;

        if (!collider.transform.parent.parent.TryGetComponent<Sword>(out Sword swordComponent))
            return;

        if (_sword == null)
            StartEnteringInScabbard(swordComponent);
    }

    [ContextMenu("MakeKeyframe")]
    public void MakeKeyframe()
    {
        if (Keyframes == null) Keyframes = new();

        float fillAmount;
        if (Keyframes.Count == 0) fillAmount = 0f;
        else if (Keyframes.Count == 1) fillAmount = 1f;
        else
        {
            Vector3 localSwordPosition = _scabbardEnd.InverseTransformPoint(SwordComponentForKeyframes.SwordTipTransform.position);

            Vector3 StartToEnd = Keyframes[^1].SwordTipPosition - Keyframes[0].SwordTipPosition;
            Vector3 StartToPoint = localSwordPosition - Keyframes[0].SwordTipPosition;

            // Проекция вектора StartToPoint на прямую StartToEnd
            float projectionLength = Vector3.Dot(StartToPoint, StartToEnd.normalized);

            fillAmount = projectionLength / StartToEnd.magnitude;
        }

        SwordInScabbardKeyframe newKeyframe = new()
        {
            SwordTipPosition = _scabbardEnd.InverseTransformPoint(SwordComponentForKeyframes.SwordTipTransform.position),
            SwordTipRotation = Quaternion.Inverse(_scabbardEnd.rotation) * SwordComponentForKeyframes.SwordTipTransform.rotation,
            FillAmount = fillAmount
        };

        Keyframes.Add(newKeyframe);

        Keyframes.Sort((a, b) => a.FillAmount.CompareTo(b.FillAmount));
    }

    public void StartEnteringInScabbard(Sword swordComponent)
    {
        if (Keyframes == null || Keyframes.Count == 0)
        {
            Debug.LogError("Не установлены ключевые кадры");
            return;
        }

        _sword = swordComponent;

        //отключаем коллизии между мечом и ножнами
        foreach (Collider swordCollider in _sword.Colliders)
        {
            foreach (Collider scabbardCollider in scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider);
        }

        //ПРОБЛЕМА при решении через кинаматический дочерний джоинт:
        //Меч немного отстаёт от ножен, если не делать его дочерним компонентом ножен. Вот так:
        _sword.transform.SetParent(this.transform, false);
        //Но если после этого отпустить мечь и взять его снова(пока он в ножнах), то XRGrabInteractable сделает parent=null или parent=wasBefore.
        //РЕШЕНИЕ: сделать свой MyXRInteractable : XRGrabInteractable с "override void Drop()",
        //где будет учитываться, изменился ли родитель во время переноса 


        //TODO: следующее убрать, когда сделаю обновлении поворота в кейфрейме.
        //Устанавливаем для меча начальный угол поворота.
        // Вычисляем целевое вращение для меча, чтобы кончик оказался на запомненном относительном вращении к ножнам
        Quaternion targetRotation = _scabbardEnd.rotation * Keyframes[0].SwordTipRotation;
        // Корректируем вращение _swordComponent так, чтобы его кончик занял запомненное положение
        Quaternion swordTipOffset = Quaternion.Inverse(_sword.SwordTipTransform.rotation) * targetRotation;
        _sword.transform.rotation = _sword.transform.rotation * swordTipOffset;

        
        _currentKeyframeIndex = 0;
        _currentKeyframe = Keyframes[_currentKeyframeIndex];

        SetupScabbardEndJoint();
        SetupScabbardSidesJoint();

        StartCoroutine(nameof(PerformeKeyframeSwitchingAndLogic));
    }

    private void SetupScabbardEndJoint()
    {
        //Прикрепляем джоинт к концу меча.
        _jointScabbardEndCosplayer.connectedAnchor = _sword.transform.InverseTransformPoint(_sword.SwordTipTransform.position);
        //TODO:Мб лимитировать надо тоже здась? потому что если лимитировать изначально, то с XY плохо получается, а это почему-то норм.

        //TODO: эту часть можно делать не в рантайме, а заранее. Надо только дать автоматизированную настройку.
        //помещаем якорь самого джоинта в точку начала ножен.
        _jointScabbardEndCosplayer.anchor = _jointScabbardEndCosplayer.transform.InverseTransformPoint(_scabbardStart.position);
        //Устанавливаем ограничения: разрешаем концу меча двигаться только до конца ножен.
        var linerLimit = _jointScabbardEndCosplayer.linearLimit;
        linerLimit.limit = Vector3.Magnitude(_scabbardStart.position - _scabbardEnd.position);
        _jointScabbardEndCosplayer.linearLimit = linerLimit;

        //после настройки - присоеденяем Rigidbody
        _jointScabbardEndCosplayer.connectedBody = _sword.Rigidbody;
    }

    private void SetupScabbardSidesJoint()
    {
        _jointScabbardSidesCosplayer.anchor = Vector3.zero;

        _jointScabbardSidesCosplayer.connectedAnchor = _sword.transform.InverseTransformPoint(_sword.SwordTipTransform.position);

        //нужные значения делать Limited и изменять интерполированно.
        _jointScabbardSidesCosplayer.xMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.yMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.zMotion = ConfigurableJointMotion.Free; //свободно, потому что ограничивается другим джоинтом
        _jointScabbardSidesCosplayer.angularYMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularXMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularZMotion = ConfigurableJointMotion.Locked;

        //после настройки - присоеденяем Rigidbody
        _jointScabbardSidesCosplayer.connectedBody = _sword.Rigidbody;
    }

    private IEnumerator PerformeKeyframeSwitchingAndLogic()
    {
        while (true)
        {
            FillAmount = СalculateFillAmount();

            Debug.Log(FillAmount);

            if (FillAmount < _exitFillAmount)
            {
                SwordExitScabbard();
                yield break;
            }
            else if (FillAmount > 1)
            {
                FillAmount = 1;
                //TrySetFullyInsideState();//это состояние фиксирует позицию полностью в ножнах.
            }

            UpdateCurrentKeyframe();

            if (_currentKeyframeIndex != Keyframes.Count - 1)
                LerpConfigurationBetweenKeyframes(_currentKeyframe, Keyframes[_currentKeyframeIndex + 1]);

            yield return null;
        }
    }

    /// <summary>
    /// Обновляет FillAmount.
    /// Устанавливает значение newValue < 0, если меч вышел из ножен.
    /// 0 <= newValue <= 1, если в пределах ножен
    /// newValue > 1, если за ножнами.
    /// </summary>
    private float СalculateFillAmount()
    {
        Vector3 localSwordTipPosition = _scabbardEnd.InverseTransformPoint(_sword.SwordTipTransform.position);

        Vector3 StartToEnd = _lastSwordTipPosition - _firstSwordTipPosition;
        Vector3 StartToPoint = localSwordTipPosition - _firstSwordTipPosition;

        // Проекция вектора StartToPoint на прямую StartToEnd
        float projectionLength = Vector3.Dot(StartToPoint, StartToEnd.normalized);

        var fillAmount = projectionLength / StartToEnd.magnitude;
        return fillAmount;
    }

    private void UpdateCurrentKeyframe()
    {
        while (true)
        {
            var wasSwitched = false;

            if (_currentKeyframeIndex != 0)
            {
                if (FillAmount < _currentKeyframe.FillAmount)
                {
                    _currentKeyframeIndex--;
                    _currentKeyframe = Keyframes[_currentKeyframeIndex];
                    wasSwitched = true;
                }
            }

            if (_currentKeyframeIndex != Keyframes.Count - 1)
            {
                if (FillAmount >= Keyframes[_currentKeyframeIndex + 1].FillAmount)
                {
                    _currentKeyframeIndex++;
                    _currentKeyframe = Keyframes[_currentKeyframeIndex];
                    wasSwitched = true;
                }
            }

            if (!wasSwitched)
                return;
        }
    }

    private void LerpConfigurationBetweenKeyframes(in SwordInScabbardKeyframe currentKeyframe, in SwordInScabbardKeyframe nextKeyframe)
    {
        var distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount = FillAmount - currentKeyframe.FillAmount;

        Vector3 targetWorldPosition;
        if (distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount < 0)
        {
            targetWorldPosition = _scabbardEnd.TransformPoint(Keyframes[0].SwordTipPosition);
            _jointScabbardSidesCosplayer.anchor = this.transform.InverseTransformPoint(targetWorldPosition);
            return;
        }

        var distanceBetweenKeyframesFillAmount = nextKeyframe.FillAmount - currentKeyframe.FillAmount;
        var stepBetweenKeyframes = distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount / distanceBetweenKeyframesFillAmount;

        //Way 1: joints. Work;
        var localRememberedSwordTipPosition = Vector3.Lerp(currentKeyframe.SwordTipPosition, nextKeyframe.SwordTipPosition, stepBetweenKeyframes);
        targetWorldPosition = _scabbardEnd.TransformPoint(localRememberedSwordTipPosition);
        _jointScabbardSidesCosplayer.anchor = this.transform.InverseTransformPoint(targetWorldPosition);

        //Если джоинт нельзя настроить идеально, возможно придётся делать всё через rb.MovePosition и rb.MoveRotation.
        /*//Way 2: rb.MovePosition with Arm counting
        var holderPosition = holderTransform.position;
        var newPosition = Vector3.Lerp(currentKeyframe.SwordTipPosition, nextKeyframe.SwordTipPosition, stepBetweenKeyframes);
        Vector3 targetTipWorldPosition = this.transform.TransformPoint(newPosition);
        // Вычисляем смещение, необходимое для перемещения наконечника в целевую позицию
        Vector3 offset = targetTipWorldPosition - _swordComponent.SwordTipTransform.position;
        // Применяем это смещение к позиции корневого объекта меча
        _swordComponent.Rigidbody.position += offset;*/
    }

    private void SwordExitScabbard()
    {
        foreach (Collider swordCollider in _sword.Colliders)
        {
            foreach (Collider scabbardCollider in scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider, false);
        }

        _sword = null;
        _jointScabbardEndCosplayer.connectedBody = null;
        _jointScabbardSidesCosplayer.connectedBody = null;


        _jointScabbardSidesCosplayer.xMotion = ConfigurableJointMotion.Free;
        _jointScabbardSidesCosplayer.yMotion = ConfigurableJointMotion.Free;
        _jointScabbardSidesCosplayer.zMotion = ConfigurableJointMotion.Free;
        _jointScabbardSidesCosplayer.angularYMotion = ConfigurableJointMotion.Free;
        _jointScabbardSidesCosplayer.angularXMotion = ConfigurableJointMotion.Free;
        _jointScabbardSidesCosplayer.angularZMotion = ConfigurableJointMotion.Free;
    }
}

[System.Serializable]
public struct SwordInScabbardKeyframe
{
    /// <summary>
    /// С какого значения заполнения начинают учитываться параметры этого ключегого кадра. 0 - меч не зашёл в ножны. 1 - меч полностью в ножнах
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float FillAmount;

    //позиция и поворот конца меча относительно конца ножен
    public Vector3 SwordTipPosition;
    public Quaternion SwordTipRotation;

    //public JointConfiguration JointConfiguration;
}

/*[System.Serializable]
public class JointConfiguration
{
    public int XandZAngleLimit;

}*/
