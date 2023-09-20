using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LevelData level;
    [SerializeField] private Pipe cellPrefab;

    private bool gameFinished;
    private Pipe[,] pipes;
    private List<Pipe> startPipes;

    private void Awake()
    {
        Instance = this;
        gameFinished = false;
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        pipes = new Pipe[level.Row, level.Column];
        startPipes = new List<Pipe>();

        for(int i = 0; i < level.Row; i++)
        {
            for(int j = 0; j < level.Column; j++)
            {
                Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                Pipe tempPipe = Instantiate(cellPrefab);
                tempPipe.transform.position = spawnPos;
                tempPipe.Init(level.Data[i * level.Column + j]);
                pipes[i,j] = tempPipe;
                if(tempPipe.pipeType == 1)
                {
                    startPipes.Add(tempPipe);
                }
            }
        }

        Camera.main.orthographicSize = Mathf.Max(level.Row, level.Column) + 2f;
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = level.Column * 0.5f;
        cameraPos.y = level.Row * 0.5f;
        Camera.main.transform.position = cameraPos;
    }

    private void Update()
    {
        if (gameFinished)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);

        if(row < 0 || col < 0)
        {
            return;
        }

        if (row >= level.Row)
        {
            return;
        }

        if(col >= level.Column)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            pipes[row, col].UpdateInput();
            StartCoroutine(ShowHint());
        }
    }

    private IEnumerator ShowHint()
    {
        yield return new WaitForSeconds(0.1f);
        CheckFill();
        CheckWin();
    }

    private void CheckFill()
    {
        for (int i = 0; i < level.Row; i++)
        {
            for (int j = 0; j < level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                if(tempPipe.pipeType != 0)
                {
                    tempPipe.isFilled = false;
                }
            }
        }
        Queue<Pipe> check = new Queue<Pipe>();
        HashSet<Pipe> finished = new HashSet<Pipe>();
        foreach(var pipe in startPipes)
        {
            check.Enqueue(pipe);
        }

        while(check.Count > 0)
        {
            Pipe pipe = check.Dequeue();
            finished.Add(pipe);
            List<Pipe> connected = pipe.ConnectedPipes();
            foreach(var connectedPipe in connected)
            {
                if (!finished.Contains(connectedPipe))
                {
                    check.Enqueue(connectedPipe);
                }
            }
        }

        foreach(var filled in finished)
        {
            filled.isFilled = true;
        }

        for (int i = 0; i < level.Row; i++)
        {
            for (int j = 0; j < level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }
    }

    private void CheckWin()
    {
        for (int i = 0; i < level.Row; i++)
        {
            for(int j = 0; j < level.Column; j++)
            {
                if(!pipes[i,j].isFilled)
                {
                    return;
                }
            }
        }

        gameFinished = true;
        StartCoroutine(FinishGame());
    }

    private IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}