using _Scripts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Game.SceneManagement
{
    public class ContinueScreen : MonoBehaviour
    {
        public void Setup()
        {
            gameObject.SetActive(true);
        }

        public void RestartButton()
        {
            SceneManager.LoadScene("GameScene");
            GlobalVariables.coinsCollected = 0;
        }

        public void ExitButton()
        {
            //todo need to setup a main menu screen
            SceneManager.LoadScene("MainMenu"); 
        }
    }
}
