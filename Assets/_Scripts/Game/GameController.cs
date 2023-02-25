using _Scripts.Game.InputControl;
using _Scripts.Game.SceneManagement;
using UnityEngine;

namespace _Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        public GameOverScreen gameOverScreen;
        // Start is called before the first frame update
        public GameObject player;

        private Controls _frogControls;

        private void GameOver()
        {
            gameOverScreen.Setup((int) player.GetComponent<Controls>().maxHeightReached);
        }
        void Start()
        {
            _frogControls = player.GetComponent<Controls>();
        }

        // Update is called once per frame
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
    }
}
