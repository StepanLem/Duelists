using Trisibo;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRegistry", menuName = "Duelists/SceneRegistry")]
public class SceneRegistrySO : ScriptableObject
{
    [SerializeField] private SceneField _playerVRScene;
    [SerializeField] private SceneField _bootstrap;

    public SceneField PlayerVRScene => _playerVRScene;
    public SceneField Bootstrap => _bootstrap;
}
