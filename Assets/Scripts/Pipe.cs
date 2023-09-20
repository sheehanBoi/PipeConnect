using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [HideInInspector] public bool isFilled;
    [HideInInspector] public int pipeType;

    [SerializeField] private Transform[] pipePrefabs;

    private Transform currentPipe;
    private int rotation;

    private SpriteRenderer emptySprite;
    private SpriteRenderer filledSprite;
    private List<Transform> connectBoxes;

    private const int minRotation = 0;
    private const int maxRotation = 3;
    private const int rotationMultiplier = 90;


    public void Init(int pipe)
    {
        pipeType = pipe % 10;
        currentPipe = Instantiate(pipePrefabs[pipeType], transform);
        currentPipe.transform.localPosition = Vector3.zero;

        if (pipeType == 1 || pipeType == 2)
        {
            rotation = pipe / 10;
        }
        else
        {
            rotation = Random.Range(minRotation, maxRotation + 1);
        }
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

        if(pipeType == 0 || pipeType == 1)
        {
            isFilled = true;
        }

        if(pipeType == 0)
        {
            return;
        }

        emptySprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
        emptySprite.gameObject.SetActive(!isFilled);
        filledSprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
        filledSprite.gameObject.SetActive(isFilled);

        connectBoxes = new List<Transform>();
        for(int i = 2; i < currentPipe.childCount; i++)
        {
            connectBoxes.Add(currentPipe.GetChild(i));
        }
    }

    public void UpdateInput()
    {
        if(pipeType == 0 || pipeType == 1 || pipeType == 2)
        {
            return;
        }
        rotation = (rotation + 1) % (maxRotation + 1);
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);
    }

    public void UpdateFilled()
    {
        if(pipeType == 0)
        {
            return;
        }
        emptySprite.gameObject.SetActive(!isFilled);
        
        filledSprite.gameObject.SetActive(isFilled);
    }

    public List<Pipe> ConnectedPipes()
    {
        List<Pipe> result = new List<Pipe>();

        foreach(var box in connectBoxes)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(box.transform.position, Vector2.zero, 0.1f);
            for(int i = 0; i < hit.Length; i++)
            {
                result.Add(hit[i].collider.transform.parent.parent.GetComponent<Pipe>());
            }
        }
        return result;
    }
}
