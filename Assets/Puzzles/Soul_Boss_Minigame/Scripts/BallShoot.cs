using UnityEngine;
using DG.Tweening;

namespace com.puzzles.Soul_Boss_Minigame
{
    public class BallShoot : MonoBehaviour
    {
        public GameObject ballToFire;
        public Transform centerPosition;
        public float ballSpeed = 10;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                ballToFire.GetComponent<BallToFireScript>().hitObjectsCount = 0;



                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
                Vector2 direction = (Camera.main.ScreenToWorldPoint(mousePos) - centerPosition.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                ballToFire.transform.position = centerPosition.position;

                ballToFire.SetActive(true);
                Vector3 offscreenTarget = ballToFire.transform.position + (Vector3)direction * 10;

                float distance = Vector3.Distance(ballToFire.transform.position, offscreenTarget);
                float duration = distance / ballSpeed;

                ballToFire.transform.DOMove(offscreenTarget, duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        ballToFire.SetActive(false);
                        //enable mouse events
                    });
            }
        }
    }
}
