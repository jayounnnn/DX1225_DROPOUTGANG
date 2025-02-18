using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodSplatterScreen : MonoBehaviour
{
    public GameObject[] bloodPrefabs;
    public Transform canvasTransform; 
    public int maxBloodStains = 5;
    public float fadeDuration = 3f; 

    private Queue<GameObject> bloodStains = new Queue<GameObject>();

    public void ShowBloodSplatter()
    {
        if (bloodPrefabs.Length == 0)
        {
            Debug.LogWarning("No blood splatter prefabs assigned!");
            return;
        }

        // Remove old stains if limit is exceeded
        if (bloodStains.Count >= maxBloodStains)
        {
            GameObject oldStain = bloodStains.Dequeue();
            Destroy(oldStain);
        }

        GameObject bloodPrefab = bloodPrefabs[Random.Range(0, bloodPrefabs.Length)];

        GameObject newBlood = Instantiate(bloodPrefab, canvasTransform);
        newBlood.transform.localPosition = new Vector3(Random.Range(-300, 300), Random.Range(-200, 200), 0);

        StartCoroutine(FadeOutBlood(newBlood));

        bloodStains.Enqueue(newBlood);
    }

    IEnumerator FadeOutBlood(GameObject blood)
    {
        Image bloodImage = blood.GetComponent<Image>();
        Color originalColor = bloodImage.color;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            bloodImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(blood);
    }
}
