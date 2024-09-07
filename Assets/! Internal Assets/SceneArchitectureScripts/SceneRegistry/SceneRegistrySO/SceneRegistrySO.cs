using Trisibo;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRegistry", menuName = "Duelists/SceneRegistry")]
public class SceneRegistrySO : ScriptableObject
{
    [SerializeField] private SceneField _playerVRScene;
    [SerializeField] private SceneField _bootstrap;
    [SerializeField] private SceneField _mainMenu;
    [SerializeField] private SceneField _loading;

    public SceneField PlayerVRScene => _playerVRScene;
    public SceneField Bootstrap => _bootstrap;
    public SceneField MainMenu => _mainMenu;
    public SceneField Loading => _loading;
}
