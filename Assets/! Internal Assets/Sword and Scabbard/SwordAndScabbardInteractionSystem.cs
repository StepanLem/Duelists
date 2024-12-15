using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//TODO Firstly:
//1. Надо бы сделать хелпер, чтобы он плавно развернул меч к нужному углу поворота.
//2. Есть пролаги во время входа в ножны и выхода. Возможно из-за того, что меняем transform.position, а не rb.position. Или почему-то другому. Надо разобраться.
//3. Сделать разные editorSword для отслеживания разных лимитов.

//TODO Secondly:
//1. Оптимизировать вычисления. В настройках физики; в настройках джоинтов; в частоте обновления корутины.
//2. Сделать разные типы мечей. И проверять можно ли засунуть данный тип меча в данные ножны.
//3. Адаптировать запомненные значения под текущий размер.
public class SwordAndScabbardInteractionSystem : MonoBehaviour
{
    public const string SwordEndTag = "SwordEnd";

    [Range(0.0f, 1.0f)]
    public float FillAmount;

    [SerializeField] private float _exitFillAmount;

    [SerializeField] private Quaternion _defaultSwordTipRotation;
    [SerializeField] private List<SwordInScabbardKeyframe> Keyframes;

    /// Нужен, чтобы можно было свободно изменять пивот ножен.
    [Tooltip("Точка отсчёта для кейфреймов. Если передвинуть - кейфреймы сломаются. Предлагается ставить в конец ножен.")]
    [SerializeField] private Transform _origin;

    [Tooltip("Mouth - устье ножен (aka место, куда заходит острие меча в самом начале)")]
    [SerializeField] private OnTriggerComponent MouthTriggerChecker;

    [SerializeField] internal XRGrabInteractable _xrGrabInteractable;
    [SerializeField] internal Rigidbody _rigidody;
    [SerializeField] private Collider[] _scabbardColliders;

    [System.NonSerialized]
    [Tooltip("Меч, что сейчас в ножнах")]
    internal Sword _sword;

    [Tooltip("Джоинт, ограничивающий перемещение и повороты меча, как это делали бы реальные ножны, НО дающие мечу вывалиться из конца ножен.")]
    private ConfigurableJoint _jointScabbardSidesCosplayer;

    [Tooltip("Джоинт, не дающий мечу вывалиться из конца ножен.")]
    private ConfigurableJoint _jointScabbardEndCosplayer;

    private SwordInScabbardKeyframe _currentKeyframe;
    private int _currentKeyframeIndex;

    public bool spawnSwordOnStart;
    [SerializeField] private GameObject _swordPrefab;

    private SwordScabbardState _currentState;
    internal OnlySwordHolded _onlySwordHolded;
    internal OnlySkabbardHolded _onlyScabbardHolded;
    internal BothAreHolded _bothAreHolded;
    internal BothNotHolded _bothNotHolded;

    public void Awake()
    {
        _onlySwordHolded = new(this);
        _onlyScabbardHolded = new(this);
        _bothAreHolded = new(this);
        _bothNotHolded = new(this);

        MouthTriggerChecker.OnEnter += OnMouthTriggerEnter;


        if (spawnSwordOnStart)
        {
            var swordGameObject = Instantiate(_swordPrefab, this.transform.parent);
            if (!swordGameObject.TryGetComponent<Sword>(out var sword))
            {
                Debug.LogError("Sword component is missing on prefab!");
                return;
            }

            // Позиция
            var localRememberedSwordTipPosition = Keyframes[^1].SwordTipPosition;
            var targetTipWorldPosition = _origin.TransformPoint(localRememberedSwordTipPosition);
            sword.transform.position += targetTipWorldPosition - sword.Tip.position;

            // Вращение
            Quaternion targetSwordTipRotation = _origin.rotation * Keyframes[^1].SwordTipRotation;
            Quaternion swordTipToTargetOffset = Quaternion.Inverse(sword.Tip.rotation) * targetSwordTipRotation;
            sword.transform.rotation = sword.transform.rotation * swordTipToTargetOffset;

            StartEnteringInScabbard(sword);
        }
    }

