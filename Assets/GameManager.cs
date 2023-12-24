using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static int CoinCount = 0;
    public int cointCount;
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
        CoinCount = cointCount;
        TextMeshProUGUI = textMeshProUGUI;
    }
}
