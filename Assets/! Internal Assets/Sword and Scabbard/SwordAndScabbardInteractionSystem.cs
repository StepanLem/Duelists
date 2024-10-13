using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
///
//TODO: добавить что при установке нового keyframe он устанавливался автоматически между подходящими значениями.
//TODO: + keyframe должны всегда быть в порядке возрастания по fillAmount.
//TODO: keyframe.fillamount может быть 0 только у 0-ого элемента. Если в инспекторе это не так, должно сразу выводить ошибку в консоль.(+ если филэмаунты равны тоже)
public class SwordAndScabbardInteractionSystem : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float FillAmount;

    public List<SwordInScabbardKeyframe> Keyframes;

    private SwordInScabbardKeyframe previousKeyframe;
    private SwordInScabbardKeyframe currentKeyframe;
    private SwordInScabbardKeyframe nextKeyframe;

    private int currentKeyframeIndex;

    public Collider[] scabbardColliders;

    [SerializeField] private Sword _swordComponent;

    public SphereCollider EndSwordTrigger;//for test

    [SerializeField] private Transform _scabbardStart;
    [SerializeField] private Transform _scabbardEnd;

    [SerializeField] private ConfigurableJoint _jointScabbardEndCosplayer;
    [SerializeField] private ConfigurableJoint _jointScabbardSidesCosplayer;//это может быть дочерний кинематик. Так что не обязательно родителю это вешать.

    public bool isSwordEntering;
    public bool isSwordFullyInside;

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
        if (Keyframes.Count < 0) { Debug.LogError("Нет ключевых кадров"); return; }

        _firstSwordPosition = Keyframes[0].SwordPosition;
        _lastSwordPosition = Keyframes[^1].SwordPosition;
        _lastSwordPositionRelativeToFirstPosition = _lastSwordPosition - _firstSwordPosition;
    }

    [ContextMenu("MakeKeyframe")]
    public void MakeKeyframe()
    {
        SwordInScabbardKeyframe newKeyframe = new()
        {
            SwordPosition = SwordTransformForKeyframes.localPosition,
        };

        Keyframes.Add(newKeyframe);
    }

    [ContextMenu("StartEnteringInScabbard")]
    public void StartEnteringInScabbard(/*Transform swordTransform*/)
    {
        if (_swordComponent != null)
        {
            Debug.Log("Ножны уже заняты мечом");
            return;
        }

        if (Keyframes.Count == 0)
        {
            Debug.LogError("Не установлены ключевые кадры");
            return;
        }

        var swordEndTrigger = EndSwordTrigger;//в место этого получает его из OnTriggerEnter; (ps: и этот метод тоже оттуда вызывать)
        _swordComponent = swordEndTrigger.GetComponentInParent<Sword>();//интересно есть ли какая-то выгода если идти через parent.TryGetComponent();

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

        isSwordEntering = true;
    }

    /// <summary>
    /// Настраивает джоинт, не дающий мечу вывалиться из конца ножен.
    /// </summary>
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
        //Настраиваем якоря. Крч я в инспекторе всё настроил: у мечя - на конце меча; у ножен - на начале ножен. Или якоря можно просто занулить?
        _jointScabbardSidesCosplayer.anchor = _jointScabbardSidesCosplayer.transform.InverseTransformPoint(_scabbardStart.position);

        _jointScabbardSidesCosplayer.connectedAnchor = _swordComponent.transform.InverseTransformPoint(_swordComponent.EndSwordPointTransform.position);

        //нужные значения делать Limited и изменять интерполированно.
        _jointScabbardSidesCosplayer.xMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.yMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.zMotion = ConfigurableJointMotion.Free; //свободно, потому что контролируется другим джоинтом
        _jointScabbardSidesCosplayer.angularYMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularXMotion = ConfigurableJointMotion.Locked;
        _jointScabbardSidesCosplayer.angularZMotion = ConfigurableJointMotion.Locked;

        //после настройки - присоеденяем Rigidbody
        _jointScabbardSidesCosplayer.connectedBody = _swordComponent.Rigidbody;
    }

    private IEnumerator PerformeKeyframeSwitchingAndLogic() //можно просто через тикер то же самое делать. и можно разделить на фиксед и обычный апдейт если нужно.
    {

        while (true)
        {
            yield return null;

            UpdateFillAmount();

            //if (FillAmount < 0) { SwordExitScabbard(); return; }

            TrySwitchKeyframe();

            if (nextKeyframe != null)
                LerpConfigurationBetweenKeyframes();
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

   /* private void Update()
    {
        if (!isSwordEntering)//вместо этого можно запускать и останавливать корутину.
            return;

        UpdateFillAmount();

        //if (FillAmount < 0) { SwordExitScabbard(); return; }


        TrySwitchKeyframe();

        if (nextKeyframe != null)
            LerpConfigurationBetweenKeyframes();
    }*/

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
            if (previousKeyframe != null && FillAmount < currentKeyframe.StartFillAmount)
            {
                SetCurrentKeyframe(currentKeyframeIndex - 1);
            }
            else if (nextKeyframe != null && FillAmount >= nextKeyframe.StartFillAmount)
            {
                SetCurrentKeyframe(currentKeyframeIndex + 1);
            }
            else break;
        }
    }

    private void LerpConfigurationBetweenKeyframes()
    {
        var distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount = FillAmount - currentKeyframe.StartFillAmount;
        var distanceBetweenKeyframesFillAmount = nextKeyframe.StartFillAmount - currentKeyframe.StartFillAmount;
        var stepBetweenKeyframes = distanceBetweenCurrentFillAmountAndCurrentKeyframeStartFillAmount / distanceBetweenKeyframesFillAmount;

        //TODO:  Как я думаю сейчас: всё это делается через поворот и перемещение объекта держателя джоинта.(видимо джоинт у третей позиции тоже надо будет перенести на этот объект и его тут же обновлять)
        //1.позиция 100% всегда. Её ключевые кадры будут всегда!
        //Пока что сделать возможным изменение позиции только в двух осях.

        //2.Поворот, когда катана. Так что видимо для этого будет отдельный метод у наследника. Но пока что тоже 100%.
        //Пока что сделать поворот только по одной оси.

        //_swordComponent.transform.localPosition = Vector3.Lerp(currentKeyframe.SwordPosition, nextKeyframe.SwordPosition, stepBetweenKeyframes);
        //rotation
        //joint config
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
    public float StartFillAmount;


    //relative to scabbard;
    public Vector3 SwordPosition;

    //public JointConfiguration JointConfiguration;
}

[System.Serializable]
public class JointConfiguration
{
    public int XandZAngleLimit;

}
