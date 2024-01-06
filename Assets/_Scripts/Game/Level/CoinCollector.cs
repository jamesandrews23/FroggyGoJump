using _Scripts.Game;
using _Scripts.Game.Level;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CoinCollector : MonoBehaviour
{
    public RectTransform coinTarget;
    private Canvas _canvas;
    public GameObject canvasCoin;
    public float cycle = 1f;
    private Tween _shakeTween;
    private ParticleSystem _starExplosion;
    private static TextMeshProUGUI _scoreText;
    private int _score;
    private AudioSource _coinSound;

    public void Start()
    {
        _score = GlobalVariables.coinsCollected;
        _canvas = GameManager.Canvas;
        coinTarget = GameManager.CoinTarget;
        _starExplosion = GameManager.StarExplosion;
        _scoreText = GameManager.TextMeshProUGUI;
        _scoreText.text = "" + _score;
        _coinSound = GameObject.Find("AudioSources").GetComponent<AudioSource>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Frog"))
        {
            _coinSound.Play();
            //first convert coin's coordinates to canvas coordiantes
            var canvasCoords = ConvertWorldToCanvasSpace(gameObject.transform.position);
            var generateCanvasObject = GenerateCanvasObject(canvasCoords);
            //remove this game object from scene
            Destroy(gameObject);
            //move coin to coin purse
            MoveCanvasCoin(generateCanvasObject, coinTarget.position);
        }
    }

    Vector2 ConvertWorldToCanvasSpace(Vector3 worldPosition)
    {
        // Convert world position to screen position
        if (Camera.main != null)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            // Convert screen position to canvas space
            var canvasPosition = Vector2.zero;
            switch (_canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    canvasPosition = screenPosition;
                    break;
                case RenderMode.ScreenSpaceCamera:
                {
                    Vector2 viewportPosition = Camera.main.ScreenToViewportPoint(screenPosition);
                    canvasPosition = new Vector2(viewportPosition.x * _canvas.pixelRect.width, viewportPosition.y * _canvas.pixelRect.height);
                    break;
                }
            }

            return canvasPosition;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private GameObject GenerateCanvasObject(Vector2 position)
    {
            // Instantiate the objectPrefab within the canvas
            var newObject = Instantiate(canvasCoin, _canvas.transform);
            // adjust the position of the new gameObject in canvas to match current position
            newObject.transform.position = position;
            newObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            newObject.GetComponent<RectTransform>().SetAsFirstSibling();
            return newObject;
    }

    private void MoveCanvasCoin(GameObject coin, Vector3 targetPosition){
        coin.GetComponent<RectTransform>().DOMove(targetPosition, cycle).SetEase(Ease.InBack).OnComplete(() => {
            // _starExplosion.Play();
            GlobalVariables.coinsCollected++;
            _scoreText.text = "" + GlobalVariables.coinsCollected;
        });
        coin.GetComponent<RectTransform>().DOScale(.5f, cycle * 2);
    }
}

