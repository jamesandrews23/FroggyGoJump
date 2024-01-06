using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Game
{
    public class MainMenuSceneManager : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
