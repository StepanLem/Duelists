using UnityEngine;

public static class DifficultySystem
{
    private static RoundSO _easyRound = Resources.Load<RoundSO>("1. Easy");
    private static RoundSO _mediumRound = Resources.Load<RoundSO>("2. Medium");
    private static RoundSO _hardRound = Resources.Load<RoundSO>("3. Hard");

    public static RoundSO GetRoundByDifficulty(string difficultyTag)
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
