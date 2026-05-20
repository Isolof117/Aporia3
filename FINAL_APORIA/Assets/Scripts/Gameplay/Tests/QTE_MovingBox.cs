using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class QTE_MovingBox : MonoBehaviour
{
    private WeaponBase currentWeapon;

    [SerializeField] private float moveSpeed = 2.0f;

    [SerializeField] private Slider slider;

    [SerializeField] private RectTransform goalRect;

    private float time = 0;

    public bool QTEActive = false;

    bool goalFound = true;

    [SerializeField] float[] widths = { 400.0f, 300.0f, 250.0f, 200.0f };

    private int widthPointer = 0;

    public AudioClip[] ReloadSounds;

    // Update is called once per frame
    void Update()
    {
        //if(!QTEActive)
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        //Display the QTE
        //        StartCoroutine(DoQTE());
        //    }
        //}
        //else
        //{
        if (QTEActive)
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (ISOverlappingUI(goalRect, slider.handleRect))
                {
                    Debug.Log("YOU HIT");

                    goalFound = true;

                    int soundIndex = widthPointer;

                    currentWeapon.Audio.PlayOneShot(ReloadSounds[soundIndex]);

                }
                else
                {
                    Debug.Log("YOU MISSED");
                }
            }

    }

   

    void SetNewGoalPosition() //Move the goal somewhere along the slider and set width
    {
        RectTransform sliderRect = slider.GetComponent<RectTransform>();

        goalRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widths[widthPointer]);
       
        float t = Random.Range(0.1f, 0.9f);

        float width = sliderRect.rect.width;

        float xPos = Mathf.Lerp(-width / 2f, width / 2f, t);

        goalRect.anchoredPosition = new Vector2(xPos, goalRect.anchoredPosition.y);
    }

    public IEnumerator DoQTE(WeaponBase weapon)
    {
        currentWeapon = weapon;
        QTEActive = true;
        slider.gameObject.SetActive(true);

        time = 0f;
        widthPointer = 0;

        for (int phases = 0; phases < widths.Length; phases++)
        {
            goalFound = false;

            widthPointer = phases;

            SetNewGoalPosition();

            yield return StartCoroutine(LerpSlider()); //Wait for goal to be found

        }

        //Hide QTE event

        QTEActive = false;
        weapon.Reload();
        slider.gameObject.SetActive(false);
        
    }
    IEnumerator LerpSlider()
    {
        //time = 0f;

        while (!goalFound)
        {
            time += Time.deltaTime * moveSpeed;

            float t = Mathf.PingPong(time, 1);

            slider.value = Mathf.Lerp(0, 1, t);

            yield return null;
        }
        
    }

    public bool ISOverlappingUI(RectTransform rectA , RectTransform rectB)
    {
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];

        rectA.GetWorldCorners(cornersA);
        rectB.GetWorldCorners(cornersB);

        Rect rect1 = new Rect(cornersA[0], cornersA[2] - cornersA[0]);
        Rect rect2 = new Rect(cornersB[0], cornersB[2] - cornersB[0]);

        return rect1.Overlaps(rect2);

    }

    
}
