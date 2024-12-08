﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Firstly:
//1.Стабилизировать ножны.(чтобы были так же спокойны, как меч)
//2.Убрать излишнее кручение ножен при входе/выходе меча. + сделать более плавное возвращение к своей изначальной позиции

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

    [SerializeField] private Collider[] _scabbardColliders;

    private Sword _sword;

    private SwordInScabbardKeyframe _currentKeyframe;
    private int _currentKeyframeIndex;

    public void Awake()
    {
        MouthTriggerChecker.OnEnter += OnMouthTriggerEnter;
    }

    private void OnMouthTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(SwordEndTag))
            return;

        //TODO: сделать распознование главного родителя как-то по умному. Такое распознование много где может понадобиться.
        if (!collider.transform.parent.parent.TryGetComponent<Sword>(out Sword swordComponent))
            return;

        if (_sword == null)
            StartEnteringInScabbard(swordComponent);
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
            foreach (Collider scabbardCollider in _scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider);
        }

        //TODO: человек может хватать и отпускать меч много раз. Надо сделать обработку событий.
        _sword.XRGrabInteractable.trackRotation = false;

        //Устанавливаем для меча начальный угол поворота.
        // Вычисляем целевое вращение для меча, чтобы кончик оказался на запомненном относительном вращении к ножнам
        Quaternion targetRotation = _scabbardEnd.rotation * Keyframes[0].SwordTipRotation;
        // Корректируем вращение _swordComponent так, чтобы его кончик занял запомненное положение
        Quaternion swordTipOffset = Quaternion.Inverse(_sword.Tip.rotation) * targetRotation;
        _sword.transform.rotation = _sword.transform.rotation * swordTipOffset;

        _currentKeyframeIndex = 0;
        _currentKeyframe = Keyframes[_currentKeyframeIndex];

        SetupScabbardEndJoint();
        SetupScabbardSidesJoint();

        StartCoroutine(nameof(PerformeKeyframeSwitchingAndLogic));
    }

    private void SetupScabbardEndJoint()
    {
        _jointScabbardEndCosplayer = this.gameObject.AddComponent<ConfigurableJoint>();

        _jointScabbardEndCosplayer.connectedBody = _sword.Rigidbody;
        _jointScabbardEndCosplayer.autoConfigureConnectedAnchor = false;
        _jointScabbardEndCosplayer.connectedAnchor = _sword.transform.InverseTransformPoint(_sword.Tip.position);

        _jointScabbardEndCosplayer.anchor = this.transform.InverseTransformPoint(_scabbardStart.position);//_scabbardStart.position заменить на keyframes[0].position

        var linearLimit = new SoftJointLimit
        {
            limit = Vector3.Distance(_scabbardStart.position, _scabbardEnd.position)//см. выше. и то же самое для _scabbardEnd => keyframes[^1]
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

        //anchor устанавливается на каждом кадре в зависимости от fillAmount.
        //Так что если тут выставлять, то тоже сначала fillAmount надо высчитать.
        //_jointScabbardSidesCosplayer.anchor = 
        //И то же самое с параметрами джоинта на повороты, когда их добавлю.

        _jointScabbardSidesCosplayer.projectionDistance = .001f;

        //нужные значения делать Limited и изменять интерполированно.
        _jointScabbardSidesCosplayer.xMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.yMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.zMotion = ConfigurableJointMotion.Free; //свободно, потому что ограничивается другим джоинтом
        _jointScabbardSidesCosplayer.angularYMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularXMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularZMotion = ConfigurableJointMotion.Locked;
    }

    private IEnumerator PerformeKeyframeSwitchingAndLogic()
    {
        while (true)
        {
            FillAmount = СalculateFillAmount(_sword, Keyframes[0].SwordTipPosition, Keyframes[^1].SwordTipPosition);

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

    /// <returns> 
    /// value &lt; 0, если меч вышел из ножен. 
    /// 0 &lt;= value &lt;= 1, если в пределах ножен. 
    /// value &gt; 1, если за ножнами. 
    /// </returns>
    private float СalculateFillAmount(Sword sword, Vector3 start, Vector3 end)
    {
        Vector3 localSwordTipPosition = _scabbardEnd.InverseTransformPoint(sword.Tip.position);

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

        Vector3 targetWorldPosition;
        if (distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount < 0)//если меч перед нулевым кадром
        {
            targetWorldPosition = _scabbardEnd.TransformPoint(Keyframes[0].SwordTipPosition);
            _jointScabbardSidesCosplayer.anchor = this.transform.InverseTransformPoint(targetWorldPosition);
            return;
        }

        var distanceBetweenKeyframesFillAmount = nextKeyframe.FillAmount - currentKeyframe.FillAmount;
        var stepBetweenKeyframes = distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount / distanceBetweenKeyframesFillAmount;

        //Position
        var localRememberedSwordTipPosition = Vector3.Lerp(currentKeyframe.SwordTipPosition, nextKeyframe.SwordTipPosition, stepBetweenKeyframes);
        targetWorldPosition = _scabbardEnd.TransformPoint(localRememberedSwordTipPosition);
        _jointScabbardSidesCosplayer.anchor = this.transform.InverseTransformPoint(targetWorldPosition);
    }

    private void SwordExitScabbard()
    {
        foreach (Collider swordCollider in _sword.Colliders)
        {
            foreach (Collider scabbardCollider in _scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider, false);
        }

        _sword = null;

        Destroy(_jointScabbardEndCosplayer);
        Destroy(_jointScabbardSidesCosplayer);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Объект с которого списываются кейфрэймы меча в ножнах. Должен быть напрямую дочерним объектом ножен.
    /// </summary>
    [SerializeField] private Sword _swordComponentForKeyframesCreation;

    [ContextMenu("MakeKeyframe")]
    public void MakeKeyframe()
    {
        if (Keyframes == null) Keyframes = new();

        float fillAmount;
        if (Keyframes.Count == 0) fillAmount = 0f;
        else if (Keyframes.Count == 1) fillAmount = 1f;
        else fillAmount = СalculateFillAmount(_swordComponentForKeyframesCreation, Keyframes[0].SwordTipPosition, Keyframes[^1].SwordTipPosition);

        SwordInScabbardKeyframe newKeyframe = new()
        {
            SwordTipPosition = _scabbardEnd.InverseTransformPoint(_swordComponentForKeyframesCreation.Tip.position),
            SwordTipRotation = Quaternion.Inverse(_scabbardEnd.rotation) * _swordComponentForKeyframesCreation.Tip.rotation,
            FillAmount = fillAmount
        };

        Keyframes.Add(newKeyframe);

        Keyframes.Sort((a, b) => a.FillAmount.CompareTo(b.FillAmount));
    }
#endif

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
