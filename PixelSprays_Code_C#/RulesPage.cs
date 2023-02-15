using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesPage : MonoBehaviour
{
    private int mCurrPage = -1;
    private int mTotalPages = -1;
    private Transform mPages;
    private GameObject mPrevButton;
    private GameObject mNextButton;
    private Text mPageNum;

    private void Awake()
    {
        mPages = transform.Find("Pages");
        mTotalPages = mPages.childCount;
        mPrevButton = transform.Find("PrevPage").gameObject;
        mNextButton = transform.Find("NextPage").gameObject;
        mPageNum = transform.Find("PageNum").GetComponent<Text>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        GotoPage(0);
    }

    public void NextPage()
    {
        GotoPage(mCurrPage + 1);
    }

    public void PrevPage()
    {
        GotoPage(mCurrPage - 1);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void GotoPage(int pPage)
    {
        if (mCurrPage > -1)
        {
            mPages.GetChild(mCurrPage).gameObject.SetActive(false);
        }
        mPages.GetChild(pPage).gameObject.SetActive(true);
        mCurrPage = pPage;
        UpdatePageNum();
        UpdateButtons();
    }

    private void UpdatePageNum()
    {
        mPageNum.text = $"{mCurrPage + 1}/{mTotalPages}";
    }

    private void UpdateButtons()
    {
        if (mCurrPage == 0) mPrevButton.SetActive(false);
        else mPrevButton.SetActive(true);

        if (mCurrPage == mTotalPages - 1) mNextButton.SetActive(false);
        else mNextButton.SetActive(true);
    }
}
