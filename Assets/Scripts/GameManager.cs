using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private List<string> animals = new List<string> { "ant", "capybara", "cow", "elephant", "frog", "goat", "human", "monkey", "orangutan", "pig", "pug", "sheep", "wolf", "hippo"};
    private Dictionary<string, string> timeStamps = new Dictionary<string, string>();

    private List<string> randomized = new List<string>();
    private int currentIndex = 0;
    private float timer = 0f;
    private bool started = false;
    private bool finished = false;

    public float cycleTime = 5.0f;

    public float safetyThreshold = 7.5f;

    public GridBuilder grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (!started && timer >= cycleTime)
        {
            StartSequence();
            started = true;
            timer = 0f;
            ShowNextAnimal();  // Show first image immediately after delay
        }
        else if (started && !finished && timer >= cycleTime)
        {
            ShowNextAnimal();
            timer = 0f;
        }
    }
    void StartSequence()
    {
        randomized = new List<string>(animals);
        for (int i = 0; i < randomized.Count; i++)
        {
            int rand = Random.Range(i, randomized.Count);
            (randomized[i], randomized[rand]) = (randomized[rand], randomized[i]);
        }
        currentIndex = 0;
    }

    void ShowNextAnimal()
    {
        if (currentIndex < randomized.Count)
        {
            string animal = randomized[currentIndex];
            Sprite loaded = Resources.Load<Sprite>("Animals/" + animal);
            spriteRenderer.sprite = loaded;

            if (!timeStamps.ContainsKey(animal))
                timeStamps.Add(animal, System.DateTime.Now.ToString("HH:mm:ss"));

            currentIndex++;
        }
        else
        {
            finished = true;
            Invoke("ClearImage", cycleTime);
        }
    }

    void ClearImage()
    {
        //spriteRenderer.sprite = null;
        if (DidPlayerWin())
        {
            Sprite loaded = Resources.Load<Sprite>("checkmark");
            spriteRenderer.sprite = loaded;
        }
        else
        {
            Sprite loaded = Resources.Load<Sprite>("fail");
            spriteRenderer.sprite = loaded;
        }
    }
    bool DidPlayerWin()
    {
        for (int row = 0; row < grid.rows; row++)
        {
            string animalCell = grid.cell_datas[row, 0].get_text();
            string timestampCell = grid.cell_datas[row, 1].get_text();

            if (string.IsNullOrWhiteSpace(animalCell) || string.IsNullOrWhiteSpace(timestampCell))
                continue;

            string animal = animalCell.Trim().ToLower();
            string timestampStr = timestampCell.Trim();

            if (!timeStamps.ContainsKey(animal))
                return false;

            string savedTimestampStr = timeStamps[animal];

            if (!System.DateTime.TryParse(timestampStr, out var cellTime))
                return false;

            if (!System.DateTime.TryParse(savedTimestampStr, out var savedTime))
                return false;

            double secondsDiff = System.Math.Abs((cellTime - savedTime).TotalSeconds);

            if (secondsDiff > safetyThreshold)
                return false;
        }
        return true;
    }
}
