using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("Doors (assign 7)")]
    [SerializeField] private Transform[] doors;
    [SerializeField] private EnemySentry[] sentries;

    [SerializeField] private float doorMoveAmount = 10f;
    [SerializeField] private float doorMoveSpeed = 2f;

    [Header("Enemy Setup")]
    [SerializeField] private GameObject enemyTemplate;
    [SerializeField] private float spawnOffsetBehindDoor = 2f;

    [Header("NavMesh")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("Wave Text")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private float textFadeDuration = 1f;

    private int currentWave = 1;

    // Tracks spawned enemy HOLDERS
    private readonly List<GameObject> spawnedEnemies =
        new List<GameObject>();

    //Track Sentries
    private int activeSentryCount = 0;

    // Stores currently opened doors
    private int[] currentOpenDoors;

    // Original closed positions
    private Vector3[] closedDoorPositions;

    private bool waveActive = false;
    private bool startingNextWave = false;

    private void Start()
    {
        if (waveText != null)
        {
            waveText.alpha = 0f;
        }

        // Save original door positions
        closedDoorPositions =
            new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] != null)
            {
                closedDoorPositions[i] =
                    doors[i].position;
            }
        }

        // Start Wave 1
        StartCoroutine(BeginWave(1, 2, 0));
    }

    private void OnEnable()
    {
        EnemySentry.OnSentryDeactivated += HandleSentryDeactivated;
    }
    private void OnDisable()
    {
        EnemySentry.OnSentryDeactivated -= HandleSentryDeactivated;
    }

    private void HandleSentryDeactivated(EnemySentry sentry)
    {
        activeSentryCount = Mathf.Max(0, activeSentryCount - 1);
    }
    private void Update()
    {
        // Remove entries whose Enemy-tagged child is gone
        spawnedEnemies.RemoveAll(enemy =>
        {
            if (enemy == null)
                return true;

            Transform[] children =
                enemy.GetComponentsInChildren<Transform>();

            foreach (Transform child in children)
            {
                if (child.CompareTag("Enemy"))
                {
                    return false;
                }
            }

            return true;
        });

        // No checks if wave inactive
        if (!waveActive)
            return;

        // Start next wave when all enemies dead
        if (spawnedEnemies.Count == 0 && activeSentryCount == 0 && !startingNextWave)
        {
            waveActive = false;
            startingNextWave = true;

            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        // CLOSE current doors first
        if (currentOpenDoors != null)
        {
            yield return StartCoroutine(
                MoveDoorsDown(currentOpenDoors)
            );

            // Rebuild NavMesh after closing
            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
            }
        }

        yield return new WaitForSeconds(3f);

        currentWave++;

        // Increase doors each wave
        int doorCount =
            Mathf.Clamp(currentWave + 1, 2, doors.Length);

        int sentryCount = Mathf.Clamp(currentWave / 2, 0, sentries.Length); // 0 , 1, 1, 2, 2, 3, 3

        yield return StartCoroutine(
            BeginWave(currentWave, doorCount, sentryCount)
        );

        startingNextWave = false;
    }

    private IEnumerator BeginWave(
        int waveNumber,
        int doorCount, int sentryCount)
    {
        // Set wave text
        if (waveText != null)
        {
            switch (waveNumber)
            {
                case 1:
                    waveText.text = "WAVE ONE";
                    break;

                case 2:
                    waveText.text = "WAVE TWO";
                    break;

                case 3:
                    waveText.text = "WAVE THREE";
                    break;

                case 4:
                    waveText.text = "WAVE FOUR";
                    break;

                case 5:
                    waveText.text = "WAVE FIVE";
                    break;

                default:
                    waveText.text =
                        "WAVE " + waveNumber;
                    break;
            }
        }

        // Delay before starting
        yield return new WaitForSeconds(5f);

        // Pick unique random doors
        int[] selectedDoors =
            GetUniqueRandomDoors(doorCount);

        // Store open doors
        currentOpenDoors = selectedDoors;

        // Spawn enemies
        SpawnEnemies(selectedDoors);

        //Activate any sentries

        ActivateSentries(sentryCount);

        // Wave now active
        waveActive = true;

        // Open selected doors
        yield return StartCoroutine(
            MoveDoorsUp(selectedDoors)
        );

        // Update NavMesh
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        // Show UI text
        StartCoroutine(ShowWaveText());
    }

    private void SpawnEnemies(int[] activeDoors)
    {
        if (enemyTemplate == null || doors == null)
            return;

        foreach (int i in activeDoors)
        {
            if (i < 0 || i >= doors.Length)
                continue;

            if (doors[i] == null)
                continue;

            Transform door = doors[i];

            Vector3 spawnPos =
                door.position -
                door.forward * spawnOffsetBehindDoor;

            GameObject clone = Instantiate(
                enemyTemplate,
                spawnPos,
                enemyTemplate.transform.rotation
            );

            spawnedEnemies.Add(clone);
        }
    }

    void ActivateSentries(int amount)
    {
        amount = Mathf.Clamp(amount, 0, sentries.Length);

        for(int i = 0; i < amount; i++)
        {
            int rand = Random.Range(0, sentries.Length);

            if(!sentries[rand].isActive)
            {
                sentries[rand].Activate();
                activeSentryCount++; 
            }
            else
            {
                i--;
            }
        }
    }

    private int[] GetUniqueRandomDoors(int amount)
    {
        amount =
            Mathf.Clamp(amount, 1, doors.Length);

        List<int> available =
            new List<int>();

        for (int i = 0; i < doors.Length; i++)
        {
            available.Add(i);
        }

        int[] selected = new int[amount];

        for (int i = 0; i < amount; i++)
        {
            int randomIndex =
                Random.Range(0, available.Count);

            selected[i] =
                available[randomIndex];

            // Prevent duplicates
            available.RemoveAt(randomIndex);
        }

        return selected;
    }

    private IEnumerator MoveDoorsUp(int[] activeDoors)
    {
        float t = 0f;

        Vector3[] startPositions =
            new Vector3[doors.Length];

        Vector3[] targetPositions =
            new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] == null)
                continue;

            startPositions[i] =
                doors[i].position;

            targetPositions[i] =
                doors[i].position;
        }

        foreach (int i in activeDoors)
        {
            if (i < 0 || i >= doors.Length)
                continue;

            targetPositions[i] =
                closedDoorPositions[i] +
                Vector3.up * doorMoveAmount;
        }

        while (t < 1f)
        {
            t +=
                Time.deltaTime *
                doorMoveSpeed;

            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] == null)
                    continue;

                doors[i].position =
                    Vector3.Lerp(
                        startPositions[i],
                        targetPositions[i],
                        t
                    );
            }

            yield return null;
        }
    }

    private IEnumerator MoveDoorsDown(int[] activeDoors)
    {
        float t = 0f;

        Vector3[] startPositions =
            new Vector3[doors.Length];

        Vector3[] targetPositions =
            new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] == null)
                continue;

            startPositions[i] =
                doors[i].position;

            targetPositions[i] =
                doors[i].position;
        }

        foreach (int i in activeDoors)
        {
            if (i < 0 || i >= doors.Length)
                continue;

            targetPositions[i] =
                closedDoorPositions[i];
        }

        while (t < 1f)
        {
            t +=
                Time.deltaTime *
                doorMoveSpeed;

            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] == null)
                    continue;

                doors[i].position =
                    Vector3.Lerp(
                        startPositions[i],
                        targetPositions[i],
                        t
                    );
            }

            yield return null;
        }
    }

    private IEnumerator ShowWaveText()
    {
        if (waveText == null)
            yield break;

        float t = 0f;

        // Fade in
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;

            waveText.alpha =
                Mathf.Lerp(
                    0f,
                    1f,
                    t / textFadeDuration
                );

            yield return null;
        }

        waveText.alpha = 1f;

        yield return new WaitForSeconds(0.5f);

        t = 0f;

        // Fade out
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;

            waveText.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    t / textFadeDuration
                );

            yield return null;
        }

        waveText.alpha = 0f;
    }
}