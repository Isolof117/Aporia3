using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SkillCheck : MonoBehaviour
{
    // public static SkillCheck Instance { get; set; }

    private WeaponBase currentWeapon;
    public AudioClip[] ReloadSounds;
    public TextMeshProUGUI Sequence;
    private List<KeyCode> keySequence = new List<KeyCode>();
    private KeyCode[] Exceptions =
        {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.R, KeyCode.Space
        };
    private KeyCode[] possibleKeys =
        {
            KeyCode.L, KeyCode.K, KeyCode.J, KeyCode.I
        };
    public int currentKey = 0;
    public bool isVisible;

    // Start is called before the first frame update
    //private void Awake()
    //{
    //    Instance = this;
    //}

    public void StartQTE(WeaponBase weapon)
    {
        currentWeapon = weapon;
        FillKeyCodes();
        DisplaySequence();

        currentKey = 0;
        isVisible = true;        
    }

    // Update is called once per frame
    void Update()
    {
        if (Sequence != null && isVisible)
        {
            if (Input.anyKeyDown)
            {
                KeyCode pressedKey = GetPressedKey();

                // Ignore keys that are not part of the QTE system
                if (!possibleKeys.Contains(pressedKey))
                    return;

                if (pressedKey == keySequence[currentKey])
                {
                    // Implement for reload sound per keycode

                    int soundIndex = currentKey;

                    currentWeapon.ReloadAudio.PlayOneShot(ReloadSounds[soundIndex]);

                    currentKey++;

                    if (currentKey >= keySequence.Count)
                    {
                        CompletedQTE();
                    }
                }
                else
                {
                    FailedQTE();
                    Debug.Log("QTE Failed");
                }
            }
        }
    }

    void FillKeyCodes()
    {
        keySequence.Clear();

        for(int i = 0; i < possibleKeys.Length; i++)
        {
            keySequence.Add(possibleKeys[Random.Range(0, possibleKeys.Length)]);
        }
    }

    KeyCode GetPressedKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
                return key;
        }
        return KeyCode.None;
    }

    void DisplaySequence()
    {
        Sequence.gameObject.SetActive(true);
        Sequence.text = string.Join(" ", keySequence);
    }

    void CompletedQTE()
    {
        isVisible = false;
        Sequence.gameObject.SetActive(false);

        Debug.Log("QTE Success");
        currentWeapon.Reload();
    }

    void FailedQTE()
    {
        isVisible = false;
        Sequence.gameObject.SetActive(false);

        Debug.Log("QTE Failed");
        currentWeapon.isReloading = false;
    }
}

