using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectEnemyTypesScreen : MonoBehaviour
{
    [SerializeField] private SegmentedButton _roundCountButton;
    [Space]
    [SerializeField] private RectTransform _firstLine;
    [SerializeField] private RectTransform _secondLine;
    [Space]
    [SerializeField] private GameObject _cardSmallPrefab;
    [SerializeField] private GameObject _cardBigPrefab;

    private List<Card> _spawnedCards = new();
    private string[] _cachedCardValues;

    private const string CardHeaded = "Раунд";
    private const int OneLineSpacing = 32;
    private const int TwoLineSpacing = 16;

    public string[] GetValues()
    {
        string[] values = new string[_spawnedCards.Count];
        for (int i = 0; i < _spawnedCards.Count; i++)
        {
            values[i] = _spawnedCards[i].Value;
        }
        return values;
    }

    private void Awake()
    {
        ClearTransform(_firstLine.transform);
        ClearTransform(_secondLine.transform);
    }

    private void ClearTransform(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        SpawnCards(int.Parse(_roundCountButton.Value));
    }

    private void SpawnCards(int roundsCount)
    {
        if (_spawnedCards.Count == roundsCount)
        {
            return;
        }

        CacheValues();
        ClearTransform(_firstLine.transform);
        ClearTransform(_secondLine.transform);

        if (roundsCount <= 5)
        {
            FillOneLine(roundsCount);
        }
        else
        {
            FillTwoLines(roundsCount);
        }
    }

    private void CacheValues()
    {
        _cachedCardValues = new string[_spawnedCards.Count];
        for (int i = 0; i < _spawnedCards.Count; i++)
        {
            _cachedCardValues[i] = _spawnedCards[i].Value;
        }
    }

    private void FillOneLine(int roundsCount)
    {
        SetupLine(_firstLine, _cardBigPrefab.GetComponent<RectTransform>().sizeDelta.y, OneLineSpacing);
        SetupLine(_secondLine, 0);

        FillLine(_firstLine, _cardBigPrefab, roundsCount);
    }

    private void FillTwoLines(int roundsCount)
    {

        SetupLine(_firstLine, _cardSmallPrefab.GetComponent<RectTransform>().sizeDelta.y, TwoLineSpacing);
        SetupLine(_secondLine, _cardSmallPrefab.GetComponent<RectTransform>().sizeDelta.y, TwoLineSpacing);

        int firstLineCardsCount = roundsCount / 2 + 1;
        FillLine(_firstLine, _cardSmallPrefab, firstLineCardsCount);

        int secondLineCardCount = roundsCount - firstLineCardsCount;
        FillLine(_secondLine, _cardSmallPrefab, secondLineCardCount, firstLineCardsCount);
    }

    private void SetupLine(RectTransform line, float height, float spacing=0)
    {
        line.sizeDelta = new Vector2(line.sizeDelta.x, height);
        line.GetComponent<HorizontalLayoutGroup>().spacing = spacing;
    }

    private void FillLine(RectTransform line, GameObject cardPrefab, int cardsCount, int offset=0)
    {
        for (int i = 0; i < cardsCount; i++)
        {
            Card card = Instantiate(cardPrefab, line.transform).GetComponent<Card>();
            _spawnedCards.Add(card);
            int cardIndex = offset + i;
            card.ChangeHeader($"{CardHeaded} {cardIndex}");
            if (cardIndex < _cachedCardValues.Length)
            {
                card.SetOption(_cachedCardValues[cardIndex]);
            }
        }
    }
}
