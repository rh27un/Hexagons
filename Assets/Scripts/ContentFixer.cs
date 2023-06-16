using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentFixer : MonoBehaviour
{
    // I don't know
    void Awake()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
