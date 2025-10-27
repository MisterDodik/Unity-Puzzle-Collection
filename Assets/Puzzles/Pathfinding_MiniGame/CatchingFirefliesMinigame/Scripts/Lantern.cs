using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
//using Hellmade.Sound;

namespace com.puzzles.PathfindingMiniGame
{
    [System.Serializable]
    public struct GlowSprite
    {
        public Sprite Glow;
        public Sprite FireFlies;
    }

    public class Lantern : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private GlowSprite[] glowSprites;
        [SerializeField] private SpriteRenderer glowSpriteRenderer;
        [SerializeField] private SpriteRenderer fireflieSpriteRenderer;
        [SerializeField] private int fireflyToCaught;
        [SerializeField] private Vector3 winPosition;
        [SerializeField] private float winMoveDuration = 0.75f;

        [SerializeField]
        private AudioClip collectionSound;

        private Rigidbody2D m_RigidBody;
        private Camera m_Camera;

        private Vector3 m_InitialPosition;
        private bool m_Selected;
        private Vector3 m_Offset;

        private int m_FireflyCaught;
        private int m_CurrentGlowIndex;
        private bool m_IsPuzzleRunning;

        void Awake()
        {
            PuzzleStateController.OnStateChange += OnPuzzleStateChanged;
        }

        void OnDestroy()
        {
            PuzzleStateController.OnStateChange -= OnPuzzleStateChanged;
        }

        private void Start()
        {
            Init();
        }
        void Init()
        {
            m_Camera = Camera.main;
            m_RigidBody = GetComponent<Rigidbody2D>();
            m_InitialPosition = transform.localPosition;

            Show();
        }

        void Show()
        {
            m_RigidBody.bodyType = RigidbodyType2D.Static;
            transform.localPosition = m_InitialPosition;
            m_Selected = false;
            m_FireflyCaught = 0;
            m_CurrentGlowIndex = 0;
            m_IsPuzzleRunning = true;
            glowSpriteRenderer.sprite = null;
            fireflieSpriteRenderer.sprite = null;
        }

        void Complete()
        {
            m_Selected = false;
            m_IsPuzzleRunning = false;
            glowSpriteRenderer.sprite = glowSprites[glowSprites.Length - 1].Glow;
            fireflieSpriteRenderer.sprite = glowSprites[glowSprites.Length - 1].FireFlies;
            transform.DOLocalMove(winPosition, winMoveDuration);
        }

        void FireflyCaught()
        {
            m_FireflyCaught++;
            PlayCollectionSound();
            float percentage = (m_FireflyCaught / (float)fireflyToCaught) * 100f;
            Debug.Log("Percentage: " + percentage + " "+ m_FireflyCaught);
            int glowIndex = (int)percentage / 25;
            if (glowIndex > 0 && m_CurrentGlowIndex != glowIndex)
            {
                m_CurrentGlowIndex = glowIndex;
                glowSpriteRenderer.sprite = glowSprites[glowIndex - 1].Glow;
                fireflieSpriteRenderer.sprite = glowSprites[glowIndex - 1].FireFlies;
            }
            if (m_FireflyCaught >= fireflyToCaught)
            {
                m_IsPuzzleRunning = false;
                PuzzleStateController.OnStateChange?.Invoke(PuzzleState.COMPLETE);
            }
        }

        /*
         * called when puzzle state change
         */
        void OnPuzzleStateChanged(PuzzleState state)
        {
            switch (state)
            {
                case PuzzleState.INIT:
                    Init();
                    break;

                case PuzzleState.RESUME:
                case PuzzleState.SHOW:
                    Show();
                    break;

                case PuzzleState.FIRE_FLY_CAUGHT:
                    FireflyCaught();
                    break;

                case PuzzleState.COMPLETE:
                    Complete();
                    break;

                case PuzzleState.LOST:
                    m_IsPuzzleRunning = false;
                    break;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_IsPuzzleRunning && m_Selected == false)
            {
                m_RigidBody.bodyType = RigidbodyType2D.Kinematic;
                m_Selected = true;
                m_Offset = transform.position - m_Camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_Selected = false;
            m_RigidBody.bodyType = RigidbodyType2D.Static;
            if (m_IsPuzzleRunning && m_FireflyCaught < fireflyToCaught)
            {
                PuzzleStateController.OnStateChange?.Invoke(PuzzleState.LOST);
                m_IsPuzzleRunning = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_Selected && m_IsPuzzleRunning)
            {
                transform.position = m_Offset + m_Camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        public void PlayCollectionSound()
        {

        }
    }
}