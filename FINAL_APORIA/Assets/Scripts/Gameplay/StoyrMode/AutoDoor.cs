using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(AudioSource))]
public class AutoDoor : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float openDistance = 2f;
    public Transform detectionPoint;

    [Header("Door Movement")]
    public float slideAmount = 5f;
    public float moveSpeed = 2f;

    [Header("NavMesh")]
    public NavMeshSurface navMeshSurface;

    [Header("Door Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;

    [Header("Music Switch")]
    public AudioSource musicSource;
    public AudioClip newMusic;
    public float musicFadeTime = 2f;

    private AudioSource audioSource;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private enum DoorState { Closed, Open, Transitioning }
    private DoorState state = DoorState.Closed;

    private bool musicSwitched = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * slideAmount;

        if (detectionPoint == null)
            detectionPoint = transform;
    }

    void Update()
    {
        if (player == null) return;

        if (state == DoorState.Transitioning)
            return;

        float distance = Vector3.Distance(detectionPoint.position, player.position);

        if (distance <= openDistance && state == DoorState.Closed)
        {
            StartCoroutine(OpenDoor());
        }
        else if (distance > openDistance && state == DoorState.Open)
        {
            StartCoroutine(CloseDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        state = DoorState.Transitioning;

        if (openSound != null)
            audioSource.PlayOneShot(openSound);

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();

        if (!musicSwitched && musicSource != null && newMusic != null)
        {
            musicSwitched = true;
            StartCoroutine(FadeToNewMusic());
        }

        while ((transform.position - openPosition).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                openPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = openPosition;
        state = DoorState.Open;
    }

    IEnumerator CloseDoor()
    {
        state = DoorState.Transitioning;

        if (closeSound != null)
            audioSource.PlayOneShot(closeSound);

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();

        while ((transform.position - closedPosition).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                closedPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = closedPosition;
        state = DoorState.Closed;
    }

    IEnumerator FadeToNewMusic()
    {
        float startVolume = musicSource.volume;
        float t = 0f;

        while (t < musicFadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / musicFadeTime);
            yield return null;
        }

        musicSource.clip = newMusic;
        musicSource.Play();

        t = 0f;

        while (t < musicFadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / musicFadeTime);
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}