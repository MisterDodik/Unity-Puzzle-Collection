using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        TriangleLogics triangle = collision.gameObject.GetComponent<TriangleLogics>();
        if (triangle != null && spriteRenderer.enabled!=triangle.isLightSource)
        {
            spriteRenderer.enabled = triangle.isLightSource;
        }
    }
}
