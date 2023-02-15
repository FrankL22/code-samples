using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    private int mTime;
    private Text mText;
    private Animator mAnim;

    private void Awake()
    {
        mText = GetComponent<Text>();
        mAnim = GetComponent<Animator>();
    }

    public void SetTime(int pTime)
    {
        mTime = pTime;
        mText.text = mTime.ToString();
        mAnim.enabled = true;
    }

    public void DecrementTime()
    {
        mTime--;
        if (mTime < 1)
        {
            mText.text = "GO!";
            mAnim.Play("Go");
            GameManager.IsPlaying = true;
        }
        else
        {
            mText.text = mTime.ToString();
        }
    }
}
