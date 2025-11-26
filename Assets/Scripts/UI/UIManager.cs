using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerController;
    private HeatManager heatManager;
    public Slider healthBar;
    public Slider heatMeter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        heatManager = player.GetComponent<HeatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = playerController.currentHealth/playerController.maxHealth;
        heatMeter.value = heatManager.currentHeat/heatManager.maxHeat;
    }
}
