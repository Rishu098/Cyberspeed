using UnityEngine;
using UnityEngine.UI;
using System;

public class CardView : MonoBehaviour
{
    [SerializeField] GameObject front;
    [SerializeField] GameObject back;

    public Button button;
    public Transform visual;

    public int Index;
    public event Action<int> Clicked;

    void Awake()
    {
        button.onClick.AddListener(
            () => Clicked?.Invoke(Index));
    }

    public void Flip(bool show)
    {
        StopAllCoroutines();
        StartCoroutine(FlipAnim(show));
    }

    System.Collections.IEnumerator FlipAnim(bool show)
    {
        float t = 0;

        bool swapped = false;

        float start = show ? 0 : 180;
        float end = show ? 180 : 0;

        while (t < 1)
        {
            t += Time.deltaTime * 6;

            float angle = Mathf.Lerp(start, end, t);
            visual.localRotation = Quaternion.Euler(0, angle, 0);

            // 👉 SWAP AT 90 DEGREES
            if (!swapped && t >= 0.5f)
            {
                swapped = true;
                SetFaceVisible(show);
            }

            yield return null;
        }

        visual.localRotation = Quaternion.Euler(0, end, 0);
    }


    public void SetMatched()
    {
        button.interactable = false;
    }

    void SetFaceVisible(bool showFront)
    {
        front.SetActive(showFront);
        back.SetActive(!showFront);
    }
}
