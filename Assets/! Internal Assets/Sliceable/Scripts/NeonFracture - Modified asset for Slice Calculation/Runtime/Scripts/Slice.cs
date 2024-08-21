using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(Rigidbody))]
public class Slice : MonoBehaviour
{
    public SliceOptions sliceOptions;
    public CallbackOptions callbackOptions;

    /// <summary>
    /// The number of times this fragment has been re-sliced.
    /// </summary>
    private int currentSliceCount;

    /// <summary>
    /// Collector object that stores the produced fragments
    /// </summary>
    private GameObject fragmentRoot;

    /// <summary>
    /// Slices the attached mesh along the cut plane
    /// </summary>
    /// <param name="sliceNormalWorld">The cut plane normal vector in world coordinates.</param>
    /// <param name="sliceOriginWorld">The cut plane origin in world coordinates.</param> //TODO: разобраться зачем здесь sliceOrigin. По сути можно и без него обойтись.
    public List<GameObject> ComputeSlice(Vector3 sliceNormalWorld, Vector3 sliceOriginWorld)
    {

        var mesh = this.GetComponent<MeshFilter>().sharedMesh;

        if (mesh != null)
        {
            // If the fragment root object has not yet been created, create it now
            if (this.fragmentRoot == null)
            {
                // Create a game object to contain the fragments
                this.fragmentRoot = new GameObject($"{this.name}Slices");
                this.fragmentRoot.transform.SetParent(this.transform.parent);

                // Each fragment will handle its own scale
                this.fragmentRoot.transform.position = this.transform.position;
                this.fragmentRoot.transform.rotation = this.transform.rotation;
                this.fragmentRoot.transform.localScale = Vector3.one;
            }

            var sliceTemplate = CreateSliceTemplate();

            //Раньше было:
            /*var sliceNormalLocal = this.transform.InverseTransformDirection(sliceNormalWorld);
            var sliceOriginLocal = this.transform.InverseTransformPoint(sliceOriginWorld);*/
            //Почему изменил:
            //Если изменить рамзер родителя(например хочешь, чтобы враг был большим. С размером 3), то при резке он увеличивается в размерах дважды
            //(хз из-за чего. мб тут привязаны настройки импорта)
            //То есть если размер родителя = 3, то и меш почему-то увеличится в 3 раза. В конечном итоге получится размер 9.
            //Чтобы это исправить после резки объектов с изменённым размером, рамер родителя меняем на 1. То есть уже получается 1*3=3.
            //От этого в три раза уменьшаются коллайдеры и другие не связанные с мешем объекты(так что их localScale надо прорграмно возвращать назад)
            //НО. Также помимо того, что увеличивается размер изменяется ещё и место резки.
            //То есть бъёшь по телу => режется в коленях. Из-за расчёта следующих полей в локальных координатах.
            //Чтобы это исправить домножаем на размер родителя(не знаю как делать с множественными родителями, но с одним срабатывает).
            //Изменил на следующиее
            var sliceNormalLocal = Vector3.Scale(this.transform.InverseTransformDirection(sliceNormalWorld), transform.parent.localScale);
            var sliceOriginLocal = Vector3.Scale(this.transform.InverseTransformPoint(sliceOriginWorld), transform.parent.localScale);

            var fragments = Fragmenter.Slice(this.gameObject,
                             sliceNormalLocal,
                             sliceOriginLocal,
                             this.sliceOptions,
                             sliceTemplate,
                             this.fragmentRoot.transform);

            // Done with template, destroy it
            GameObject.Destroy(sliceTemplate);

            // Deactivate the original object
            this.gameObject.SetActive(false);

            // Fire the completion callback
            if (callbackOptions.onCompleted != null)
            {
                callbackOptions.onCompleted.Invoke();
            }

            return fragments;
        }

        return null;
    }

    /// <summary>
    /// Creates a template object which each fragment will derive from
    /// </summary>
    /// <returns></returns>
    private GameObject CreateSliceTemplate()
    {
        // If pre-fracturing, make the fragments children of this object so they can easily be unfrozen later.
        // Otherwise, parent to this object's parent
        GameObject obj = new GameObject();
        obj.name = "Slice";
        obj.tag = this.tag;

        // Update mesh to the new sliced mesh
        obj.AddComponent<MeshFilter>();

        // Add materials. Normal material goes in slot 1, cut material in slot 2
        var meshRenderer = obj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = new Material[2] {
            this.GetComponent<MeshRenderer>().sharedMaterial,
            this.sliceOptions.insideMaterial
        };

        // Copy collider properties to fragment
        var thisCollider = this.GetComponent<Collider>();
        var fragmentCollider = obj.AddComponent<MeshCollider>();
        fragmentCollider.convex = true;
        fragmentCollider.sharedMaterial = thisCollider.sharedMaterial;
        fragmentCollider.isTrigger = thisCollider.isTrigger;

        // Copy rigid body properties to fragment
        var thisRigidBody = this.GetComponent<Rigidbody>();
        var fragmentRigidBody = obj.AddComponent<Rigidbody>();
        if (thisRigidBody != null)
        {
            fragmentRigidBody.velocity = thisRigidBody.velocity;
            fragmentRigidBody.angularVelocity = thisRigidBody.angularVelocity;
            fragmentRigidBody.drag = thisRigidBody.drag;
            fragmentRigidBody.angularDrag = thisRigidBody.angularDrag;
            fragmentRigidBody.useGravity = thisRigidBody.useGravity;
        }

        // If refracturing is enabled, create a copy of this component and add it to the template fragment object
        if (this.sliceOptions.enableReslicing &&
           (this.currentSliceCount < this.sliceOptions.maxResliceCount))
        {
            CopySliceComponent(obj);
        }

        return obj;
    }

    /// <summary>
    /// Convenience method for copying this component to another component
    /// </summary>
    /// <param name="obj">The GameObject to copy this component to</param>
    private void CopySliceComponent(GameObject obj)
    {
        var sliceComponent = obj.AddComponent<Slice>();

        sliceComponent.sliceOptions = this.sliceOptions;
        sliceComponent.callbackOptions = this.callbackOptions;
        sliceComponent.currentSliceCount = this.currentSliceCount + 1;
        sliceComponent.fragmentRoot = this.fragmentRoot;
    }
}