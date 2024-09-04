using UnityEngine;

public class SliceLogicForSkinnedMeshWithOneCollider : SliceLogicBase
{
    [SerializeField] private Slice _sliceComponent;

    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshFilter _meshFilter;

    public override bool TrySlice(Vector3 sliceNormal, Vector3 sliceOrigin)
    {
        var material = _skinnedMeshRenderer.material; //их может быть несколько. И это проблема. TODO: исправить.
        Mesh bakedMesh = new Mesh();
        _skinnedMeshRenderer.BakeMesh(bakedMesh);
        _meshFilter.mesh = bakedMesh;
        _meshRenderer.material = material;


        var fragments = _sliceComponent.ComputeSlice(sliceNormal, sliceOrigin);

        return fragments != null;
    }

    private void Reset()
    {
        if (!gameObject.TryGetComponent<Slice>(out _sliceComponent))
            _sliceComponent = gameObject.AddComponent<Slice>();

        if (!gameObject.TryGetComponent<MeshRenderer>(out _meshRenderer))
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
        if (!gameObject.TryGetComponent<MeshFilter>(out _meshFilter))
            _meshFilter = gameObject.AddComponent<MeshFilter>();

        gameObject.TryGetComponent<SkinnedMeshRenderer>(out _skinnedMeshRenderer);
    }
}
