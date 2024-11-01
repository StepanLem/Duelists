using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StartSoloGameScreen : MonoBehaviour
{
    [SerializeField] private SegmentedButton _difficultyButton;
    [SerializeField] private SegmentedButton _roundsCountButton;
    [SerializeField] private SegmentedButton _enemyTypeButton;
    [SerializeField] private Button _chooseEnemyTypesButton;

    private MatchManager _matchManager;
    private DifficultySystem _difficultySystem;

    [Inject]
    public void Construct(MatchManager matchManager, DifficultySystem difficultySystem)
    {
        _matchManager = matchManager;
        _difficultySystem = difficultySystem;
    }

    public void StartMatch()
    {
        int roundsCound = int.Parse(_roundsCountButton.Value);
        RoundSO roundData = _difficultySystem.GetRoundByDifficulty(_difficultyButton.Value);
        _matchManager.StarMatch(roundsCound, roundData);
    }

    private void Start()
    {
        _chooseEnemyTypesButton.interactable = _enemyTypeButton.Value == "Manual";
        _enemyTypeButton.SelectSegment += EnemyTypeButton_SelectSegment;
    }

    private void EnemyTypeButton_SelectSegment(string value)
    {
        _chooseEnemyTypesButton.interactable = value == "Manual";
    }
}
