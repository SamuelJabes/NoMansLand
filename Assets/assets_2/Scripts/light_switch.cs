using UnityEngine;
// Se seu projeto usa URP 2D, o Light2D está neste namespace:
using UnityEngine.Rendering.Universal;

public class HorrorLightFlicker2D : MonoBehaviour
{
    [Header("Referência")]
    [SerializeField] private Light2D light2D; // se deixar vazio, pega automático

    [Header("Intensidade base e amplitude")]
    [Min(0f)] public float baseIntensity = 1.0f;  // nível médio da luz
    [Range(0f, 5f)] public float amplitude = 0.6f; // quanto a luz varia em torno da base

    [Header("Frequências (Hz)")]
    [Min(0f)] public float freq1 = 8f;    // seno principal (tremulação rápida)
    [Min(0f)] public float freq2 = 23f;   // harmônica para irregularidade
    [Min(0f)] public float envelopeFreq = 0.7f; // envelope lento (respiração/queda)

    [Header("Limites")]
    public float minIntensity = 0.05f;
    public float maxIntensity = 2.5f;

    [Header("Outras opções")]
    public bool useUnscaledTime = false; // true se quiser ignorar timescale

    private float phase1, phase2, phaseEnv;
    private const float TAU = Mathf.PI * 2f;

    void Reset()
    {
        light2D = GetComponent<Light2D>();
    }

    void Awake()
    {
        if (!light2D) light2D = GetComponent<Light2D>();

        // fases aleatórias para o “gaguejo” não ficar repetitivo
        phase1 = Random.value * TAU;
        phase2 = Random.value * TAU;
        phaseEnv = Random.value * TAU;
    }

    void Update()
    {
        if (!light2D) return;

        float t = useUnscaledTime ? Time.unscaledTime : Time.time;

        // duas ondas senoidais (rápidas) + envelope (lento) para dar “vida”
        float s1 = Mathf.Sin(TAU * freq1 * t + phase1);
        float s2 = Mathf.Sin(TAU * freq2 * t + phase2);
        float env = 0.5f + 0.5f * Mathf.Sin(TAU * envelopeFreq * t + phaseEnv); // [0..1]

        // mistura com pesos (ajuste como quiser)
        float mixed = (0.6f * s1 + 0.4f * s2) * env;

        float target = baseIntensity + amplitude * mixed;
        target = Mathf.Clamp(target, minIntensity, maxIntensity);

        light2D.intensity = target;
    }
}
