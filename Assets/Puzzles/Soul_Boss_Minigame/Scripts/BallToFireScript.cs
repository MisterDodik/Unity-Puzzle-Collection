using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.puzzles.Soul_Boss_Minigame
{
    public class BallToFireScript : MonoBehaviour
    {

        public BallTrainManager trainManager;

        Vector2 shootingPoint;

        [HideInInspector] public int hitObjectsCount = 0;
        private void OnCollisionEnter2D(Collision2D collision)
        { 
            if (collision.gameObject.GetComponent<Ball>() == null)
                return;
            hitObjectsCount++;
            if (hitObjectsCount > 1)
                return;

            int hitBallIndex = collision.gameObject.GetComponent<Ball>().index;

            shootingPoint = Vector2.zero;   //character position

            Vector2 selfPos = collision.transform.localPosition;
            Vector2 contactPoint = collision.contacts[0].point;

            Vector2 direction1 = (contactPoint - shootingPoint).normalized;
            float angle1 = Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg;
            angle1 = (angle1 + 360) % 360;


            Vector2 direction2 = (selfPos - shootingPoint).normalized;
            float angle2 = Mathf.Atan2(direction2.y, direction2.x) * Mathf.Rad2Deg;
            angle2 = (angle2 + 360) % 360;


            if (shootingPoint.x > selfPos.x)
            {
                if (shootingPoint.y < selfPos.y)      //gornji lijevi oktant
                {
                    //print("gornji lijevi");
                    if (angle1 > angle2)
                    {
                        hitBallIndex++;
                       // print("lijevo");
                    }
                    else
                    {
                        //hitBallIndex--;
                       // print("desno");
                    }
                }
                else                                //donji lijevi oktant
                {
                   // print("donji lijevi");
                    if (angle1 > angle2)
                    {
                        //hitBallIndex--;
                       // print("desno");
                    }
                    else
                    {
                        hitBallIndex++;
                       // print("lijevo");
                    }
                }
            }
            else
            {
                if (shootingPoint.y < selfPos.y)     //gornji desni oktant
                {
                  //  print("gornji desni");
                    if (angle1 > angle2)
                    {
                        hitBallIndex++;
                      //  print("lijevo");
                    }
                    else
                    {
                        //hitBallIndex--;
                      //  print("desno");
                    }
                }
                else                                //donji desni oktant
                {
                   // print("donji desni");
                    if (angle1 > angle2)
                    {
                        //hitBallIndex--;
                      //  print("desno");
                    }
                    else
                    {
                        hitBallIndex++;
                      //  print("lijevo");
                    }
                }
            }
            //print(("treba ubaciti na index: ",hitBallIndex));
            trainManager.AddNewBall(hitBallIndex);

            gameObject.SetActive(false);
        }
    }
}
