using System;
using UnityEngine;


/// <summary>
/// На этом классе отвесвенность за довод меча до состояния когда он не может крутиться в ножнах.
/// Он должен плавно развернуть меч, чтобы он входил в прорезь ножен и смотрел отсрым концом на ножны.
///+он не должен давать вылезать за края ножен пока только-только заходит(то есть либо автоматом входить в начальную позицию пока не успел задеть, либо сделать лимиты.)
/// </summary>
public class SwordWantEnterScabbardHelper : MonoBehaviour
{
    public const string SwordEndTag = "SwordEnd";
    public Sword Sword;
    public SphereCollider myTrigger;

    public bool IsHelping;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(SwordEndTag))
            return;

        if (IsHelping)
        {
            //Debug.Log("Уже помогаю ", Sword);
            return;
        }

        Sword = other.GetComponentInParent<Sword>();

        //заменил на SwordEnd. Удалить.
        /*var dstanceFromColliderCenter = Vector3.Magnitude(Sword.EndSwordPointTransform.position - myTrigger.center);
        if (dstanceFromColliderCenter < myTrigger.radius)
            return;*/

        //TODO:проверка на IsJistExitScabbard;

        StartHelping();
    }

    private void StartHelping()
    {
        IsHelping = true;

        //TODO: притягивать конец меча к точке начала входа в ножны.
        //тоже джоинтом? Может просто физикой AddForce?

        //Или не притягивание, а доворачивание. Типо овверрайдится джоинт контроллера. И если отклонение в угле поворота меньше чем 20 градусов, то оверрайдится угол джоинта

        //Крч сто проц надо помогать с углом поворота. То есть положение руки может быть продиктованно контроллером. Или предметом. В данном случае надо переключаться на предмет.
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(SwordEndTag))
            return;

        if (!IsHelping)
        {
            //Debug.Log("Ещё не начал помагать");
            return;
        }

        StopHelping();
    }

    private void StopHelping()
    {


        IsHelping = false;
    }
}
