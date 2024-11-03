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

    [Inject]
    public void Construct(MatchManager matchManager)
    {
        _matchManager = matchManager;
    }

    public void StartMatch()
    {
        int roundsCound = int.Parse(_roundsCountButton.Value);
        _matchManager.StarMatch(roundsCound);
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
