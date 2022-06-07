using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    private Animator anim;
    public float currentHealth { get; private set; }
    private bool dead;

    [Header("iFrames")]
    [SerializeField]  private float iFramesDuration;
    [SerializeField]  private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField]private Behaviour[] components;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private bool invulnerable;


    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {   if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
            SoundManager.instance.PlaySound(hurtSound);

        }
        else
        {
            if (!dead) { 
   

    
                

                anim.SetBool("grounded", true);
                anim.SetTrigger("die");
                

                dead = true;
                SoundManager.instance.PlaySound(deathSound);
                //Deactivate all attached components
                foreach (Behaviour component in components)
                    component.enabled = false;
            }
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("idle");
        StartCoroutine(Invunerability());


        //Deactivate all attached components
        foreach (Behaviour component in components)
            component.enabled = true;

    }
    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10,11,true);
        //Invunerability duration
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration/ (numberOfFlashes*2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false; ;


    }
    private void Deactivate()
    {
        gameObject.SetActive(false); 
    }
}
