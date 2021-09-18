using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Assets.Scripts;

public class Level : MonoBehaviour
{
    private static Level instance;
    public static Level Instance
    {
        get => instance;
    }

    private const float PIPE_WIDTH = 14f;
    private const float CAMERA_ORTHO_SIZE = 50F;
    private const float FULL_SCREEN_HEIGHT = CAMERA_ORTHO_SIZE * 2f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = 100f;
    private const float BIRD_X_POSITION = 0f;

    private List<Pipe> pipeList;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize = 50f;
    private int pipeSpawned = 0;
    private int pipePassedCount = 0;
    private State state;

    public enum State
    {
        WaitingToStart,
        Playing,
        BirdDead
    }

    private enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    private void Start()
    {
        Bird.Instance.OnDied += Bird_OnDied;
        Bird.Instance.OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        state = State.BirdDead;
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Update()
    {
        if(state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];

            MovePipeAndCountThePoint(pipe);

            if(pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
            {
                //Detroy Pipe
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void MovePipeAndCountThePoint(Pipe pipe)
    {
        bool isPipeOnBirdRightSide = pipe.GetXPosition() > BIRD_X_POSITION;
        pipe.Move();
        bool isPipeHasPassedTheBird = pipe.GetXPosition() < BIRD_X_POSITION;

        if (isPipeOnBirdRightSide && isPipeHasPassedTheBird && pipe.IsBottom)
        {
            pipePassedCount++;
        }
    }

    private void IncreaseDifficulty()
    {
        SetDifficulty(GetDifficulty());
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimer = 1.2f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimer = 1.1f;
                break;
            case Difficulty.Hard:
                gapSize = 33f;
                pipeSpawnTimer = 1.0f;
                break;
            case Difficulty.Impossible:
                gapSize = 24f;
                pipeSpawnTimer = 0.8f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipeSpawned >= 10) return Difficulty.Medium;
        if (pipeSpawned >= 20) return Difficulty.Hard;
        if (pipeSpawned >= 30) return Difficulty.Impossible;
        return Difficulty.Easy;
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if(pipeSpawnTimer < 0)
        {
            //Reset timer
            pipeSpawnTimer += pipeSpawnTimerMax;
            GenerateRandomGapPipe();
            IncreaseDifficulty();
        }
    }

    private void GenerateRandomGapPipe()
    {
        // To avoid the top of the pipe is generate at the edge of the screen.
        float heightEdgeLimit = 10f;

        float haftOfGapSize = gapSize * 0.5f;
        float minHeight = haftOfGapSize + heightEdgeLimit;
        float maxHeight = FULL_SCREEN_HEIGHT - minHeight;

        float height = Random.Range(minHeight, maxHeight);
        CreateGapPipe(height, gapSize, PIPE_SPAWN_X_POSITION);
    }

    private void CreateGapPipe(float gapCenterYPosition, float gapSize, float xPosition)
    {
        float haftOfGapSize = gapSize * 0.5f;
        bool isBottom = true;

        float bottomPipeHeight = gapCenterYPosition - haftOfGapSize;
        CreatePipe(bottomPipeHeight, xPosition, isBottom);

        float topPipeHeight = FULL_SCREEN_HEIGHT - gapCenterYPosition - haftOfGapSize;
        CreatePipe(topPipeHeight, xPosition, !isBottom);

        pipeSpawned++;
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        float pipeYPosition;
        Transform pipeTransform = Instantiate(GameAssets.GetInstance().pfPipe);

        if (createBottom)
        {
            pipeYPosition = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeYPosition = CAMERA_ORTHO_SIZE;
            // revert the pipe.
            pipeTransform.localScale = new Vector3(1, -1, 1);
        }

        pipeTransform.position = new Vector3(xPosition, pipeYPosition);

        SpriteRenderer pipeSprintRenderer = pipeTransform.GetComponent<SpriteRenderer>();
        pipeSprintRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBoxCollider = pipeTransform.GetComponent<BoxCollider2D>();
        pipeBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBoxCollider.offset = new Vector2(0f, height * .5f);

        Pipe pipe = new Pipe(pipeTransform, createBottom);
        pipeList.Add(pipe);
    }

    public int GetPipeSpawned()
    {
        return pipeSpawned;
    }

    public int GetPipesPassedCount()
    {
        return pipePassedCount;
    }

    private class Pipe
    {
        private Transform pipeTransform;
        private bool isBottom;

        public Pipe(Transform pipeTransform, bool isBottom)
        {
            this.pipeTransform = pipeTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeTransform.position.x;
        }

        public void DestroySelf()
        {
            Destroy(pipeTransform.gameObject);
        }

        public bool IsBottom
        {
            get => isBottom;
        }
          
    }
}
