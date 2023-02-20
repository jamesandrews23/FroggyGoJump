using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Game.SceneManagement
{
    public class GameOverScreen : MonoBehaviour
    {
        public TextMeshProUGUI pointsText;
    
        public void Setup(int score)
        {
            gameObject.SetActive(true);
            pointsText.text = score.ToString() + " POINTS";
        }

        public void RestartButton()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void ExitButton()
        {
            //todo need to setup a main menu screen
            SceneManager.LoadScene("MainMenu"); 
        }
    }
}
