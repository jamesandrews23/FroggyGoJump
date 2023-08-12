using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static int coins = 0;
    public static RectTransform CoinPurseRectTransform;
    public RectTransform coinPurseTransform;

    public void Awake()
    {
        CoinPurseRectTransform = coinPurseTransform;
    }
}
