using UnityEngine;

public class DifficultySystem : MonoBehaviour
{
    [SerializeField] private RoundSO _easyRound;
    [SerializeField] private RoundSO _mediumRound;
    [SerializeField] private RoundSO _hardRound;

    public RoundSO GetRoundByDifficulty(string difficultyTag)
    {
        return difficultyTag switch
        {
            "Easy" => _easyRound,
            "Medium" => _mediumRound,
            "Hard" => _hardRound,
            _ => throw new System.NotImplementedException()
        };
    }
}
