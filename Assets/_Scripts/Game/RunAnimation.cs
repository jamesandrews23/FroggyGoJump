using UnityEngine;

namespace _Scripts.Game
{
    public class RunAnimation : MonoBehaviour
    {
        public GameObject player;
        Animator animator;
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void TriggerAnimation()
        {
            animator.SetTrigger("Start");
        }
    }
}
