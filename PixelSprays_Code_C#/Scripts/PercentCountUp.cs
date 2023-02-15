using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PercentCountUp : MonoBehaviour
{
    private float mCountTo = 0;
    private float mCurrent = 0;
    private float mCountSpeed = 10;

    private Text mText;

    [SerializeField] private string Prefix = "";

    private void Awake()
    {
        mText = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        if (mCurrent == mCountTo) return;
        mCurrent += mCountSpeed * Time.fixedDeltaTime;
        if (mCurrent > mCountTo) mCurrent = mCountTo;
        UpdateText();
    }

    public void CountTo(float pCountTo, float pCountTime)
    {
        mCountTo = pCountTo;
        mCountSpeed = pCountTo/pCountTime;
    }

    private void UpdateText()
    {
        mText.text = $"{Prefix}\n{mCurrent.ToString("P1")}";
    }
}
