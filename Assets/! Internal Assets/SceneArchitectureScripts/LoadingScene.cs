using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Image _loadingImage;

    public Image LoadingImage => _loadingImage;
}
