using System;
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

    //все позиции local
    private Vector3 _currentSwordPositionRelativeToFirstPosition;
    private Vector3 _firstSwordPosition;
    private Vector3 _lastSwordPosition;
    private Vector3 _lastSwordPositionRelativeToFirstPosition;

    public List<SwordInScabbardKeyframe> Keyframes;

    public Collider[] scabbardColliders;

    [SerializeField] private Sword _swordComponent;

    public SphereCollider EndSwordTrigger;//for test

    [SerializeField] private Transform _swordEndWhenStartEnteringScabbard;
    [SerializeField] private Transform _swordEndWhenFullyInScabbard;

    [SerializeField] private ConfigurableJoint _joint;

    public bool isSwordEntering;
    public bool isSwordFullyInside;

    /// <summary>
    /// Объект с которого списываются кейфрэймы меча в ножнах. Должен быть напрямую дочерним объектом ножен.
    /// </summary>
    public Transform SwordTransformForKeyframes;

    public void Awake()
    {
        if (Keyframes.Count < 0)
        {
            Debug.LogError("Нет ключевых кадров");
            return;
        }
            
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
        isSwordEntering = true;



        /*_joint.connectedAnchor = GetPositionRelativeTo(_swordComponent.EndSwordPointTransform.position, _swordComponent.transform.position);//можно бы заменить на local, но тогда надо, чтобы EndSwordPointTransform всегда был чайлдом корня меча

        //Настраиваем максимальный вектор вхождения меча в ножны:
        //Сначала помещаем якорь в точку старта входа меча.
        Debug.Log(_swordEndWhenStartEnteringScabbard.position);
        Debug.Log(_joint.transform.position);
        _joint.anchor = GetPositionRelativeTo(_swordEndWhenStartEnteringScabbard.position, _joint.transform.position);
        //Потом разрешаем мечу двигаться только до окончания входа меча.
        var linerLimit = _joint.linearLimit;
        linerLimit.limit = Vector3.Magnitude(_swordEndWhenStartEnteringScabbard.position - _swordEndWhenFullyInScabbard.position);
        _joint.linearLimit = linerLimit;

        //После настройки - подсоединяем rigidbody
        _joint.connectedBody = _swordComponent.Rigidbody;*/
    }

    private SwordInScabbardKeyframe previousKeyframe;
    private SwordInScabbardKeyframe currentKeyframe;
    private SwordInScabbardKeyframe nextKeyframe;

    private int currentKeyframeIndex;

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

    private void Update()
    {
        if (!isSwordEntering)//вместо этого можно запускать и останавливать корутину.
            return;

        UpdateFillAmount();

        /*if (FillAmount<0)
        {
            SwordExitScabbard();
            return;
        }*/

        TrySwitchKeyframe();

        if(nextKeyframe != null)
            LerpConfigurationBetweenKeyframes();
    }

    private void UpdateFillAmount()
    {
        _currentSwordPositionRelativeToFirstPosition = _swordComponent.transform.localPosition - _firstSwordPosition;
        //TODO: если fillAmount отрицательный, то "выйти из ножен"
        FillAmount = Vector3.Magnitude(_currentSwordPositionRelativeToFirstPosition) / Vector3.Magnitude(_lastSwordPositionRelativeToFirstPosition);
    }

    //TODO: проверка если перепрыгнули через кадр. для этого надо разделить на TryGetNexKeyframse и SetupKeyframeConfiguration
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

        _swordComponent.transform.localPosition = Vector3.Lerp(currentKeyframe.SwordPosition, nextKeyframe.SwordPosition, stepBetweenKeyframes);
        //rotation
        //joint config
    }

    private void SwordExitScabbard()
    {
        _swordComponent = null;
        _joint.connectedBody = null;

        //включаем коллизии между мечом и ножнами
        foreach (Collider swordCollider in _swordComponent.swordColliders)
        {
            foreach (Collider scabbardCollider in scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider, false);
        }
    }

    ///Когда кончик меча оказывается близко к ножнам(10 см), его начинает подтягивать к дырке входа.
    ///Когда он касается дырки входа, состояние меча переходит на "входит в ножны"
    ///ConfigurableJoint привязывается к кончику меча и позволяет ему менять position только в по лини заданной(в идеале даже по изогнутой, но пока что на стадии: "только по Y")
    ///И ограничивает ему угол поворота. Начальное ограничение очень маленькое, но чем глубже меч продвигается по линии, тем меньший ему разрешается держать угол наклона.
    ///Лист углов наклона - это ключевые кадры. Между ними значения интерполируются, каждый раз проверяя прогресс между ключевыми кадрами.
    ///Joint Motion Limit должен сам высчитываться из длинны начаольной и конечной позиции. А motion.anchor(не connectedAnchot, а именно anchor) должен быть на середине этой позиции.  

    ///Конец меча всегда должен двигаться по одной и той же траектории. Для этого joint, что контралирует меч может делать это через drive. Но хз получится ли так?
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
