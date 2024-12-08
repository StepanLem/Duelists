using UnityEngine;

public class PlayerDataProvider : MonoBehaviour, IPlayerDataProvider
{
    [SerializeField] private GameObject LeftHand;
    [SerializeField] private GameObject RightHand;
    [SerializeField] private GameObject Head;
    public PlayerData GetData()
    {
        return new PlayerData(
            CreateTransform(LeftHand), 
            CreateTransform(RightHand), 
            CreateTransform(Head));
    }

    public void SetData(PlayerData playerData)
    {
        ApplyTransform(playerData.LeftHand, LeftHand);
        ApplyTransform(playerData.RightHand, RightHand);
        ApplyTransform(playerData.Head, Head);
    }

    private void ApplyTransform(PlayerData.Transform model, GameObject gameObject)
    {
        gameObject.transform.position = model.Position;
        gameObject.transform.rotation = model.Rotation;
    }

    private PlayerData.Transform CreateTransform(GameObject gameObject)
    {
        return new PlayerData.Transform(gameObject.transform.position, gameObject.transform.rotation);
    }
}