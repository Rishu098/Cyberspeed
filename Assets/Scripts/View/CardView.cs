using UnityEngine;
using UnityEngine.UI;
using System;

public class CardView : MonoBehaviour
{
    [SerializeField] GameObject front;
    [SerializeField] GameObject back;

    [SerializeField] private Image frontImage;

    public Button button;
    public Transform visual;

    public int Index;
    public int CardId { get; set; }

    public event Action<int> Clicked;

    void Awake()
    {
        button.onClick.AddListener(() =>
            Clicked?.Invoke(Index));
    }

    // -------- FLIP LOGIC --------

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

            // swap front/back at half rotation
            if (!swapped && t >= 0.5f)
            {
                swapped = true;
                SetFaceVisible(show);
            }

            yield return null;
        }

        visual.localRotation = Quaternion.Euler(0, end, 0);
    }

    private void SetFaceVisible(bool showFront)
    {
        if (front != null) front.SetActive(showFront);
        if (back != null) back.SetActive(!showFront);
    }

    // -------- DATA --------

    public void SetImage(Sprite s, int id)
    {
        frontImage.sprite = s;
        CardId = id;
    }

    // -------- MATCH STATE --------

    public void SetMatched()
    {
        // disable interaction when matched
        if (button != null)
            button.interactable = false;
    }
}
