using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReformedButtonScript : MonoBehaviour
{
    private int buttonIndex;

    StoneTableManager tableManager;
    private void Start()
    {
        buttonIndex = transform.GetSiblingIndex();

        tableManager = StoneTableManager.instance;
    }

    private void OnMouseDown()
    {
        if (tableManager.moveInProgress)
            return;
        StoneTableManager.instance.Rotate(buttonIndex);
    }
}