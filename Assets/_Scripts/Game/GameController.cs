using _Scripts.Game.InputControl;
using _Scripts.Game.SceneManagement;
using UnityEngine;

namespace _Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        public ContinueScreen gameOverScreen;
        public GameObject player;
        private Controls _frogControls;

        void Start()
        {
            _frogControls = player.GetComponent<Controls>();
        }
        void Update()
        {
            float cameraBottomY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).y;
            bool isPlayerAttached = _frogControls.frogTongue.GetComponent<SpringJoint2D>().isActiveAndEnabled;
            if (!_frogControls.IsDragging && player.transform.position.y < cameraBottomY && !isPlayerAttached)
            {
                player.SetActive(false);
                GameOver();
            }
        }
        private void GameOver()
        {
            gameOverScreen.Setup();
        }
    }
}
