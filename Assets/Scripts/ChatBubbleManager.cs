using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubbleManager : MonoBehaviour
{
    public GameObject chatBubblePrefab; // Assign this in the scene inspector

    private static ChatBubbleManager _instance;

    public static ChatBubbleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ChatBubbleManager>();
            }
            return _instance;
        }
    }
}