    private void OnMouthTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(SwordEndTag))
            return;

        //TODO: сделать распознование главного родителя как-то по умному. Такое распознование много где может понадобиться.
        if (!collider.transform.parent.parent.TryGetComponent<Sword>(out Sword newSword))
            return;

        if (_sword == null)
            StartEnteringInScabbard(newSword);
    }

    public void StartEnteringInScabbard(Sword newSword)
    {
        if (Keyframes == null || Keyframes.Count == 0)
        {
            Debug.LogError("Не установлены ключевые кадры");
            return;
        }

        _sword = newSword;

        //отключаем коллизии между мечом и ножнами
        foreach (Collider swordCollider in _sword.Colliders)
        {
            foreach (Collider scabbardCollider in _scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider);
        }

        //Устанавливаем для меча начальный угол поворота.
        // Вычисляем целевое вращение для меча, чтобы кончик оказался на запомненном относительном Origin
        Quaternion targetRotation = _origin.rotation * Keyframes[0].SwordTipRotation;
        // Корректируем вращение _swordComponent так, чтобы его кончик занял запомненное положение
        Quaternion swordTipOffset = Quaternion.Inverse(_sword.Tip.rotation) * targetRotation;
        _sword.transform.rotation = _sword.transform.rotation * swordTipOffset;

        _currentKeyframeIndex = 0;
        _currentKeyframe = Keyframes[_currentKeyframeIndex];

        SetupScabbardEndJoint();
        SetupScabbardSidesJoint();

        if (_xrGrabInteractable.isSelected && _sword.XRGrabInteractable.isSelected)
            _currentState = _bothAreHolded;
        else if (_xrGrabInteractable.isSelected)
            _currentState = _onlyScabbardHolded;
        else if (_sword.XRGrabInteractable.isSelected)
            _currentState = _onlySwordHolded;
        else
            _currentState = _bothNotHolded;

        _currentState.EnterState();
        Debug.Log("при заходе" + _currentState.GetType());

        StartCoroutine(nameof(PerformeKeyframeSwitchingAndLogic));
    }

    internal void SwitchState(SwordScabbardState newState)
    {
        _currentState.ExitState();
        _currentState = newState;
        Debug.Log(_currentState.GetType());
        _currentState.EnterState();
    }

    private void SetupScabbardEndJoint()
    {
        _jointScabbardEndCosplayer = this.gameObject.AddComponent<ConfigurableJoint>();

        _jointScabbardEndCosplayer.connectedBody = _sword.Rigidbody;
        _jointScabbardEndCosplayer.autoConfigureConnectedAnchor = false;
        _jointScabbardEndCosplayer.connectedAnchor = _sword.transform.InverseTransformPoint(_sword.Tip.position);

        //Устанавливаем якорь в начало ножен
        Vector3 worldFirstTipPosition = _origin.TransformPoint(Keyframes[0].SwordTipPosition);
        _jointScabbardEndCosplayer.anchor = this.transform.InverseTransformPoint(worldFirstTipPosition);

        //Разрешаем мечу двигаться только до конца ножен
        var linearLimit = new SoftJointLimit
        {
            limit = Vector3.Distance(Keyframes[0].SwordTipPosition, Keyframes[^1].SwordTipPosition)
        };
        _jointScabbardEndCosplayer.linearLimit = linearLimit;

        _jointScabbardEndCosplayer.projectionDistance = .001f;


        _jointScabbardEndCosplayer.zMotion = ConfigurableJointMotion.Limited;
    }

    private void SetupScabbardSidesJoint()
    {
        _jointScabbardSidesCosplayer = this.gameObject.AddComponent<ConfigurableJoint>();

        _jointScabbardSidesCosplayer.connectedBody = _sword.Rigidbody;
        _jointScabbardSidesCosplayer.autoConfigureConnectedAnchor = false;
        _jointScabbardSidesCosplayer.connectedAnchor = _sword.transform.InverseTransformPoint(_sword.Tip.position);

        _jointScabbardSidesCosplayer.projectionDistance = .001f;

        /*// Устанавливаем основную ось (переставляем Y вместо X)
        _jointScabbardSidesCosplayer.axis = Vector3.up; // Локальная ось Y становится осью X для джойнта
        // Устанавливаем вторичную ось (X вместо Y)
        _jointScabbardSidesCosplayer.secondaryAxis = Vector3.right; // Локальная ось X становится осью Y для джойнта*/

        //нужные значения делать Limited и изменять интерполированно.
        _jointScabbardSidesCosplayer.xMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.yMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.zMotion = ConfigurableJointMotion.Free; //свободно, потому что ограничивается другим джоинтом
        _jointScabbardSidesCosplayer.angularYMotion = ConfigurableJointMotion.Limited;
        _jointScabbardSidesCosplayer.angularXMotion = ConfigurableJointMotion.Limited;
        _jointScabbardSidesCosplayer.angularZMotion = ConfigurableJointMotion.Locked;//не разрешается менять, так как начинается тряска если освобождены все 3
    }

    private IEnumerator PerformeKeyframeSwitchingAndLogic()
    {
        while (true)
        {
            FillAmount = СalculateFillAmount(_sword, Keyframes[0].SwordTipPosition, Keyframes[^1].SwordTipPosition);

            //Debug.Log(FillAmount);

            if (FillAmount < _exitFillAmount)
            {
                SwordExitScabbard();
                yield break;
            }
            else if (FillAmount > 1)
            {
                FillAmount = 1;
                //SetFullyInsideState();//это состояние фиксирует позицию полностью в ножнах.
            }

            UpdateCurrentKeyframe();

            if (_currentKeyframeIndex != Keyframes.Count - 1)
                LerpConfigurationBetweenKeyframes(_currentKeyframe, Keyframes[_currentKeyframeIndex + 1]);

            yield return new WaitForFixedUpdate();
            //yield return null;
        }
    }

    /// <returns>value &lt; 0, если меч вышел из ножен. 
    /// 0 &lt;= value &lt;= 1, если в пределах ножен. 
    /// value &gt; 1, если за ножнами.</returns>
    private float СalculateFillAmount(Sword sword, Vector3 start, Vector3 end)
    {
        Vector3 localSwordTipPosition = _origin.InverseTransformPoint(sword.Tip.position);

        Vector3 StartToEnd = end - start;
        Vector3 StartToPoint = localSwordTipPosition - start;

        // Проекция вектора StartToPoint на прямую StartToEnd
        float projectionLength = Vector3.Dot(StartToPoint, StartToEnd.normalized);

        return projectionLength / StartToEnd.magnitude;
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
        

        var distanceBetweenKeyframesFillAmount = nextKeyframe.FillAmount - currentKeyframe.FillAmount;
        var stepBetweenKeyframes = distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount / distanceBetweenKeyframesFillAmount;

        if (stepBetweenKeyframes < 0)//если меч перед нулевым кадром
            stepBetweenKeyframes = 0;


        //Position
        var localRememberedSwordTipPosition = Vector3.Lerp(currentKeyframe.SwordTipPosition, nextKeyframe.SwordTipPosition, stepBetweenKeyframes);
        var targetWorldPosition = _origin.TransformPoint(localRememberedSwordTipPosition);
        _jointScabbardSidesCosplayer.anchor = this.transform.InverseTransformPoint(targetWorldPosition);


        //Rotation limits
        SoftJointLimit limit = new();

        // Ограничение углов по оси X
        limit.limit = FloatLerp(currentKeyframe.minX, nextKeyframe.minX, stepBetweenKeyframes);
        _jointScabbardSidesCosplayer.lowAngularXLimit = limit;
        limit.limit = FloatLerp(currentKeyframe.maxX, nextKeyframe.maxX, stepBetweenKeyframes);
        _jointScabbardSidesCosplayer.highAngularXLimit = limit;

        // Ограничение углов по оси Y
        limit.limit = FloatLerp(currentKeyframe.Y, nextKeyframe.Y, stepBetweenKeyframes);
        _jointScabbardSidesCosplayer.angularYLimit = limit;
    }

    private void SwordExitScabbard()
    {
        _currentState.ExitState();

        _sword.XRGrabInteractable.trackRotation = true;
        this._xrGrabInteractable.trackRotation = true;

        StartCoroutine(SmoothlyGoToArm());

        foreach (Collider swordCollider in _sword.Colliders)
        {
            foreach (Collider scabbardCollider in _scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider, false);
        }

        _sword = null;

        Destroy(_jointScabbardEndCosplayer);
        Destroy(_jointScabbardSidesCosplayer);
    }

    public IEnumerator SmoothlyGoToArm()
    {
        var grabbable = _sword.XRGrabInteractable;
        var remembered = grabbable.angularVelocityScale;
        grabbable.angularVelocityScale = 0.1f;//TODO: сделать настройку в инспекторе

        yield return new WaitForSeconds(0.2f);//TODO: сделать настройку в инспекторе

        grabbable.angularVelocityScale = remembered;
    }

    private float FloatLerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Объект с которого списываются кейфрэймы меча в ножнах. Должен быть напрямую дочерним объектом ножен.
    /// </summary>
    [Space]
    [Header("Keyframes Editor")]

    [SerializeField] private Sword _swordComponentForKeyframesCreation;

    [SerializeField] private int _editorKeyframeIndex;

    [SerializeField] private Sword _swordForAngleLimits_MinX;
    [SerializeField] private Sword _swordForAngleLimits_MaxX;
    [SerializeField] private Sword _swordForAngleLimits_Y;

    [ContextMenu("MakeKeyframe")]
    public void MakeKeyframe()
    {
        if (Keyframes == null) Keyframes = new();

        float fillAmount;
        if (Keyframes.Count == 0) fillAmount = 0f;
        else if (Keyframes.Count == 1) fillAmount = 1f;
        else fillAmount = СalculateFillAmount(_swordComponentForKeyframesCreation, Keyframes[0].SwordTipPosition, Keyframes[^1].SwordTipPosition);

        //Устанавливаем лимиты с меча для лимитов.
        //MinX
        Quaternion currentRotation_MinX = Quaternion.Inverse(_origin.rotation) * _swordForAngleLimits_MinX.Tip.rotation;
        Quaternion deviation_MinX = Quaternion.Inverse(_defaultSwordTipRotation) * currentRotation_MinX;
        var angularLimit_MinX = NormalizeAngles(deviation_MinX.eulerAngles);
        //MaxX
        Quaternion currentRotation_MaxX = Quaternion.Inverse(_origin.rotation) * _swordForAngleLimits_MaxX.Tip.rotation;
        Quaternion deviation_MaxX = Quaternion.Inverse(_defaultSwordTipRotation) * currentRotation_MaxX;
        var angularLimit_MaxX = NormalizeAngles(deviation_MaxX.eulerAngles);
        //Y
        Quaternion currentRotation_Y = Quaternion.Inverse(_origin.rotation) * _swordForAngleLimits_Y.Tip.rotation;
        Quaternion deviation_Y = Quaternion.Inverse(_defaultSwordTipRotation) * currentRotation_Y;
        var angularLimit_Y = NormalizeAngles(deviation_Y.eulerAngles);

        SwordInScabbardKeyframe newKeyframe = new()
        {
            SwordTipPosition = _origin.InverseTransformPoint(_swordComponentForKeyframesCreation.Tip.position),
            SwordTipRotation = Quaternion.Inverse(_origin.rotation) * _swordComponentForKeyframesCreation.Tip.rotation,
            FillAmount = fillAmount,
            minX = Math.Abs(angularLimit_MinX.y),
            maxX = Math.Abs(angularLimit_MaxX.y),
            Y = Math.Abs(angularLimit_Y.x),
        };

        Keyframes.Add(newKeyframe);

        Keyframes.Sort((a, b) => a.FillAmount.CompareTo(b.FillAmount));
    }

    [ContextMenu("SetDefaultSwordTipRotation")]
    public void SetDefaultSwordTipRotation()
    {
        _defaultSwordTipRotation = Quaternion.Inverse(_origin.rotation) * _swordComponentForKeyframesCreation.Tip.rotation;
    }

    [ContextMenu("NullifyAllLimitsIfLow")]
    public void NullifyAllLimitsIfLow()
    {
        for (int i = 0; i < Keyframes.Count; i++)
        {
            var kf = Keyframes[i];

            if (kf.maxX < 0) kf.maxX = 0;
            if (kf.minX < 0) kf.minX = 0;
            if (kf.Y < 0) kf.Y = 0;

            Keyframes[i] = kf;
        }
    }

    [ContextMenu("SetKeyframeDataToEditingSword")]
    public void SetKeyframeDataToEditingSword()
    {
        //Position
        var localRememberedSwordTipPosition = Keyframes[_editorKeyframeIndex].SwordTipPosition;
        var targetTipWorldPosition = _origin.TransformPoint(localRememberedSwordTipPosition);
        _swordComponentForKeyframesCreation.transform.position += targetTipWorldPosition - _swordComponentForKeyframesCreation.Tip.position;

        //Rotation
        Quaternion targetSwordTipRotation = _origin.rotation * Keyframes[_editorKeyframeIndex].SwordTipRotation;
        Quaternion swordTipToTargetOffset = Quaternion.Inverse(_swordComponentForKeyframesCreation.Tip.rotation) * targetSwordTipRotation;
        _swordComponentForKeyframesCreation.transform.rotation = _swordComponentForKeyframesCreation.transform.rotation * swordTipToTargetOffset;
    }

    // Нормализация углов в диапазон [-180, 180]
    private Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }
#endif

}

[System.Serializable]
public struct SwordInScabbardKeyframe
{
    [Tooltip("С какого значения заполнения начинают учитываться параметры этого ключегого кадра. 0 - меч не зашёл в ножны. 1 - меч полностью в ножнах")]
    [Range(0.0f, 1.0f)]
    public float FillAmount;

    //позиция и поворот конца меча относительно конца ножен
    public Vector3 SwordTipPosition;
    public Quaternion SwordTipRotation;

    [Tooltip("Максимальное отклонение угла в градусах.")]
    public float minX;
    public float maxX;
    public float Y;
}
