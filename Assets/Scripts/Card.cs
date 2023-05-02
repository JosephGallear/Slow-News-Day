using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardText;

    private Speech currentResponse;

    private RectTransform layoutTransform;

    float normalScale = 1;
    float hoverScale = 1.35f;

    private void Awake()
    {
        layoutTransform = transform.parent.GetComponent<RectTransform>();
    }

    public void SetResponse(Speech response)
    {
        currentResponse = response;
        cardText.text = response.speechText;
    }

    public Speech GetResponse()
    {
        return currentResponse;
    }

    public void ScaleUp()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleCard(0.25f, Vector3.one * hoverScale));
    }

    public void ScaleDown()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleCard(0.25f, Vector3.one * normalScale));
    }

    IEnumerator ScaleCard(float duration, Vector3 scale)
    {
        float timer = 0;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = scale;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, timer / duration);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutTransform);
            yield return null;
        }
    }
}
