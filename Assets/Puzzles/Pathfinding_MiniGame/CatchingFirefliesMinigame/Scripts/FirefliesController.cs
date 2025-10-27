using UnityEngine;

namespace com.puzzles.PathfindingMiniGame
{
    public class FirefliesController : MonoBehaviour
    {
        [SerializeField] private Firefly fireflyPrefab;
        [SerializeField] private int firflyCount;
        [SerializeField] private Vector2 positionRange;
        //private AudioSource m_AudioSource;

        private void Awake()
        {
            PuzzleStateController.OnStateChange += OnPuzzleStateChanged;
        }

        private void OnDestroy()
        {
            PuzzleStateController.OnStateChange -= OnPuzzleStateChanged;
        }

        void Init()
        {

        }

        void Show()
        {
            for (int i = 0; i < firflyCount; i++)
            {
                float xScale = Random.Range(0, 2) == 1 ? -1 : 1;
                var pos = new Vector2(Random.Range(positionRange.x, -positionRange.x),
                    Random.Range(positionRange.y, -positionRange.y));
                var firefly = Instantiate(fireflyPrefab, transform);
                firefly.transform.localScale = new Vector2(xScale, 1);
                firefly.SetRandomPosition();
            }
        }

        void PuzzleEnd()
        {
            foreach(Transform child in transform)
            {
                child.GetComponent<Firefly>().Kill();
            }
        }

        void Pause()
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Firefly>().Fly(false);
            }
        }

        void Resume()
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Firefly>().Fly(true);
            }
        }

        /*
         * called when puzzle state change
         */
        void OnPuzzleStateChanged(PuzzleState state)
        {
            switch(state)
            {
                case PuzzleState.INIT:
                    Init();
                    break;

                case PuzzleState.SHOW:
                    Show();
                    break;

                case PuzzleState.LOST:
                case PuzzleState.COMPLETE:
                    PuzzleEnd();
                    break;

                case PuzzleState.PAUSE:
                    Pause();
                    break;

                case PuzzleState.RESUME:
                    if (transform.childCount == 0)
                        Show();
                    else
                        Resume();
                    break;
            }
        }
    }
}
