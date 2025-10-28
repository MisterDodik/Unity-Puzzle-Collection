using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonScript : MonoBehaviour
{
    [SerializeField] GameObject trianglePrefab;

    int[] triangleData;


    //rotation duration
    private float duration = 0.5f;

    SpriteRenderer spriteRenderer;
    Collider2D selfCollider;

    bool wasCorrect = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        selfCollider = GetComponent<Collider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        triangleData = EnchantmentTableManager.instance.data.triangleData[transform.GetSiblingIndex()-1].position;

        SpawnTriangles();
    }

    void SpawnTriangles()
    {
        foreach(int i in triangleData)
        {
            GameObject triangle = Instantiate(trianglePrefab, transform);
            triangle.transform.localPosition = Vector3.zero;
            triangle.transform.localRotation = Quaternion.Euler(0, 0, -i * 60);
        }
    }




    private void OnMouseDown()
    {
        StartCoroutine(RotateOverTime());
    }
    IEnumerator RotateOverTime()
    {
        // disabling surrounding colliders
        selfCollider.enabled = false;

        Collider2D[] surroundingPaths = Physics2D.OverlapCircleAll(transform.position, 1f, 64);
        foreach (Collider2D item in surroundingPaths)
        {
            item.enabled = false;
            item.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        EnchantmentTableManager.instance.moveInProgress = true;
        
        // hexagon rotation
        float elapsedTime = 0f;

        Quaternion currentRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, currentRotation.eulerAngles.z - 60);

        while (elapsedTime < duration)
        {
            transform.localRotation = Quaternion.Lerp(currentRotation, endRotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = endRotation;

        // re-enabling surrounding colliders
        selfCollider.enabled = true;

        foreach (BoxCollider2D item in surroundingPaths)
        {
            item.enabled = true;
        }

        EnchantmentTableManager.instance.moveInProgress = false;

    }
    public void CheckState(bool isCorrect)
    {
        if (isCorrect)
        {
            spriteRenderer.sprite = EnchantmentTableManager.instance.hexagonShineSprite;

            if (!wasCorrect)
                EnchantmentTableManager.instance.checkWin(1);
            wasCorrect = true;
        }
        else
        {
            spriteRenderer.sprite = EnchantmentTableManager.instance.defaultHexagonSprite;

            if (wasCorrect)
                EnchantmentTableManager.instance.checkWin(-1);
            wasCorrect = false;
        }
    }
}
