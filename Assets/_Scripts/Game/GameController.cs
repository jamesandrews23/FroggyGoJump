using System.Collections;
using System.Collections.Generic;
using _Scripts.Game;
using _Scripts.Game.InputControl;
using _Scripts.Game.SceneManagement;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameOverScreen gameOverScreen;
    // Start is called before the first frame update
    public GameObject player;

    private Controls frogControls;

    private void GameOver()
    {
        gameOverScreen.Setup(player.GetComponent<Controls>().platforms);
    }
    void Start()
    {
        frogControls = player.GetComponent<Controls>();
    }

    // Update is called once per frame
    void Update()
    {
        float cameraBottomY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).y;
        if (!frogControls.IsDragging && player.transform.position.y < cameraBottomY)
        {
            GameOver();
        }
    }
}
