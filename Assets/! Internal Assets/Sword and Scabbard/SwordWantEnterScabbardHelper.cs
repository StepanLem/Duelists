using System;
using UnityEngine;

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
            //Debug.Log("��� ������� ", Sword);
            return;
        }

        Sword = other.GetComponentInParent<Sword>();

        //������� �� SwordEnd. �������.
        /*var dstanceFromColliderCenter = Vector3.Magnitude(Sword.EndSwordPointTransform.position - myTrigger.center);
        if (dstanceFromColliderCenter < myTrigger.radius)
            return;*/

        //TODO:�������� �� IsJistExitScabbard;

        StartHelping();
    }

    private void StartHelping()
    {
        IsHelping = true;

        //TODO: ����������� ����� ���� � ����� ������ ����� � �����.
        //���� ��������? ����� ������ ������� AddForce?

        //��� �� ������������, � �������������. ���� ������������� ������ �����������. � ���� ���������� � ���� �������� ������ ��� 20 ��������, �� ������������ ���� �������

        //��� ��� ���� ���� �������� � ����� ��������. �� ���� ��������� ���� ����� ���� ������������� ������������. ��� ���������. � ������ ������ ���� ������������� �� �������.
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(SwordEndTag))
            return;

        if (!IsHelping)
        {
            //Debug.Log("��� �� ����� ��������");
            return;
        }

        StopHelping();
    }

    private void StopHelping()
    {


        IsHelping = false;
    }
}
