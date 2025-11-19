using UnityEngine;
using UnityEngine.UI;

public class UICoinAnimator : MonoBehaviour
{
    public Image targetImage;
    public Sprite[] frames;
    public float frameRate = 12f;  // frames por segundo

    int index;
    float timer;

    void Awake()
    {
        if (!targetImage)
            targetImage = GetComponent<Image>();
    }

    void Update()
    {
        if (frames == null || frames.Length == 0 || !targetImage) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            index = (index + 1) % frames.Length;
            targetImage.sprite = frames[index];
        }
    }
}
