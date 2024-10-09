using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class SwordAndScabbardInteractionSystem : MonoBehaviour
{
    //public List<SwordInScabbardKeyframe> Keyframes;

    public Collider[] scabbardColliders;

    [SerializeField] private Sword _swordComponent;

    public SphereCollider EndSwordCollider;//for test

    [SerializeField] private Transform _swordEndWhenStartEnteringScabbard;
    [SerializeField] private Transform _swordEndWhenFullyInScabbard;

    /// <summary>
    /// from 0 to 1
    /// </summary>
    public float SwordEnteringScabbardProgress;

    [SerializeField] private ConfigurableJoint _joint;

    /*public bool isSwordNearEnter;
    public bool isSwordEntering;
    public bool isSwordFullyInside;*/    



    public void Awake()
    {
        //всё это должно быть установлено так по умолчанию. то есть на этапе editKeyframes, но пока что так.
        _joint.autoConfigureConnectedAnchor = false;
    }

    /*[ContextMenu("MakeKeyFrame")]
    public void MakeKeyFrame()
    {
        SwordInScabbardKeyframe newKeyframe = new();
        //newKeyframe.RotationDelta = finalKeyframe.RotationDelta  swordTransform.rotation;

        Keyframes.Add(newKeyframe);
    }*/

    [ContextMenu("StartEnteringInScabbard")]
    public void StartEnteringInScabbard(/*Transform swordTransform*/)
    {
        if (_swordComponent != null)
        {
            Debug.Log("Ножны уже заняты мечом");
            return;
        }

        var swordEndTrigger = EndSwordCollider;//в место этого получает его из OnTriggerEnter; (ps: и этот метод тоже оттуда вызывать)
        _swordComponent = swordEndTrigger.GetComponentInParent<Sword>();//интересно есть ли какая-то выгода если идти через parent.TryGetComponent();

        //отключаем коллизии между мечом и ножнами
        foreach (Collider swordCollider in _swordComponent.swordColliders)
        {
            foreach (Collider scabbardCollider in scabbardColliders)
                Physics.IgnoreCollision(swordCollider, scabbardCollider);
        }

        _swordComponent.transform.parent = this.transform;


        _joint.connectedAnchor = GetPositionRelativeTo(_swordComponent.EndSwordPointTransform.position, _swordComponent.transform.position);//можно бы заменить на local, но тогда надо, чтобы EndSwordPointTransform всегда был чайлдом корня меча

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
        _joint.connectedBody = _swordComponent.Rigidbody;
    }

    public static Vector3 GetPositionRelativeTo(Vector3 targetWorldPosition, Vector3 originWorldPosition)
    {
        return targetWorldPosition - originWorldPosition;
    }

    public void OnSwordExitScabbard()
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
}

[System.Serializable]
public class SwordInScabbardKeyframe
{
    public JointConfiguration JointConfiguration;

    /// <summary>
    /// Rotation Delta relative to the full entry of the sword into the scabbard
    /// </summary>
    public Quaternion RotationDelta;

    /// <summary>
    /// До какого момента действуют параметры этого ключегого кадра. 0 - меч не зашёл в ножны. 1 - меч полностью в ножнах
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float WorkUntilPercent;
}

[System.Serializable]
public class JointConfiguration
{
    public int XandZAngleLimit;

}
