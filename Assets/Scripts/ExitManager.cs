using UnityEngine;

public class ExitManager : MonoBehaviour
{
    public static ExitManager Instance { get; private set; }
    public Transform exitPoint;

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
        if (exitPoint == null)
        {
            Debug.LogError("Exit point is not assigned in ExitManager.");
        }
        else
        {
            Debug.Log("Exit point is assigned: " + exitPoint.name);
        }
    }
}
