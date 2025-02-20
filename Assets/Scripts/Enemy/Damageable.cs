using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    protected float maxHealth = 100;

    [SerializeField]
    protected float health = 100;
    protected bool IsAlive = true;
    public bool isAlive => IsAlive;

    private Color[] originalColors;
    private Color damageColor = Color.red;
    public float damageEffectDuration = 0.5f;
#pragma warning disable CS0108
    private Renderer renderer;
#pragma warning restore CS0108

    protected virtual void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            originalColors = new Color[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                originalColors[i] = materials[i].color;
            }
        }

        health = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        //Debug.Log("Hit");
        health -= damage;

        if (renderer != null)
        {
            StartCoroutine(DamageEffect());
        }

        if (health <= 0 && IsAlive)
        {
            IsAlive = false;
            OnDestroyed();
        }
    }

    public virtual void Heal(float healAmount)
    {

        health = Mathf.Min(health + healAmount, maxHealth);
        Debug.Log("Healed " + healAmount + " HP. Current Health: " + health);
    }

    protected virtual void OnDestroyed()
    {
        Destroy(gameObject);
    }

    private IEnumerator DamageEffect()
    {
        if (renderer == null) yield break;

        Material[] materials = renderer.materials;

        foreach (var material in materials)
        {
            material.color = damageColor;
        }

        float elapsedTime = 0f;
        while (elapsedTime < damageEffectDuration)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = Color.Lerp(damageColor, originalColors[i], elapsedTime / damageEffectDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
    }

}
