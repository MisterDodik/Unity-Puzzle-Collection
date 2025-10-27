using UnityEngine;

namespace com.puzzles.ArchivePuzzle
{
    public class ImagePieceScript : MonoBehaviour
    {
        //BoxCollider2D coll;
        [HideInInspector] public bool isCorrect = false;
        public Vector2 winPos;

        SpriteRenderer spriteRenderer;
        Sprite initialSprite;
        Sprite winSprite;

        [HideInInspector] public ArchivePuzzleManager puzzleManager;

        public void Initialize(ArchivePuzzleManager manager, Sprites sprites)
        {
            puzzleManager = manager;
         //   coll = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            initialSprite = sprites.defaultSprite;
            winSprite = sprites.winSprite;
            winPos = -transform.parent.localPosition;
            isCorrect = false;

            spriteRenderer.sprite = initialSprite;
         //   coll.enabled = true;

            transform.parent.GetComponent<SpriteRenderer>().enabled = false;
            transform.parent.GetComponent<GlowScript>().LoadChild(this);
        }

        public void CloseEnough()
        {
            transform.parent.GetComponent<SpriteRenderer>().enabled = false;
            spriteRenderer.sprite = winSprite;
            transform.localPosition = winPos;
          //  coll.enabled = false;
            isCorrect = true;
        }
    }
}
