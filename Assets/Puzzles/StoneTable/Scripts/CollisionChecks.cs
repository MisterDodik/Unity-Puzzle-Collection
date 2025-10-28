using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecks : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Sprite glowSprite;
    Sprite beamSprite;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        glowSprite = StoneTableManager.instance.data.glowSprite;
        beamSprite = StoneTableManager.instance.data.beamSprite;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        ReformedButtonScript button = collision.gameObject.GetComponent<ReformedButtonScript>();
        if (button != null)
            spriteRenderer.sprite = beamSprite;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        spriteRenderer.sprite = glowSprite;

    }
}
