using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChairManager : MonoBehaviour
{
    public static ChairManager Instance { get; private set; }
    public List<Transform> chairs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Populate chairs list from scene
        if (chairs == null)
        {
            chairs = new List<Transform>();
        }

        chairs = FindObjectsOfType<Chair>().Select(c => c.transform).ToList();
        Debug.Log($"Number of chairs found: {chairs.Count}");
    }
}
