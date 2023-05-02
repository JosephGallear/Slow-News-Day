using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Stories
{
    public Speech[] stories;
}

[System.Serializable]
public class Responses
{
    public Speech[] responses;
}

public class Manager : MonoBehaviour
{
    [HideInInspector] public static Manager Instance { get; private set; }

    [SerializeField] private Stories[] storyList;

    [SerializeField] private Responses[] responseList;

    [SerializeField] private TextMeshProUGUI speechText;

    [SerializeField] private Card[] cards;

    private List<Speech>[] storyPools = new List<Speech>[2];
    private List<Speech>[] responsePools = new List<Speech>[3];

    private Speech currentStory;

    private int roundIndex;

    private int failurePoints;
    [SerializeField] private Image failureMeter;

    [SerializeField] private Transform moustacheTransform;

    [SerializeField] private GameObject dayScreen;
    [SerializeField] private GameObject[] dayTitles;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [HideInInspector] public bool canPause;

    [SerializeField] private AudioSource introSound;
    [SerializeField] private AudioSource outroSound;
    [SerializeField] private AudioSource dialogueSource;

    [SerializeField] private AudioClip[] dialogueClips;

    private bool canSelect;

    private int storyCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canPause = false;
        canSelect = false;
        roundIndex = 0;
        failurePoints = 0;
        failureMeter.fillAmount = 0;

        for (int i = 0; i < responsePools.Length; i++)
        {
            responsePools[i] = new List<Speech>();
            responsePools[i].AddRange(responseList[i].responses);
        }

        for (int i = 0; i < storyPools.Length; i++)
        {
            storyPools[i] = new List<Speech>();
            storyPools[i].AddRange(storyList[i].stories);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.Instance.SetVolume(0.3f);

        NextRound();

        StartCoroutine(DisplayDay());
    }

    public void NextStory()
    {
        if (storyCount >= (roundIndex + 3))
        {
            roundIndex++;

            if (roundIndex >= 3)
            {
                canPause = false;

                winScreen.SetActive(true);

                MusicPlayer.Instance.StopMusic();
                outroSound.Play();

                return;
            }

            NextRound();

            StartCoroutine(DisplayDay());
            return;
        }

        storyCount++;

        int storyIndex = 0;

        switch (roundIndex)
        {
            case 0:
                storyIndex = 0;
                break;

            case 1:
                storyIndex = Random.Range(0, 2);
                break;

            case 2:
                storyIndex = 1;
                break;
        }

        if (storyPools[storyIndex].Count <= 0)
        {
            storyPools[storyIndex].AddRange(storyList[storyIndex].stories);
        }

        int rnd = Random.Range(0, storyPools[storyIndex].Count);

        currentStory = storyPools[storyIndex][rnd];
        storyPools[storyIndex].RemoveAt(rnd);

        StartCoroutine(DisplayStory(currentStory.speechText));
    }

    public void NextRound()
    {
        storyCount = 0;

        for (int i = 0; i < dayTitles.Length; i++)
        {
            if (i == roundIndex)
            {
                dayTitles[i].SetActive(true);
            }
            else
            {
                dayTitles[i].SetActive(false);
            }
        }

        int neutralIndex = Random.Range(0, cards.Length);

        for (int i = 0; i < cards.Length; i++)
        {
            int positivityIndex = (i == neutralIndex) ? 1 : (Random.Range(0, 2) * 2);

            if (responsePools[positivityIndex].Count <= 0)
            {
                responsePools[positivityIndex].AddRange(responseList[positivityIndex].responses);
            }

            int rnd = Random.Range(0, responsePools[positivityIndex].Count);

            cards[i].SetResponse(responsePools[positivityIndex][rnd]);
            responsePools[positivityIndex].RemoveAt(rnd);

            cards[i].gameObject.SetActive(true);
            cards[i].ScaleDown();
        }
    }

    public void UpdateFailure()
    {
        failurePoints++;
        failureMeter.fillAmount = failurePoints / 5f;
    }

    public void CardClicked(int cardIndex)
    {
        if (canSelect)
        {
            Speech response = cards[cardIndex].GetResponse();

            cards[cardIndex].gameObject.SetActive(false);

            if (Mathf.Abs(currentStory.positivity - response.positivity) >= 2)
            {
                UpdateFailure();
            }

            StartCoroutine(DisplayResponse(response.speechText));
        }
    }

    private IEnumerator DisplayStory(string _text)
    {
        yield return StartCoroutine(DisplaySpeech(_text));

        yield return new WaitForSeconds(1f);

        canSelect = true;
    }

    private IEnumerator DisplayResponse(string _text)
    {
        canSelect = false;

        yield return StartCoroutine(DisplaySpeech(_text));

        yield return new WaitForSeconds(3f);

        if (failurePoints >= 5)
        {
            canPause = false;
            loseScreen.SetActive(true);
            MusicPlayer.Instance.StopMusic();
            outroSound.Play();
        }
        else
        {
            NextStory();
        }
    }

    private IEnumerator DisplaySpeech(string _text)
    {
        speechText.text = "";

        bool moustacheUp = false;
        Vector3 moustachePosition = moustacheTransform.localPosition;
        int moustacheBuffer = 0;

        foreach (char character in _text.ToCharArray())
        {
            if (!dialogueSource.isPlaying)
            {
                dialogueSource.clip = dialogueClips[Random.Range(0, dialogueClips.Length)];
                dialogueSource.Play();
            }

            moustacheBuffer++;

            if (moustacheBuffer >= 5)
            {
                moustachePosition.y = moustacheUp ? -45f : -30f;
                moustacheTransform.localPosition = moustachePosition;
                moustacheUp = !moustacheUp;
                moustacheBuffer = 0;
            }

            speechText.text += character;
            yield return new WaitForSeconds(0.02f);
        }

        moustachePosition.y = -45f;
        moustacheTransform.localPosition = moustachePosition;
    }

    private IEnumerator DisplayDay()
    {
        canPause = false;
        canSelect = false;

        MusicPlayer.Instance.StopMusic();

        dayScreen.SetActive(true);
        introSound.Play();

        yield return new WaitForSeconds(introSound.clip.length);

        dayScreen.SetActive(false);
        MusicPlayer.Instance.PlayMusic();
        NextStory();

        canPause = true;
    }
}
