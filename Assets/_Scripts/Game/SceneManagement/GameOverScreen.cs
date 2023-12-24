using _Scripts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Game.SceneManagement
{
    public class GameOverScreen : MonoBehaviour
    {
        public void Setup()
        {
            gameObject.SetActive(true);
        }

        public void RestartButton()
        {
            GlobalVariables.coinsCollected = 0;
            SceneManager.LoadScene("GameScene");
        }

        public void ExitButton()
        {
            SceneManager.LoadScene("MainMenu"); 
        }
    }
}
