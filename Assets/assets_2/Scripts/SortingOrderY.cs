using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingOrderY : MonoBehaviour
{
    private SpriteRenderer sr;
    [Tooltip("Offset opcional para ajustar manualmente o sorting.")]
    public int offset = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Quanto menor o Y, maior o sortingOrder (fica “na frente”)
        sr.sortingOrder = -(int)(transform.position.y * 100) + offset;
    }
}
