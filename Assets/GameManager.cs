using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    [SerializeField] bool isTutorial;
    private void OnEnable()
    {
        Instance = this;
    }

    public bool TestTransition; 

    [SerializeField] float Timer;
    public int sectorCurrent;
    public int sectorObjective = 70;
    bool isGameOver;
    bool isGoalReached;
    [SerializeField] Transform GemChamber;
    public bool ClearPathway;

    [Header("UI")]
    public Transform BlackBG;
    public Image parryEffect;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI sectorCounterText;
    public bool doTimer;

    [Header("Finishing")]
    [SerializeField] GameObject DeactivatePanel;
    [SerializeField] bool FinishingMode;
    [SerializeField] bool FinishingModeFirstFrame;
    [SerializeField] bool FinishingModeWin;
    [SerializeField] Transform DeactivateBar;
    [SerializeField] float deactivateValue;
    [SerializeField] float deactivateMax = 100;

    [Header("Utility")]
    [SerializeField] AudioManager aud;

    public void DoParryEffect()
    {
        StartCoroutine(ParryEffect());
    }

    IEnumerator ParryEffect()
    {
        aud.PlaySFX(aud.s_parried);
        parryEffect.gameObject.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.3f);
        parryEffect.gameObject.SetActive(false);
        Time.timeScale = 1;
    }


    private void Start()
    {
        aud = AudioManager.Instance;
        if (!isTutorial) aud.PlayBGM(aud.m_dstorv);
    }


    private void Update()
    {
        


        if (TestTransition)
        {
            GoToGemChamber();
            TestTransition = false;
        }


        if (sectorCurrent>=sectorObjective && !isGoalReached)
        {
            GoToGemChamber();
            PlayerControl.Instance.enabled = false;
            isGoalReached = true;
        }

        if (!doTimer) return;
        if (0>Timer && !isGameOver)
        {
            PlayerControl.Instance.enabled = false;
            DeactivatePanel.SetActive(false);
            doTimer = false;
            BlackBG.gameObject.LeanColor(new(0, 0, 1, 0), 0).setOnComplete(() =>
            {
                BlackBG.LeanMoveX(Screen.width / 2, 0).setOnComplete(() =>
                {
                    BlackBG.gameObject.LeanColor(new(0, 0, 1, 1), 1).setOnComplete(() =>
                    {
                        SceneManager.LoadScene("GameOver");
                    });
                });
            });
            isGameOver = true;
        }
        else if (0<Timer)
        {
            if (!isTutorial) Timer -= Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(Timer);
            if (timerText!=null) timerText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            if (!isGoalReached && timerText!=null) sectorCounterText.text = $"{sectorObjective - sectorCurrent} Sectors to go";
        }

        if (FinishingMode)
        {
            DeactivateBar.transform.localScale = new(deactivateValue/deactivateMax,
                DeactivateBar.transform.localScale.y,
                DeactivateBar.transform.localScale.z);

            if (deactivateValue>0) deactivateValue-=Time.deltaTime*10;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                deactivateValue += 2;
                if (deactivateValue>deactivateMax && !FinishingModeWin)
                {
                    DeactivatePanel.SetActive(false);
                    doTimer = false;
                    BlackBG.LeanMoveY(Screen.height / 2, 0).setOnComplete(() =>
                    {
                        BlackBG.LeanMoveX(3 * (Screen.width / 2), 0).setOnComplete(() =>
                        {
                            BlackBG.LeanMoveX(Screen.width / 2, 0.5f).setEaseOutQuint().setOnComplete(() =>
                            {
                                SceneManager.LoadScene("GameOver");
                            });
                        });
                    });
                    FinishingModeWin = true;
                }
            }

            if (!FinishingModeFirstFrame)
            {
                aud.PlaySFX(aud.s_warning);
                DeactivatePanel.SetActive(true);
                FinishingModeFirstFrame = true;
            }
        }
    }


    void GoToGemChamber()
    {
        doTimer = false;
        BlackBG.LeanMoveY(Screen.height/2, 0).setOnComplete(() =>
        {
            BlackBG.LeanMoveX(3*(Screen.width/2), 0).setOnComplete(() =>
            {
                BlackBG.LeanMoveX(Screen.width / 2, 0.5f).setEaseOutQuint().setOnComplete(() =>
                {
                    FinishingMode = true;
                    sectorCounterText.text = "SPAM SPACE TO STOP";
                    PlayerControl.Instance.transform.position = GemChamber.position;
                    PlayerControl.Instance.StaminaPanel.SetActive(false);
                    Camera.main.GetComponent<Follow>().doFollow = false;
                    Camera.main.transform.position = new(GemChamber.position.x, GemChamber.position.y, -10);
                    for (int i = Camera.main.transform.childCount - 1; i >= 0; i--) Destroy(Camera.main.transform.GetChild(i).gameObject);
                    BlackBG.LeanMoveX(-(Screen.width / 2), 0.5f).setDelay(0.5f).setEaseOutQuint().setOnComplete(() =>
                    {
                        doTimer = true;
                    });
                });
            });
        });
    }
}