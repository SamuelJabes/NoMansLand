using UnityEngine;

public class ScoreTextPunch : MonoBehaviour
{
    public RectTransform target;
    public float punchScale = 1.2f;
    public float punchTime = 0.08f;

    Vector3 baseScale;
    bool isPunched;

    void Awake()
    {
        if (!target) target = GetComponent<RectTransform>();
        baseScale = target.localScale;
    }

    public void Punch()
    {
        if (!gameObject.activeInHierarchy || isPunched) return;
        StartCoroutine(PunchRoutine());
    }

    System.Collections.IEnumerator PunchRoutine()
    {
        isPunched = true;
        target.localScale = baseScale * punchScale;
        yield return new WaitForSeconds(punchTime);
        target.localScale = baseScale;
        isPunched = false;
    }
}
