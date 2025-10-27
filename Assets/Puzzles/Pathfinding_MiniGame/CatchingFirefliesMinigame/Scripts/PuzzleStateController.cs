using System;
using System.Collections;
using UnityEngine;

namespace com.puzzles.PathfindingMiniGame
{
    public class PuzzleStateController : MonoBehaviour
    {
        [SerializeField] private float resetDelay = 1.5f;
        private PuzzleState m_CurrentState;
        private Coroutine m_PuzzleEndRoutine;

        public static Action<PuzzleState> OnStateChange;


        [SerializeField] private PathFindingManager gameManager;


        public Transform pRewardItemTransform => throw new NotImplementedException();

        public event EventHandler OnFinished;
        public event EventHandler OnStuck;

        private void Awake()
        {

            OnStateChange += OnPuzzleStateChanged;
        }

     

        void OnShowUIScreen(bool isShown, int id)
        {
            OnStateChange?.Invoke(isShown ? PuzzleState.PAUSE : PuzzleState.RESUME);
        }

        void OnPuzzleHintShown(bool isShown)
        {
            OnStateChange?.Invoke(isShown ? PuzzleState.RESUME : PuzzleState.PAUSE);
        }

        private void OnDestroy()
        {
            OnStateChange -= OnPuzzleStateChanged;
        }

        /*
         * called when puzzle state change
         */
        void OnPuzzleStateChanged(PuzzleState state)
        {
            m_CurrentState = state;
            switch (state)
            {
                case PuzzleState.LOST:
                    if (m_PuzzleEndRoutine != null) StopCoroutine(m_PuzzleEndRoutine);
                    m_PuzzleEndRoutine = StartCoroutine(PuzzleEnd(resetDelay));
                    break;

                case PuzzleState.COMPLETE:
                    Completed();
                    break;

                case PuzzleState.PAUSE:
                    if (m_PuzzleEndRoutine != null) StopCoroutine(m_PuzzleEndRoutine);
                    break;
            }   
        }

        IEnumerator PuzzleEnd(float delay)
        {
            yield return new WaitForSeconds(resetDelay);
            ResetGame();
            m_PuzzleEndRoutine = null;
        }


        private void Start()
        {
            Show();
        }
        private bool mIsPuzzleInitialized = false;
        public void Show()
        {
            if (!mIsPuzzleInitialized)
            {
                mIsPuzzleInitialized = true;
                OnStateChange?.Invoke(PuzzleState.INIT);
                OnStateChange?.Invoke(PuzzleState.SHOW);
            }
            else
            {
                OnStateChange?.Invoke(PuzzleState.RESUME);
            }
        }

        public void ResetGame()
        {
            if (m_CurrentState == PuzzleState.SHOW) return;
            if (m_PuzzleEndRoutine != null) StopCoroutine(m_PuzzleEndRoutine);
            OnStateChange?.Invoke(PuzzleState.SHOW);
        }

        public void Skip()
        {
            if (m_CurrentState == PuzzleState.COMPLETE) return;
            if (m_PuzzleEndRoutine != null) StopCoroutine(m_PuzzleEndRoutine);
            OnStateChange?.Invoke(PuzzleState.COMPLETE);

            Completed();
        }

        public void Completed()
        {
            gameManager.LoadNextPart();
        }

        public void OnBack()
        {
            OnStateChange?.Invoke(PuzzleState.RESUME);
        }
    }
}
