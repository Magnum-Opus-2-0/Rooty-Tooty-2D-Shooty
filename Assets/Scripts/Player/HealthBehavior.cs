using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehavior : MonoBehaviour
{
    public SimpleHealthBar healthBar;
    public ExplodeBehavior explodeScript;

    public int maxHealth = 100;
    public int currentHealth;   // set to public for debugging

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


    public bool doesRespawn = true;





    // Start is called before the first frame update
    void Start()
    {
        respawnTime = TOTAL_RESPAWN_TIME;
        currentHealth = maxHealth;
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
            if (doesRespawn)
            {
                // If the GameObject this is attached to does respawn then we want to
                // show the black bar and how long they have to wait.
                respawnTime -= Time.fixedDeltaTime;
                respawn_background.gameObject.SetActive(true);
                respawnText.gameObject.SetActive(true);
                respawnText.text = "You're DEAD! Respawning in " + respawnTime.ToString("0");
            }
            else
            {
                // If the GameObject this is attached to does not respawn we don't want
                // to keep the black bar visible.
                respawn_background.gameObject.SetActive(false);
            }
        }

        if (respawnTime <= 0 && doesRespawn)
        {
            explodeScript.Restore();
            setHealth(maxHealth);

            respawn_background.gameObject.SetActive(false);
            respawnText.gameObject.SetActive(false);

            respawnTime = 5;
        }

        // We only want to show the health bar if the GameObjec this is attached
        // to has taken some damage.
        if (currentHealth < maxHealth)
        {
            respawn_background.transform.parent.gameObject.SetActive(true);
        }
        // Player canvases always need to be on whether they've taken damage or not
        // I should have used the TagManager here, but I'm not quite sure the
        // best way to reference it.
        else if (tag != "Player1_obj" && tag != "Player2_obj")
        {
            respawn_background.transform.parent.gameObject.SetActive(false);
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

        if (this.gameObject.tag == "P1_Base" || this.gameObject.tag == "P2_Base" ||
            this.gameObject.tag == "P1_Spawner" || this.gameObject.tag == "P2_Spawner" || 
            this.gameObject.tag == "P1_Turret" || this.gameObject.tag == "P2_Turret" ||
            this.gameObject.tag == "P1_Healer" || this.gameObject.tag == "P2_Healer" ||
            this.gameObject.tag == "P1_Minion" || this.gameObject.tag == "P2_Minion")
        {
            // Update the health bar amount
            healthBar.UpdateBar(currentHealth, maxHealth);

            // Update the health bar color
            if (currentHealth >= (0.5 * maxHealth))
                healthBar.UpdateColor(Color.green);
            else if (currentHealth >= (0.2 * maxHealth))
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

        if (value > maxHealth || value < 0) return false;



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

            if (currentHealth + delta <= maxHealth)
            {

                currentHealth += delta;
                changed = true;
            }
        }

        // Damage
        else if (delta < 0)
        {

            currentHealth += delta;
            changed = true;

            if (currentHealth < 0)
            {
                currentHealth = 0;
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
