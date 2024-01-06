using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static RectTransform CoinTarget;
    public RectTransform coinTarget;
    public static Canvas Canvas;
    public Canvas canvas;
    public static ParticleSystem StarExplosion;
    public ParticleSystem starExplosion;

    public TextMeshProUGUI textMeshProUGUI;

    public static TextMeshProUGUI TextMeshProUGUI;

    public void Awake()
    {
        CoinTarget = coinTarget;
        Canvas = canvas;
        StarExplosion = starExplosion;
        TextMeshProUGUI = textMeshProUGUI;
    }
}
