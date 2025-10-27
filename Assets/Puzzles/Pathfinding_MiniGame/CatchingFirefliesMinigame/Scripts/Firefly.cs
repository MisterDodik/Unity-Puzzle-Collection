using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace com.puzzles.PathfindingMiniGame
{
    public class Firefly : MonoBehaviour
    {
        //[SerializeField] private SkeletonAnimation m_Animation;
        [SerializeField] private float moveDuration;
        [SerializeField] private float fastMoveDuration;
        [SerializeField] private Vector2 localMoveRange;
        [SerializeField] private Vector2 screenMoveRange;
        [SerializeField] private float trapDelay = 1;
        [SerializeField] private float timeToTrap = 0.5f;
        [SerializeField] private float moveInLanternDuration = 0.15f;
        private bool m_InRange;
        private bool m_StayInRange;
        private bool m_Trapped;

        Vector2 GetRandomPosition()
        {
            if (transform.localPosition.x < -screenMoveRange.x 
                || transform.localPosition.x > screenMoveRange.x)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            float xPos = transform.localScale.x > 0 ? -localMoveRange.x : localMoveRange.x;
            float yPos = Random.Range(-localMoveRange.y, localMoveRange.y);

            if (transform.localPosition.y > screenMoveRange.y)
                yPos = -localMoveRange.y;
            else if (transform.localPosition.y < -screenMoveRange.y)
                yPos = localMoveRange.y;
            return transform.localPosition + new Vector3(xPos, yPos);
        }

        Vector2 GetScreenRandomPosition()
        {
            var pos = new Vector2(Random.Range(screenMoveRange.x + 1.0f, -screenMoveRange.x - 1.0f),
                Random.Range(screenMoveRange.y + 1.0f, -screenMoveRange.y - 1.0f));
            return pos;
        }

        private void Start()
        {
            Move(GetRandomPosition()); ;
        }

        public void Fly(bool enable)
        {
            if(enable)
            {
                DOTween.Play(transform);
            }
            else
            {
                DOTween.Pause(transform);
            }
        }

        public void SetRandomPosition()
        {
            transform.localPosition = GetScreenRandomPosition();
        }

        public void RePosition()
        {
            Move(GetScreenRandomPosition(), true);
        }

        void Move(Vector2 pos, bool fastMove = false)
        {
            if (m_Trapped)
                return;
            DOTween.Kill(transform);
            float duration = fastMove ? fastMoveDuration : moveDuration;
            transform.DOLocalMove(pos, duration).OnComplete(() =>
            {
                Move(GetRandomPosition());
            });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var lantern = collision.GetComponent<Lantern>();
            if(lantern != null)
            {
                RePosition();
                if(collision.isTrigger)
                {
                    if (m_InRange)
                    {
                        Trapped(lantern.transform.position);
                    }
                    else
                    {
                        m_StayInRange = true;
                        m_InRange = true;
                        StartCoroutine(OutOfRange());
                        StartCoroutine(InRange(lantern.transform.position));
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var lantern = collision.GetComponent<Lantern>();
            if (lantern != null)
            {
                m_StayInRange = false;
            }
        }

        void Trapped(Vector3 position)
        {
            m_Trapped = true;
            DOTween.Kill(transform);
            transform.DOMove(position, moveInLanternDuration).OnComplete(() =>
            {
                PuzzleStateController.OnStateChange?.Invoke(PuzzleState.FIRE_FLY_CAUGHT);
                Kill();
            });
        }

        IEnumerator OutOfRange()
        {
            yield return new WaitForSeconds(trapDelay);
            m_InRange = false;
        }

        IEnumerator InRange(Vector3 position)
        {
            yield return new WaitForSeconds(timeToTrap);
            if(m_StayInRange)
            {
                Trapped(position);
            }
        }

        public void Kill()
        {
            DOTween.Kill(transform);
            StopAllCoroutines();
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }
    }
}