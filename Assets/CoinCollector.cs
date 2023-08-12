using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCollector : MonoBehaviour
{
    private Vector3 coinPurse = Vector3.zero; // Drag and drop the Treasure Chest RectTransform here in the Inspector
    public float coinGrowthFactor = 2.0f;
    public float duration = 10.0f;
    public void Start()
    {
        if (GameManager.CoinPurseRectTransform != null)
        {
            Vector2 localPositionInCoinPurse = new Vector2(0.5f, 0.5f);
            Vector3 globalPosition = GameManager.CoinPurseRectTransform.TransformPoint(localPositionInCoinPurse);
            Vector3 localPosition = Camera.main.ScreenToWorldPoint(globalPosition);
            coinPurse = localPosition;
            Debug.Log("Coin Purse Transform: " + coinPurse);
        }
        else
        {
            Debug.LogWarning("Coin purse reference not set!");
        }
    }
    // When a coin collides with the player
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Frog"))
        {
            // Increasae size of coin
            gameObject.transform.localScale *= coinGrowthFactor;
            // Move the coin towards the treasure chest using a coroutine
            StartCoroutine(MoveCoinToChest(gameObject, coinPurse));
        }
    }

    // Coroutine to move the coin towards the treasure chest
    private IEnumerator MoveCoinToChest(GameObject coin, Vector3 targetPosition)
    {
        float timer = 0.0f;
        
        while (timer < duration)
        {
            // Resize the coin to gradually make it smaller
            if(coin.transform.localScale.x > 0.25f)
                coin.transform.localScale *= (1 - timer / duration) * 1;

            coin.transform.position = Vector3.Lerp(coin.transform.position, targetPosition, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}

