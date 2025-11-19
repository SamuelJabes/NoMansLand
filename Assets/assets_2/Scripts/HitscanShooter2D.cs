using System.Collections;
using UnityEngine;

public class HitscanShooter2D : MonoBehaviour
{
    [Header("Referências")]
    public WeaponInventory2D inventory;   // arraste o do Player
    public Transform muzzle;              // ponta do cano
    public LineRenderer tracerPrefab;     // opcional: um prefab simples de LineRenderer

    [Header("Raycast")]
    public LayerMask hitMask;             // inclua Enemy/Obstacles, REMOVA Player
    public bool useUnscaledTime = false;

    float nextShotTime;
    bool reloading;

    void Reset()
    {
        if (!inventory) inventory = GetComponentInParent<WeaponInventory2D>();
    }

    void Update()
    {
        if (inventory?.Current == null || reloading) return;

        AimAtMouse();

        var cfg = inventory.Current.config;
        bool wantsShoot = cfg.automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        // recarregar
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryStartReload();
            return;
        }

        // sem bala no pente? tentar recarregar automático ao clicar
        if (wantsShoot && inventory.Current.clip < cfg.ammoPerShot)
        {
            TryStartReload();
            return;
        }

        if (wantsShoot && Time.time >= nextShotTime)
        {
            FireShot();
            nextShotTime = Time.time + (1f / Mathf.Max(0.01f, cfg.fireRate));
        }
    }

    void AimAtMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - transform.position);
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, ang);

        // flip opcional do sprite da arma
        Vector3 s = transform.localScale;
        s.y = Mathf.Abs(s.y) * (Mathf.Abs(ang) > 90f ? -1f : 1f);
        transform.localScale = s;
    }

    void FireShot()
    {
        var state = inventory.Current;
        var cfg = state.config;

        // Consome munição POR DISPARO (shotgun continua 1 por clique)
        if (state.clip < cfg.ammoPerShot) return;
        state.clip -= cfg.ammoPerShot;

        // Direção base
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 baseDir = ((Vector2)(mouseWorld - muzzle.position)).normalized;

        int pellets = Mathf.Max(1, cfg.pelletsPerShot);
        float half = cfg.spreadDegrees * 0.5f;

        for (int i = 0; i < pellets; i++)
        {
            float randAngle = (cfg.spreadDegrees <= 0f) ? 0f : Random.Range(-half, half);
            Vector2 dir = (Quaternion.Euler(0, 0, randAngle) * (Vector3)baseDir).normalized;

            RaycastHit2D hit = Physics2D.Raycast(muzzle.position, dir, cfg.range, hitMask);

            if (hit.collider)
            {
                var hp = hit.collider.GetComponentInParent<Health2D>();
                if (hp) hp.ApplyDamage(cfg.damage);
            }

            if (tracerPrefab)
                StartCoroutine(ShowTracer(hit, muzzle.position, dir, cfg.range, cfg.tracerTime));
        }
    }

    IEnumerator ShowTracer(RaycastHit2D hit, Vector3 origin, Vector2 dir, float range, float time)
    {
        var lr = Instantiate(tracerPrefab);
        Vector3 end = hit.collider ? (Vector3)hit.point : origin + (Vector3)(dir * range);
        lr.positionCount = 2;
        lr.SetPosition(0, origin);
        lr.SetPosition(1, end);
        lr.enabled = true;
        yield return new WaitForSeconds(time);
        if (lr) Destroy(lr.gameObject);
    }

    void TryStartReload()
    {
        var st = inventory.Current;
        if (st == null || reloading) return;
        if (st.clip >= st.config.clipSize) return;
        if (st.reserve <= 0) return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        reloading = true;
        float t0 = useUnscaledTime ? Time.unscaledTime : Time.time;
        float wait = inventory.Current.config.reloadTime;

        // (Aqui você pode chamar animação/som de reload)
        while ((useUnscaledTime ? Time.unscaledTime : Time.time) - t0 < wait)
            yield return null;

        var st = inventory.Current;
        int need = st.config.clipSize - st.clip;
        int take = Mathf.Min(need, st.reserve);
        st.clip += take;
        st.reserve -= take;

        reloading = false;
    }
}
