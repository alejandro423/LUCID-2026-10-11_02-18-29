using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public BeatMusic beatMusic;
    public HazardSpawner spawner;
    public HazardFactory factory;
    public HUDController hud;
    public float trackBeats = 96f;
    public float checkpointBeat = 48f;
    public float shakeAmount = 0.25f;

    int state;
    bool checkpointReached;
    Vector3 camBase;
    float shake;

    void Start()
    {
        camBase = Camera.main.transform.position;
        player.OnDead += HandlePlayerDead;
        player.OnHit += () =>
        {
            shake = 1f;
            hud.SetHp(player.Hp, player.maxHp);
        };
        hud.SetHp(player.Hp, player.maxHp);
        hud.SetCheckpointMark(checkpointBeat / trackBeats);
    }

    void Update()
    {
        if (shake > 0f)
        {
            shake = Mathf.Max(0f, shake - Time.deltaTime * 3f);
            Camera.main.transform.position = camBase + (Vector3)(Random.insideUnitCircle * shake * shakeAmount);
            if (shake <= 0f)
                Camera.main.transform.position = camBase;
        }

        if (state == 0)
        {
            hud.SetProgress(Mathf.Clamp01(beatMusic.BeatTime / trackBeats));
            if (!checkpointReached && beatMusic.BeatTime >= checkpointBeat)
            {
                checkpointReached = true;
                hud.FlashMessage("CHECKPOINT");
            }
            if (beatMusic.BeatTime >= trackBeats)
                Win();
        }
        else if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void HandlePlayerDead()
    {
        if (state != 0)
            return;
        float target = checkpointReached ? checkpointBeat : 0f;
        ClearHazards();
        beatMusic.RestartAt(target);
        player.ResetPlayer();
        spawner.ResetPatterns(target);
        hud.FlashMessage(checkpointReached ? "CHECKPOINT" : "OTRA VEZ");
    }

    void Win()
    {
        state = 1;
        spawner.StopAllCoroutines();
        spawner.enabled = false;
        ClearHazards();
        hud.ShowEnd(true);
    }

    void ClearHazards()
    {
        if (factory != null && factory.container != null)
            foreach (Transform child in factory.container)
                Destroy(child.gameObject);
    }
}
