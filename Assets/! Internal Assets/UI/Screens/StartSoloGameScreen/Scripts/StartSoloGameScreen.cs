using UnityEngine;
using UnityEngine.UI;

public class StartSoloGameScreen : MonoBehaviour
{
    [SerializeField] private SegmentedButton _enemyTypeButton;
    [SerializeField] private Button _chooseEnemyTypesButton;

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
