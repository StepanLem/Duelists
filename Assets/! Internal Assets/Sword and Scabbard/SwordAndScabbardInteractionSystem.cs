using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
///
//TODO: keyframe.fillamount может быть 0 только у 0-ого элемента. Если в инспекторе это не так, должно сразу выводить ошибку в консоль.(+ если филамаунты равны тоже)


public class SwordAndScabbardInteractionSystem : MonoBehaviour
{
    public const string SwordEndTag = "SwordEnd";

    [Range(0.0f, 1.0f)]
    public float FillAmount;

    public List<SwordInScabbardKeyframe> Keyframes;

    private SwordInScabbardKeyframe previousKeyframe;
    private SwordInScabbardKeyframe currentKeyframe;
    private SwordInScabbardKeyframe nextKeyframe;

    private int currentKeyframeIndex;

    public Collider[] scabbardColliders;

    private Sword _swordComponent;

    public OnTriggerComponent OnTriggerExitChecker;

    [SerializeField] private Transform _scabbardStart;
    [SerializeField] private Transform _scabbardEnd;

    [SerializeField] private ConfigurableJoint _jointScabbardEndCosplayer;
    [SerializeField] private ConfigurableJoint _jointScabbardSidesCosplayer;//это может быть дочерний кинематик. Так что не обязательно родителю это вешать.

    public bool IsSwordInside => _swordComponent == null;
    private bool isSwordFullyInside;

    /// <summary>
    /// Объект с которого списываются кейфрэймы меча в ножнах. Должен быть напрямую дочерним объектом ножен.
    /// </summary>
    public Transform SwordTransformForKeyframes;

    //все позиции local
    private Vector3 _currentSwordPositionRelativeToFirstPosition;
    private Vector3 _firstSwordPosition;
    private Vector3 _lastSwordPosition;
    private Vector3 _lastSwordPositionRelativeToFirstPosition;

    public void Awake()
    {
        OnTriggerExitChecker.OnEnter += OnScabbardExitTriggerEnter;

        _firstSwordPosition = Keyframes[0].SwordPosition;
        _lastSwordPosition = Keyframes[^1].SwordPosition;
        _lastSwordPositionRelativeToFirstPosition = _lastSwordPosition - _firstSwordPosition;
    }

    private void OnScabbardExitTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(SwordEndTag))
            return;

        Sword swordComponent;
        if (!collider.transform.parent.parent.TryGetComponent<Sword>(out swordComponent))
            return;

        if (_swordComponent == null)
        {
            StartEnteringInScabbard(swordComponent); 
        }
        else
        {
            if (_swordComponent == swordComponent)
                SwordExitScabbard();
        }
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


        SetCurrentKeyframe(0);

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

            //if (FillAmount < 0) { SwordExitScabbard(); return; }

            TrySwitchKeyframe();

            if (nextKeyframe != null)
                LerpConfigurationBetweenKeyframes(currentKeyframe, nextKeyframe);

            yield return null;
        }
    }

    public void SetCurrentKeyframe(int keyframeIndex)
    {
        currentKeyframeIndex = keyframeIndex;
        currentKeyframe = Keyframes[keyframeIndex];

        if (currentKeyframeIndex != 0)
            previousKeyframe = Keyframes[currentKeyframeIndex - 1];
        else
            previousKeyframe = null; // Или какое-то по другому это помечать

        if (currentKeyframeIndex != Keyframes.Count - 1)
            nextKeyframe = Keyframes[currentKeyframeIndex + 1];
        else
            nextKeyframe = null; // Или какое-то по другому это помечать
    }

    private void UpdateFillAmount()
    {
        //вместо этого может лучше делать проекцию на прямую?
        _currentSwordPositionRelativeToFirstPosition = _swordComponent.transform.localPosition - _firstSwordPosition;
        FillAmount = Vector3.Magnitude(_currentSwordPositionRelativeToFirstPosition) / Vector3.Magnitude(_lastSwordPositionRelativeToFirstPosition);
    }

    public void TrySwitchKeyframe()
    {
        while (true)
        {
            if (previousKeyframe != null && FillAmount < currentKeyframe.FillAmount)
            {
                SetCurrentKeyframe(currentKeyframeIndex - 1);
            }
            else if (nextKeyframe != null && FillAmount >= nextKeyframe.FillAmount)
            {
                SetCurrentKeyframe(currentKeyframeIndex + 1);
            }
            else break;
        }
    }

    private void LerpConfigurationBetweenKeyframes(in SwordInScabbardKeyframe currentKeyframe, in SwordInScabbardKeyframe nextKeyframe)
    {
        var distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount = FillAmount - currentKeyframe.FillAmount;
        var distanceBetweenKeyframesFillAmount = nextKeyframe.FillAmount - currentKeyframe.FillAmount;
        var stepBetweenKeyframes = distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount / distanceBetweenKeyframesFillAmount;

        _jointScabbardSidesCosplayer.transform.localPosition = Vector3.Lerp(currentKeyframe.SwordPosition, nextKeyframe.SwordPosition, stepBetweenKeyframes);
        //Почему не работает? Записанные значения не выставляются корректно. Надо отдебажить.
        //_jointScabbardSidesCosplayer.transform.localRotation = Quaternion.Lerp(currentKeyframe.SwordRotation, nextKeyframe.SwordRotation, stepBetweenKeyframes);
    }

    private void SwordExitScabbard()
    {
        _swordComponent = null;
        _jointScabbardEndCosplayer.connectedBody = null;

        //включаем коллизии между мечом и ножнами
        foreach (Collider swordCollider in _swordComponent.swordColliders)
        {
            foreach (Collider scabbardCollider in scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider, false);
        }
    }
}

[System.Serializable]
public class SwordInScabbardKeyframe
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
