using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private GameController gameController;

	[System.Serializable]
	public class PlayerStats {
        private int _curHealth;
        
        public int maxHealth = 10;

		public int curHealth
		{
			get { return _curHealth; }
			set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
		}

		public void Init()
		{
			curHealth = maxHealth;
		}

        public void Init(PlayerStats lastStats)
        {
            curHealth = lastStats.curHealth;
            maxHealth = lastStats.maxHealth;
        }
    }

	public PlayerStats stats = new PlayerStats();

    public CharacterController2D characterController2d;

    public Transform sceneInitialSpawnPoint;
    public Transform lastSpawnPoint;
    public float spawnDelay = 2f;

    //public int fallBoundary = -20;

	[SerializeField]
	private StatusIndicator statusIndicator;


    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameCtrl").GetComponent<GameController>();
        characterController2d = GetComponent<CharacterController2D>();
    }

    void Start()
	{
		stats.Init();

		if (statusIndicator == null)
		{
			Debug.LogError("No status indicator referenced on Player");
		}
		else
		{
			statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
		}

        // Change player's pos based on entrance if lastLoadedScene is not null. 
        if(!string.IsNullOrEmpty(gameController.lastLoadedScene))
        {
            Debug.Log(gameController.lastLoadedScene);
            sceneInitialSpawnPoint = GameObject.FindGameObjectWithTag(gameController.lastLoadedScene + "_sp").transform;
            transform.position = sceneInitialSpawnPoint.position;
        }
        
        if(sceneInitialSpawnPoint == null)
        {
            GameObject copyInitialTransform = new GameObject();
            copyInitialTransform.transform.position = transform.position;
            sceneInitialSpawnPoint = copyInitialTransform.transform;
        }

        lastSpawnPoint = sceneInitialSpawnPoint;
    }

	void Update () {
        // Update HUD Health status
        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
        

    }

	public void Damage (int damage, bool respawn = false) {
        Debug.Log(string.Format("Player get {0} damage", damage));
        stats.curHealth -= damage;
        if (stats.curHealth <= 0)
        {
            // ToDo Let player choose which reset the level
            characterController2d.PlayDeathAnimation();
            StartCoroutine(RespawnPlayer(sceneInitialSpawnPoint));

            // ToDo: reset the level
            // Maybe calling gameMaster.resetLevel() ?

            // Reset current health of the player.
            // ToDo: obtain a copy of Player.stats from last scene.
            stats.Init();
        }
        else
        {

            characterController2d.PlayHitAnimation();
            if (respawn)
                StartCoroutine(RespawnPlayer(lastSpawnPoint));
        }
    }

    public IEnumerator RespawnPlayer (Transform spawnPoint)
    {
        characterController2d.enableControl = false;

        yield return new WaitForSeconds(spawnDelay);

        transform.position = spawnPoint.position;

        //TODO: Add Spawn FX

        characterController2d.enableControl = true;
    }
}
