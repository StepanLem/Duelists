using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAndScabbardInteractionSystem : MonoBehaviour
{
    public const string SwordEndTag = "SwordEnd";

    [Range(0.0f, 1.0f)]
    public float FillAmount;

    [SerializeField] private float _exitFillAmount;

    public List<SwordInScabbardKeyframe> Keyframes;

    private SwordInScabbardKeyframe _currentKeyframe;
    private int _currentKeyframeIndex;

    private Sword _swordComponent;

    //Mouth - устье ножен (aka место, куда заходит острие меча в самом начале)
    public OnTriggerComponent MouthTriggerChecker;

    [SerializeField] private Transform _scabbardStart;
    [SerializeField] private Transform _scabbardEnd;

    [SerializeField] private ConfigurableJoint _jointScabbardEndCosplayer;
    [SerializeField] private ConfigurableJoint _jointScabbardSidesCosplayer;

    public bool IsSwordInside => _swordComponent == null;
    private bool isSwordFullyInside;

    /// <summary>
    /// Объект с которого списываются кейфрэймы меча в ножнах. Должен быть напрямую дочерним объектом ножен.
    /// </summary>
    public Transform SwordTransformForKeyframes;

    public Collider[] scabbardColliders;

    //все позиции local
    private Vector3 _firstSwordPosition;
    private Vector3 _lastSwordPosition;

    public void Awake()
    {
        MouthTriggerChecker.OnEnter += OnMouthTriggerEnter;

        _firstSwordPosition = Keyframes[0].SwordPosition;
        _lastSwordPosition = Keyframes[^1].SwordPosition;
    }

    private void OnMouthTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(SwordEndTag))
            return;

        if (!collider.transform.parent.parent.TryGetComponent<Sword>(out Sword swordComponent))
            return;

        if (_swordComponent == null)
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
            var currentSwordPositionRelativeToFirstPosition = SwordTransformForKeyframes.localPosition - Keyframes[0].SwordPosition;
            fillAmount = Vector3.Magnitude(currentSwordPositionRelativeToFirstPosition) / Vector3.Magnitude(Keyframes[^1].SwordPosition - Keyframes[0].SwordPosition);
        }

        SwordInScabbardKeyframe newKeyframe = new()
        {
            SwordPosition = SwordTransformForKeyframes.localPosition,
            SwordRotation = SwordTransformForKeyframes.localRotation,
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

        _swordComponent = swordComponent;

        //отключаем коллизии между мечом и ножнами
        foreach (Collider swordCollider in _swordComponent.swordColliders)
        {
            foreach (Collider scabbardCollider in scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider);
        }

        _swordComponent.transform.SetParent(this.transform, true);

        _swordComponent.transform.localRotation = Keyframes[0].SwordRotation;//если будет плохо при кручении, то мб тут нужен InverseTransform

        _currentKeyframeIndex = 0;
        _currentKeyframe = Keyframes[_currentKeyframeIndex];

        SetupScabbardEndJoint();
        SetupScabbardSidesJoint();

        StartCoroutine(nameof(PerformeKeyframeSwitchingAndLogic));
    }

    /// <summary>
    /// Настраивает джоинт, не дающий мечу вывалиться из конца ножен.
    /// </summary>
    //TODO: по сути здесь надо отталикиваться просто от последнего кейфрэйма и всё. Расположить в начале, а дистанцию до последнего сделать.
    private void SetupScabbardEndJoint()
    {
        //Прикрепляем джоинт к концу меча.
        _jointScabbardEndCosplayer.connectedAnchor = _swordComponent.transform.InverseTransformPoint(_swordComponent.EndSwordPointTransform.position);
        //TODO:Мб лимитировать надо тоже здась? потому что если лимитировать изначально, то с XY плохо получается, а это почему-то норм.

        //TODO: эту часть можно делать не в рантайме, а заранее. Надо только дать автоматизированную настройку.
        //помещаем якорь самого джоинта в точку начала ножен.
        _jointScabbardEndCosplayer.anchor = _jointScabbardEndCosplayer.transform.InverseTransformPoint(_scabbardStart.position);
        //Устанавливаем ограничения: разрешаем концу меча двигаться только до конца ножен.
        var linerLimit = _jointScabbardEndCosplayer.linearLimit;
        linerLimit.limit = Vector3.Magnitude(_scabbardStart.position - _scabbardEnd.position);
        _jointScabbardEndCosplayer.linearLimit = linerLimit;

        //после настройки - присоеденяем Rigidbody
        _jointScabbardEndCosplayer.connectedBody = _swordComponent.Rigidbody;
    }

    /// <summary>
    /// Настраивает джоинт, ограничивающий перемещение и повороты меча в ножнах исходя из значений кейфреймов.
    /// </summary>
    private void SetupScabbardSidesJoint()
    {
        _jointScabbardSidesCosplayer.anchor = Vector3.zero;

        _jointScabbardSidesCosplayer.connectedAnchor = _swordComponent.transform.InverseTransformPoint(_swordComponent.EndSwordPointTransform.position);

        //нужные значения делать Limited и изменять интерполированно.
        _jointScabbardSidesCosplayer.xMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.yMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.zMotion = ConfigurableJointMotion.Free; //свободно, потому что ограничивается другим джоинтом
        _jointScabbardSidesCosplayer.angularYMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularXMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularZMotion = ConfigurableJointMotion.Locked;

        //после настройки - присоеденяем Rigidbody
        _jointScabbardSidesCosplayer.connectedBody = _swordComponent.Rigidbody;
    }

    private IEnumerator PerformeKeyframeSwitchingAndLogic()
    {
        while (true)
        {
            UpdateFillAmount();

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

    /// <summary>
    /// Обновляет FillAmount.
    /// Устанавливает значение newValue < 0, если меч вышел из ножен.
    /// 0 <= newValue <= 1, если в пределах ножен
    /// newValue > 1, если за ножнами.
    /// </summary>
    private void UpdateFillAmount()
    {
        Vector3 localSwordPosition = transform.InverseTransformPoint(_swordComponent.transform.position);

        Vector3 StartToEnd = _lastSwordPosition - _firstSwordPosition;
        Vector3 StartToPoint = localSwordPosition - _firstSwordPosition;

        // Проекция вектора StartToPoint на прямую StartToEnd
        float projectionLength = Vector3.Dot(StartToPoint, StartToEnd.normalized);

        FillAmount = projectionLength / StartToEnd.magnitude;
    }

    private void LerpConfigurationBetweenKeyframes(in SwordInScabbardKeyframe currentKeyframe, in SwordInScabbardKeyframe nextKeyframe)
    {
        var distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount = FillAmount - currentKeyframe.FillAmount;

        if (distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount < 0)
        {
            _jointScabbardSidesCosplayer.transform.localPosition = Keyframes[0].SwordPosition;
            return;
        }

        var distanceBetweenKeyframesFillAmount = nextKeyframe.FillAmount - currentKeyframe.FillAmount;
        var stepBetweenKeyframes = distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount / distanceBetweenKeyframesFillAmount;

        _jointScabbardSidesCosplayer.transform.localPosition = Vector3.Lerp(currentKeyframe.SwordPosition, nextKeyframe.SwordPosition, stepBetweenKeyframes);
        //Почему не работает? Записанные значения не выставляются корректно. Надо отдебажить.
        //_jointScabbardSidesCosplayer.transform.localRotation = Quaternion.Lerp(currentKeyframe.SwordRotation, nextKeyframe.SwordRotation, stepBetweenKeyframes);
    }

    private void SwordExitScabbard()
    {
        foreach (Collider swordCollider in _swordComponent.swordColliders)
        {
            foreach (Collider scabbardCollider in scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider, false);
        }

        _swordComponent = null;
        _jointScabbardEndCosplayer.connectedBody = null;
        _jointScabbardSidesCosplayer.connectedBody = null;
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


    //relative to scabbard;
    public Vector3 SwordPosition;

    //relative to scabbard;
    public Quaternion SwordRotation;

    //public JointConfiguration JointConfiguration;
}

/*[System.Serializable]
public class JointConfiguration
{
    public int XandZAngleLimit;

}*/
