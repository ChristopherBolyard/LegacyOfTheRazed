using UnityEngine;
using UnityEngine.VFX;

public class CombatSystem : MonoBehaviour
{
    public VisualEffect emberStrikeVFX;
    public Transform vfxSpawnPoint;
    public float emberStrikeDamage = 35f;
    public float emberStrikeCooldown = 1.8f;
    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
    }

    public bool CanUseEmberStrike() => cooldownTimer <= 0;

    public void CastEmberStrike(Transform target)
    {
        if (!CanUseEmberStrike()) return;

        cooldownTimer = emberStrikeCooldown;

        // Damage
        var health = target.GetComponent<EnemyHealth>();
        if (health) health.TakeDamage(emberStrikeDamage);

        // VFX
        if (emberStrikeVFX && vfxSpawnPoint)
        {
            var vfx = Instantiate(emberStrikeVFX, vfxSpawnPoint.position, Quaternion.LookRotation(target.position - vfxSpawnPoint.position));
            Destroy(vfx.gameObject, 3f);
        }

        NotificationSystem.Instance.Show($"Ember Strike! {emberStrikeDamage} dmg");
    }
}
