using Trisibo;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRegistry", menuName = "Duelists/SceneRegistry")]
public class SceneRegistrySO : ScriptableObject
{
    [SerializeField] private SceneField _playerVRScene;
    [SerializeField] private SceneField _playerFlatScene;
    [SerializeField] private SceneField _bootstrapScene;
    [SerializeField] private SceneField _mainMenuScene;
    [SerializeField] private SceneField _loadingScene;
    [SerializeField] private SceneField _gameplayScene;


    public SceneField PlayerVRScene => _playerVRScene;
    public SceneField PlayerFlatScene => _playerFlatScene;
    public SceneField BootstrapScene => _bootstrapScene;
    public SceneField MainMenuScene => _mainMenuScene;
    public SceneField LoadingScene => _loadingScene;
    public SceneField GameplayScene => _gameplayScene;
}
