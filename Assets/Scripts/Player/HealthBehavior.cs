﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehavior : MonoBehaviour
{
    public SimpleHealthBar healthBar;
    public ExplodeBehavior explodeScript;
    public ShootController shootScript;

    public const int MAX_HEALTH = 100;
    public int currentHealth;   // set to public for debugging

    public int bulletDamage = 10;

    // Variables for demo poison function
    public bool doPoisonDemo;   // this is initialized via the Unity editor
    private const float POISON_PERIOD = 0.25f; // use this value only for testing
    private const int POISON_DAMAGE = 20;
    private float currentTimeStep;
    public Image health_icon;
    public Image respawn_background;
    public Text respawnText;
    public Text health_val;

    public const float TOTAL_RESPAWN_TIME = 5.0f;
    public float respawnTime;






    // Start is called before the first frame update
    void Start()
    {
        respawnTime = TOTAL_RESPAWN_TIME;
        currentHealth = MAX_HEALTH;
        currentTimeStep = 0;
        // health_icon.fillAmount = 1.0f;
        // health_val.text = currentHealth.ToString();
        // respawnText.gameObject.SetActive(false);
        // respawn_background.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // This is a demo function;
        // replace with an actual damage calculation in real usage
        if (doPoisonDemo) update_PoisonDamage();
    }

    void FixedUpdate()
    {
        if (!isNotDead() && respawnTime == TOTAL_RESPAWN_TIME)
        {
            explodeScript.Explode();
        }

        if (!isNotDead() && respawnTime <= TOTAL_RESPAWN_TIME)
        {
            respawnTime -= Time.fixedDeltaTime;
            respawn_background.gameObject.SetActive(true);
            respawnText.gameObject.SetActive(true);
            respawnText.text = "You're DEAD! Respawning in " + respawnTime.ToString("0");
        }

        if (respawnTime <= 0)
        {
            explodeScript.Restore();
            setHealth(MAX_HEALTH);

            respawn_background.gameObject.SetActive(false);
            respawnText.gameObject.SetActive(false);

            respawnTime = 5;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        /// Bullet collision isn't handled here anymore, now handled by <see cref="BulletController.OnCollisionEnter(Collision)"/>.
    }

    /// <summary>
    /// Test function for taking damage.
    /// The runner of this script takes damage every so often,
    /// specified by POISON_PERIOD.
    /// Once the player's health fully depletes,
    /// they are marked as dead (i.e. isAlive == false).
    /// </summary>
    private void update_PoisonDamage()
    {

        currentTimeStep += Time.deltaTime;

        if (currentTimeStep >= POISON_PERIOD)
        {

            bool wasAlive = isNotDead();

            adjustHealth(-POISON_DAMAGE);

            // Explode if you just died
            if (wasAlive && !isNotDead())
                explodeScript.Explode();

            // Reset current time step
            currentTimeStep = 0;
        }

        // Press space to reset
        if (Input.GetKeyDown(KeyCode.Space))
        {

            Start();
            explodeScript.Restore();
        }
    }

    /// <summary>
    /// Updates health bar with current health,
    /// and sets fillAmount accordingly.
    /// </summary>
    public void updateHealthBar()
    {
        if (this.gameObject.tag == "Player1_obj" || this.gameObject.tag == "Player2_obj")
        {
            health_val.text = currentHealth.ToString();
            health_icon.fillAmount = currentHealth / 100.0f;
        }

        if (this.gameObject.tag == "P1_Base" || this.gameObject.tag == "P2_Base")
        {
            // Update the health bar amount
            healthBar.UpdateBar(currentHealth, MAX_HEALTH);

            // Update the health bar color
            if (currentHealth >= (0.5 * MAX_HEALTH))
                healthBar.UpdateColor(Color.green);
            else if (currentHealth >= (0.2 * MAX_HEALTH))
                healthBar.UpdateColor(Color.yellow);
            else
                healthBar.UpdateColor(Color.red);
        }
    }

    /// <summary>
    /// Sets health to a value between 0 and MAX_HEALTH, inclusive.
    /// </summary>
    /// <param name="value">The desired value for the health</param>
    /// <returns>True if the health successfully changed, false otherwise</returns>
    public bool setHealth(int value)
    {

        if (value == currentHealth) return false;

        if (value > MAX_HEALTH || value < 0) return false;



        currentHealth = value;
        updateHealthBar();

        return true;
    }

    /// <summary>
    /// Adjusts health. Can heal or take damage.
    /// </summary>
    /// <param name="delta">Positive for healing, negative for damage</param>
    /// <returns>True if this function call changed the health, false otherwise</returns>
    public bool adjustHealth(int delta)
    {

        bool changed = false;

        // Healing
        if (delta > 0)
        {

            if (currentHealth + delta <= MAX_HEALTH)
            {

                currentHealth += delta;
                changed = true;
            }
        }

        // Damage
        else if (delta < 0)
        {

            if (currentHealth + delta >= 0)
            {

                currentHealth += delta;
                changed = true;
            }
        }

        // Avoid doing anything if possible
        if (!changed) return false;

        updateHealthBar();

        return true;
    }

    /// <summary>
    /// Tests whether the object running this script is alive.
    /// A "dead" object is one whose health has depleted to 0.
    /// An "alive" object is one whose health is greater than 0.
    /// </summary>
    /// <returns>True if alive, false if dead</returns>
    public bool isNotDead()
    {
        return currentHealth > 0;
    }

    /// <summary>
    /// Accessor function for this object's current health,
    /// on a scale from 0 to MAX_HEALTH.
    /// </summary>
    /// <returns>This object's current health</returns>
    public int getCurrentHealth()
    {
        return currentHealth;
    }
}
