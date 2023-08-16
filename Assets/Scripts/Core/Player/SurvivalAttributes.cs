using UnityEngine;

public class SurvivalAttributes : MonoBehaviour
{
    public VisualHealth VisualHealth;
    public Stammina Stammina;
    public Hungry Hungry;
    public Thirst Thirst;

    private void Start()
    {
        Hungry.OnHungry += TakeDamage;
        Thirst.OnThirst += TakeDamage;
    }

    private void TakeDamage(float damage)
    {
        VisualHealth.Health.TakeDamage(damage);
    }

    private void OnDestroy()
    {
        Hungry.OnHungry -= TakeDamage;
        Thirst.OnThirst -= TakeDamage;
    }
}
