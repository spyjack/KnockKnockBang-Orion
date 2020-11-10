using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    PlayerMovement playerMover = null;
    [SerializeField]
    PlayerLook playerLooker = null;
    [SerializeField]
    Animator playerAnimator = null;

    [SerializeField]
    Image healthBar = null;
    [SerializeField]
    Text killsTotalText = null;

    [SerializeField]
    GameObject nextLevelScreen = null;
    [SerializeField]
    GameObject failureScreen = null;

    [SerializeField]
    float maxHealth = 10;
    [SerializeField]
    float currentHealth = 10;

    [SerializeField]
    int enemiesToKill = 1;
    int enemiesKilled = 0;

    bool isDead = false;

    public PlayerMovement Mover
    {
        get { return playerMover; }
    }
    public PlayerLook Looker
    {
        get { return playerLooker; }
    }
    public Animator Animator
    {
        get { return playerAnimator; }
    }
    public bool Dead
    {
        get { return isDead; }
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        enemiesToKill = FindObjectsOfType<EnemyController>().Length;
        UpdateKillsLeft(0);
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void UpdateKillsLeft(int _kills = -1)
    {
        if(_kills != -1)
        {
            enemiesKilled += _kills;
            killsTotalText.text = enemiesKilled.ToString() + "/" + enemiesToKill.ToString() + " Killed";
        }

        if (enemiesKilled >= enemiesToKill)
        {
            nextLevelScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - _damage);
        healthBar.fillAmount = (currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            print("Died");
            isDead = true;
            failureScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Heal(float _amount)
    {
        currentHealth = Mathf.Min(currentHealth + _amount, maxHealth);
        healthBar.fillAmount = (currentHealth / maxHealth);
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string _level)
    {
        SceneManager.LoadScene(_level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
