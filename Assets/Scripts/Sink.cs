using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            StartCoroutine(DestroyPlateAfterDelay(other.gameObject, 1f));
        }
    }

    private IEnumerator DestroyPlateAfterDelay(GameObject plate, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(plate);
    }
}
