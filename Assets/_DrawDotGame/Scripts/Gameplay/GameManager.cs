using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using EasyMobile;

namespace DrawDotGame
{
    public enum GameState
    {
        Prepare,
        Playing,
        Paused,
        PreGameOver,
        GameOver
    }

    public enum Mode
    {
        LevelEditorMode,
        GameplayMode,
        HelpMode
    }

    public class GameManager : MonoBehaviour
    {


        public GameObject randomPower;

        public static event System.Action<GameState, GameState> GameStateChanged = delegate { };
        public static event System.Action<bool, bool> GameEnded = delegate { };

        private GameObject basicBomb, intermediateBomb, superBomb;

        private GameObject basicBombCount, intermediateBombCount, superBombCount;

        private int current_score;
        public const string LAST_SAVED_SCORE = "LAST_SAVED_SCORE"; //For total score till now for this player
        private static int total_score = -1;
        // The selected level
        public static int TotalScore
        {
            get
            {
                if (total_score == -1)
                {
                    levelLoaded = PlayerPrefs.GetInt(LAST_SAVED_SCORE, 1);
                }
                return total_score;
            }
            set
            {
                total_score = value;
                PlayerPrefs.SetInt(LAST_SAVED_SCORE, total_score);
            }
        }
        public GameObject statusBar;
        public GameState GameState
        {
            get
            {
                return _gameState;
            }
            private set
            {
                if (value != _gameState)
                {
                    GameState oldState = _gameState;
                    _gameState = value;

                    GameStateChanged(_gameState, oldState);
                }
            }
        }

        [SerializeField]
        private GameState _gameState = GameState.Prepare;

        public static int GameCount
        {
            get { return _gameCount; }
            private set { _gameCount = value; }
        }

        private static int _gameCount = 0;


        public static void gestureDraw(string gesture_name, int percentage_accuracy)
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            if (gesture_name == "StillToBeDecided")
            {

                foreach (GameObject ball in balls)
                    GameObject.Destroy(ball);

            }
        }



        public const string LAST_SELECTED_LEVEL_KEY = "LAST_SELECTED_LEVEL";
        private static int levelLoaded = -1;
        // The selected level
        public static int LevelLoaded
        {
            get
            {
                if (levelLoaded == -1)
                {
                    levelLoaded = PlayerPrefs.GetInt(LAST_SELECTED_LEVEL_KEY, 1);
                }
                return levelLoaded;
            }
            set
            {
                levelLoaded = value;
                PlayerPrefs.SetInt(LAST_SELECTED_LEVEL_KEY, levelLoaded);
            }
        }

        public bool showKilledEffectForTarget = false;
        public int pinkBallHits = 0;
        public int BlueBallHits = 0;

        public int BallsKilled = 0;

        public int TotalAvailableDots;

        public Text funFactText;

        public ObstacleManager obstacleManager;
        public GameplayUIManager gameplayUIManager;
        public GameObject pinkBallPrefab;
        public GameObject blueBallPrefab;
        public GameObject badiAntPrefab;
        public GameObject bonusAntPrefab;
        public int bonusPointsCaptured = 0;

        public GameObject redAntPrefab;

        public GameObject hintPrefab;
        public ParticleSystem explores;
        public ParticleSystem winning;
        [HideInInspector]
        public bool gameOver;
        [HideInInspector]
        public bool win = false;
        public GameObject killedEffect; //Particle system to be shown on killed

        public int bonusAntValue = 10;

        public int basicBombCost = 100;
        public int intermediateBombCost = 200;
        public int superBombCost = 300;


        public Material myMaterial;


        private GameObject redBombParticles;
        private GameObject greenBombParticles;
        private GameObject radialBombParticles;
        public GameObject basicBombTemplate;
        public GameObject intermediateBombTemplate;
        public GameObject superBombTemplate;


        public GameObject invincible_animation;

        private GameObject leftTop;
        private GameObject rightTop;
        private GameObject leftBottom;
        private GameObject rightBottom;
        Bounds gameAreaBounds;


        public Mode mode;
        [HideInInspector]
        public string failedScreenshotName = "failedLevel.png";
        public Sprite collider_sprite;

        public AudioClip eatSound;
        private AudioSource audioSource;

        public bool randomMovement = true;

        public Sprite invincibleCollider;

        [Header("Gameplay Config")]
        [Tooltip("The color of the drawn lines")]
        public Color lineColor;
        public Material lineMaterial;
        [Tooltip("How many hearts spent to view 1 hint")]
        public int heartsPerHint = 1;
        [Tooltip("How many hearts should be awarded when the player solves a level for the first time, put 0 to disable this feature")]
        public int heartsPerWin = 0;

        private LevelManager levelManager;
        public List<GameObject> listLine = new List<GameObject>();
        private List<Vector2> listPoint = new List<Vector2>();
        public int totalPoints = 0;
        private int livePoints = 0; // Points being drawn...
        private GameObject currentLine;
        private GameObject currentColliderObject;
        private GameObject hintTemp;
        private BoxCollider2D currentBoxCollider2D;
        private LineRenderer currentLineRenderer;
        private Rigidbody2D pinkBallRigid;
        private Rigidbody2D blueBallRigid;
        private bool stopHolding;
        private bool allowDrawing = true;

        public int noOfBallsAdded = 0;
        public int total_num_of_balls = 0;


        private bool isCreatingInsects = false;

        private List<Rigidbody2D> listObstacleNonKinematic = new List<Rigidbody2D>();
        private GameObject[] obstacles;

        private List<GameObject> lines; // All Drawn line or random structures to kill insects
        public GameObject pinkBall;

        public GameObject blueBall { get; private set; }


        public List<GameObject> insects;

        public int PointsScored
        {
            get
            {
                return pointsScored;
            }

            set
            {
                pointsScored = value;
            }
        }

        public Rect gameArea;

        private int pointsScored = 0;

        Bounds newBounds;

        private string[] fun_facts = { "An ant can lift 20 times its own body weight. If a second grader was as strong as an ant, she would be able to pick up a car!",
        "Some queen ants can live for many years and have millions of babies!",
        "Ants don’t have ears. Ants 'hear' by feeling vibrations in the ground through their feet.",
        "When ants fight, it is usually to the death!",
        "When foraging, ants leave a pheromone trail so that they know where they’ve been.",
        "Queen ants have wings, which they shed when they start a new nest.",
        "Ants don’t have lungs. Oxygen enters through tiny holes all over the body and carbon dioxide leaves through the same holes.",
        "When the queen of the colony dies, the colony can only survive a few months. Queens are rarely replaced and the workers are not able to reproduce.",
        "There are three kinds of ants in a colony: The queen, the female workers, and males. The queen and the males have wings, while the workers don’t have wings. The queen is the only ant that can lay eggs. The male ant’s job is to mate with future queen ants and they do not live very long afterwards. Once the queen grows to adulthood, she spends the rest of her life laying eggs! Depending on the species, a colony may have one queen or many queens.",
        "Ants have the ability to carry between 10 and 50 times their own body weight! The amount an ant can carry depends on the species. The Asian weaver ant, for example, can lift 100 times its own mass.",
        "Ants do not have ears and use vibrations to hear, using them when foraging for food or as an alarm signal. Ants use the vibrations in the ground to hear by picking them up in the subgenual organ which is located below the knee.",
        "That’s right, ants have two stomachs, and it’s not because they are greedy. One of their stomachs is for holding food for their own consumption, and the second one is to hold food to be shared with other ants.",
        "A study from Harvard and Florida State Universities discovered that ants first rose during the Cretaceous period around 130 million years ago! They have survived the Cretaceous-Tertiary (K/T extinction) that killed the dinosaurs as well as the ice age.",
        "In certain ant species, the soldier ants have modified heads, shaped to match the nest entrance. They block access to the nest by sitting just inside the entrance, with their heads functioning like a cork in a bottle. When a worker ant returns to the nest, it touches the soldier ant's head to let the guard know it belongs to the colony.",
        "Scientists estimate there are at least 1.5 million ants on the planet for every human being. Over 12,000 species of ants are known to exist, on every continent except Antarctica. Most live in tropical regions. A single acre of Amazon rainforest may house 3.5 million ants.",
        "Ants take the spirit of cooperation to a whole new level; as many as 50 million ants can come together to function as one highly organized, efficient colony.",
        "Ants are no dummies. One tiny ant can have as many as 250,000 brain cells, so a colony of 40,000 ants collectively has the same number of brain cells as a human.",
        "Ants are literally everywhere! Unless you live in the Arctic, Antarctica or on one of a handful of remote islands, you’ll likely have at least one ant species to deal with. Hawaiians have to put up with at least 50 known ant species!",
        "Some species of ants have been in existence for around 100 million years. Ants survived a mass extinction event known as Cretaceous-Tertiary that wiped out many dinosaurs and other prehistoric animal species approximately 65 million years ago.",
        "Ants are social insects that live in structured nest communities throughout the world. The species determines their ant habitat – whether they live underground, in mounds built at ground level, in wood structures or in plants or trees. Soil and plant matter are typically used to construct the nests.",
        "There are more than 12,000 species of ants all over the world.",
        "Argentine ants prefer sweet substances but will eat almost anything including meats, eggs, oils and fats. They leave pheromone trails everywhere they go to ensures they do not waste time visiting the same area twice.",
        "Scientists estimate there are at least 1.5 million ants on the planet for every human being.",
        "Average ant colony contains thousands of individual ants.",
        "Ants can become zombies, there is a species of fungus that infects ants and takes control of their bodies.",
        "Not all ant species build nests. A group of about 200 species known as army ants have two phases of their life: nomad and stationary. During the colony's nomad phase, the ants travel all day, attacking other colonies and insects they encounter for food. The only time they stop traveling is during the stationary phase when the queen lays eggs and the colony waits for them to hatch.",
        "Ants are found on every continent on Earth except Antarctica. A few islands such as Greenland do not have any native ant species, but individual ants have been brought in through human travel.",
        "Ants are thrift in nature.They very carefully manage their resources without wastage.",
        "Ants share everything there collect.They never eat alone.",
        "Ants always march one after the other without  chaos, confusion or disorder.",
        "Living in humid jungle conditions such as the Amazon, the bullet ant is said to have the most painful sting in the world. their sting has been compared to being hit by a bullet.",
        "North America’s red imported fire ant might only be little, but the tiny critters have a painful bite which causes a burning sensation – hence the name “fire ant”, which costs the US millions in veterinary and medical bills every year.",
        "The aptly named species of trap jaw ant, can close its jaws at 140mph, which it uses to kill its prey or injure predators.",
        "The largest ant’s nest ever found was over 3,700 miles wide.",
        "It is reported that the queen ants copy themselves to genetically produce daughters, resulting in a colony with no male ants.",
        "Ants will protect aphids from natural predators, and shelter them in their nests from heavy rain showers in order to gain a constant supply of honeydew.",
        "Ants have two stomachs. One of their stomachs is for holding food for their own consumption, and the second one is to hold food to be shared with other ants.",
        "Some species of ant, such as the Polyergus lucidus are known as slave-making ants. They invade neighboring ant colonies, capturing its inhabitants and forcing them to work for them. This process is known as ‘slave raiding’.",
        "A study from Harvard and Florida State Universities discovered that ants first rose during the Cretaceous period around 130 million years ago! They have survived the Cretaceous-Tertiary (K/T extinction) that killed the dinosaurs as well as the ice age.",
        "Soldier ants use their heads to plug the entrances, functioning like a cork in a bottle to their nests and keep intruders at bay.",
        "Fungus farming ants began their agricultural ventures about 50 million years before humans thought to raise their own crops.",
        "Army ants may prey on much larger animals such as reptiles, birds, or even small mammals.",
        "All the ants on the planet weigh about as much as all the humans.",
        "Ants communicate and cooperate by using chemicals (pheromones) that can alert others to danger or lead them to a promising food source.",
        };

        public bool IsPointInRT(Vector3 point, Rect rt)
        {


            Debug.Log(point + " = " + rt.xMin + " , " + rt.xMax + " Y= " + rt.yMin + " , " + rt.yMax);

            // Check to see if the point is in the calculated bounds
            if (point.x >= rt.xMin &&
                point.x <= rt.xMax &&
                point.y <= rt.yMin &&
                point.y >= rt.yMax)
            {
                return true;
            }
            return false;
        }

        void CheckForBombBounds()
        {
            //Disable Bomb particles if not in gamearea
            if (isPointInGameArea(basicBombTemplate.transform.position))
            {
                redBombParticles.SetActive(true);
            } else
                redBombParticles.SetActive(false);


            //Disable Bomb particles if not in gamearea
            if (isPointInGameArea(intermediateBombTemplate.transform.position))
            {
                greenBombParticles.SetActive(true);
            }
            else
                greenBombParticles.SetActive(false);

        }

        List<GameObject> getLines()
        {
            return lines;
        }

        // Subscribe to rewarded ad events
        void OnEnable()
        {
            Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
        }

        // Unsubscribe events
        void OnDisable()
        {
            Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
        }
        // Event handler called when a rewarded ad has completed
        void RewardedAdCompletedHandler(RewardedAdNetwork network, AdLocation location)
        {
            Debug.Log("Rewarded ad has completed. The user should be rewarded now.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        // Event handler called when a rewarded ad has been skipped
        void RewardedAdSkippedHandler(RewardedAdNetwork network, AdLocation location)
        {
            Debug.Log("Rewarded ad was skipped. The user should NOT be rewarded.");
            SceneManager.LoadScene("index");


        }

        public void playRewardedAd()
        {

            if ((Application.platform == RuntimePlatform.IPhonePlayer) || (Application.platform == RuntimePlatform.Android))
            {
                // Check if rewarded ad is ready
                bool isReady = Advertising.IsRewardedAdReady();
                // Show it if it's ready
                if (isReady)
                {
                    Advertising.ShowRewardedAd();
                } else
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void DrawCircle(float x1, float y1, float radius)
        {
            float theta_scale = 0.1f;             //Set lower to add more points
            int size = (int)(2.0 * Mathf.PI / theta_scale); //Total number of points in circle.

            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.SetColors(Color.red, Color.yellow);
            lineRenderer.SetWidth(0.2F, 0.2F);
            lineRenderer.SetVertexCount(size);

            int i = 0;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.1f)
            {
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);

                Vector3 pos = new Vector3(x1 + x, y1 + y, 0);
                lineRenderer.SetPosition(i, pos);
                i += 1;
            }
        }

        static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
        {
            //Author: Isaac Dart, June-13.
            Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
            return null;
        }

        void DrawRect(float x1, float y1, float width, float height)
        {


            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.material = myMaterial;
            //lineRenderer.material = new Material(shader);
            lineRenderer.SetColors(Color.red, Color.yellow);
            lineRenderer.SetWidth(0.2F, 0.2F);
            lineRenderer.SetVertexCount(5);

            float left = x1 - width / 2;
            float right = x1 + width / 2;
            float top = y1 - height / 2;
            float bottom = y1 + height / 2;

            lineRenderer.SetPosition(0, new Vector3(left, top, 0));
            lineRenderer.SetPosition(1, new Vector3(right, top, 0));
            lineRenderer.SetPosition(2, new Vector3(right, bottom, 0));
            lineRenderer.SetPosition(3, new Vector3(left, bottom, 0));
            lineRenderer.SetPosition(4, new Vector3(left, top, 0));
        }

        IEnumerator plantBomb(string bomb_type)
        {
            int currency_to_use = current_score;
            currency_to_use = totalPoints;

            Debug.Log("Planting a bomb");
            GameObject bomb = new GameObject();
            GameObject progress = null;
            float unit_progress_per_half_sec = 0f;
            switch (bomb_type)
            {
                case "basic":

                    bomb = Instantiate(basicBombTemplate, GameObject.Find("Target").transform.position, Quaternion.identity);
                    totalPoints += basicBombCost;
                    //progress = GameObject.Find(bomb.name + "/progress");
                    progress = getChildGameObject(bomb, "progress");
                    UpdateBombs();
                    unit_progress_per_half_sec = progress.transform.localScale.x/6;
                    for (int i = 0; i < 6; ++i)
                    {
                        yield return new WaitForSeconds(0.5f);
                        

                        Vector3 lTemp = progress.transform.localScale;
                        lTemp.x -= (unit_progress_per_half_sec);
                        progress.transform.localScale = lTemp;
                    }

                    break;

                case "intermediate":
                    bomb = Instantiate(intermediateBombTemplate, GameObject.Find("Target").transform.position, Quaternion.identity);

                    totalPoints += intermediateBombCost;
                    UpdateBombs();
                    progress = GameObject.Find(bomb.name + "/progress");
                    unit_progress_per_half_sec = progress.transform.localScale.x / 10;

                    for (int i = 0; i < 10; ++i)
                    {
                        yield return new WaitForSeconds(0.5f);


                        Vector3 lTemp = progress.transform.localScale;
                        lTemp.x -= (unit_progress_per_half_sec);
                        progress.transform.localScale = lTemp;
                    }
                    break;

                case "super":
                    bomb = Instantiate(superBombTemplate, GameObject.Find("Target").transform.position, Quaternion.identity);
                    totalPoints += superBombCost;
                    UpdateBombs();
                    progress = GameObject.Find(bomb.name + "/progress");
                    unit_progress_per_half_sec = progress.transform.localScale.x / 20;

                    for (int i = 0; i < 20; ++i)
                    {
                        yield return new WaitForSeconds(0.5f);

                        
                        Vector3 lTemp = progress.transform.localScale;
                        lTemp.x -= (unit_progress_per_half_sec);
                        progress.transform.localScale = lTemp;
                    }
                    break;
            }
            
           
            Destroy(bomb);
            
        }

        public void activateBomb(string bomb_type)
        {
            Debug.Log("S");
            StartCoroutine(plantBomb(bomb_type));

        }

        public void activateIntermediateBomb()
        {

        }

        public void activateBasicBomb()
        {

        }

        IEnumerator destroyObjectDelayed(GameObject obj, int secs)
        {
            yield return new WaitForSeconds(secs);
            Destroy(obj);
        }

        void UpdateBombs()
        {
            int currency_available = current_score;
            currency_available = TotalAvailableDots - totalPoints;


            basicBombCount.GetComponent<Text>().text = basicBombCost.ToString();
            intermediateBombCount.GetComponent<Text>().text = intermediateBombCost.ToString();
            superBombCount.GetComponent<Text>().text = superBombCost.ToString();


            //basicBombCount.GetComponent<Text>().text = ((currency_available / basicBombCost) == 0) ? "" : (currency_available / basicBombCost).ToString();
            //intermediateBombCount.GetComponent<Text>().text = ((currency_available / intermediateBombCost) == 0) ? "" : (currency_available / intermediateBombCost).ToString();
            //superBombCount.GetComponent<Text>().text = ((currency_available / superBombCost) == 0) ? "" : (currency_available / superBombCost).ToString();

            //Update basic bomb after 500 points
            if (currency_available >= basicBombCost)
            {
                basicBomb.GetComponent<Animator>().enabled = true;
                basicBomb.GetComponent<Button>().interactable = true;
            }
            else
            {
                basicBomb.GetComponent<Animator>().enabled = false;
                basicBomb.GetComponent<Button>().interactable = false;
                //basicBomb.GetComponent<Image>().fillAmount = (float)(currency_available % basicBombCost) / basicBombCost;
            }


            if (currency_available >= intermediateBombCost) {
                intermediateBomb.GetComponent<Animator>().enabled = true;

                intermediateBomb.GetComponent<Button>().interactable = true;

            }
            else {
                intermediateBomb.GetComponent<Animator>().enabled = false;
                intermediateBomb.GetComponent<Button>().interactable = false;
                //intermediateBomb.GetComponent<Image>().fillAmount = (float)(currency_available % intermediateBombCost) / intermediateBombCost;
            }



            if (currency_available >= superBombCost)
            {
                superBomb.GetComponent<Animator>().enabled = true;
                superBomb.GetComponent<Button>().interactable = true;
            }
            else
            {
                superBomb.GetComponent<Animator>().enabled = false;
                //superBomb.GetComponent<Image>().fillAmount = (float)(currency_available % superBombCost) / superBombCost;
                superBomb.GetComponent<Button>().interactable = false;
            }


            UpdateTextAndProgress();
        }

        public void LoadLevelOne()
        {

            //TotalAvailable Dots = 1000;
            TotalAvailableDots = 1000;
                LevelLoaded = 1;
                GameState = GameState.Prepare;
                string path = LevelScroller.JSON_PATH;
                TextAsset textAsset = Resources.Load<TextAsset>(path);
                string[] data = textAsset.ToString().Split(';');
                foreach (string o in data)
                {
                    LevelData levelData = JsonUtility.FromJson<LevelData>(o);
                    if (levelData.levelNumber == LevelLoaded)
                    {
                        CreateLevel(levelData);
                        GameState = GameState.Playing;
                        break;
                    }
                }
        }

        private IEnumerator plantRandomBomb()
        {
            float random_number = Random.Range(1.0f, 400.0f);
            yield return new WaitForSeconds(random_number);
            randomPower.transform.position = getRandomPointInGameArea();
            randomPower.SetActive(true);
            yield return new WaitForSeconds(4);
            randomPower.SetActive(false);

        }


        void Start()
        {
            StartCoroutine(plantRandomBomb());
            basicBomb = GameObject.Find("basicBomb");
            intermediateBomb = GameObject.Find("intermediateBomb");
            superBomb = GameObject.Find("superBomb");

            basicBombCount = GameObject.Find("basicBombCount");
            intermediateBombCount = GameObject.Find("intermediateBombCount");
            superBombCount = GameObject.Find("superBombCount");


       

            gameAreaBounds = GameObject.Find("GameArea").GetComponent<SpriteRenderer>().sprite.bounds;

            // gameAreaBounds.center.Set(gameAreaBounds.center.x + GameObject.Find("GameArea").transform.position.x, gameAreaBounds.center.y - GameObject.Find("GameArea").transform.position.y,0);
            Debug.Log("Before scaling" + gameAreaBounds);

            gameAreaBounds.extents.Set(gameAreaBounds.extents.x * 10 * (GameObject.Find("GameArea").transform.localScale.x),
                gameAreaBounds.extents.y * (GameObject.Find("GameArea").transform.localScale.y), 0);
            newBounds = new Bounds(new Vector3(gameAreaBounds.center.x + GameObject.Find("GameArea").transform.localPosition.x, gameAreaBounds.center.y + GameObject.Find("GameArea").transform.localPosition.y, gameAreaBounds.center.z), new Vector3(2 * gameAreaBounds.extents.x * GameObject.Find("GameArea").transform.localScale.x, 2 * gameAreaBounds.extents.y * (GameObject.Find("GameArea").transform.localScale.y), 1));
            Debug.Log("After scaling" + newBounds);
            DrawRect(newBounds.center.x, newBounds.center.y, newBounds.size.x, newBounds.size.y);
            Debug.Log(gameAreaBounds);


            bonusPointsCaptured = 0;
            PointsScored = 0;
            //TotalAvailableDots = 300;
            //TotalAvailableDots = (200 * ((levelLoaded / 10) + 1));
            TotalAvailableDots = GetTotalAvailableDots();
            UpdateBombs();
            noOfBallsAdded = 0;
            lineMaterial.SetColor("_Color", lineColor);
            if (mode == Mode.GameplayMode) //On gameplay mode
            {
                GameState = GameState.Prepare;
                string path = LevelScroller.JSON_PATH;
                TextAsset textAsset = Resources.Load<TextAsset>(path);
                string[] data = textAsset.ToString().Split(';');
                foreach (string o in data)
                {
                    LevelData levelData = JsonUtility.FromJson<LevelData>(o);
                    if (levelData.levelNumber == LevelLoaded)
                    {
                        CreateLevel(levelData);
                        GameState = GameState.Playing;
                        break;
                    }
                }
            }
           
            else //On level editor mode
            {
                BallController[] ballsController = FindObjectsOfType<BallController>();
                foreach (BallController o in ballsController)
                {
                    if (o.name.Split('(')[0].Equals("PinkBall"))
                        pinkBallRigid = o.gameObject.GetComponent<Rigidbody2D>();
                    else
                        blueBallRigid = o.gameObject.GetComponent<Rigidbody2D>();
                }
                //pinkBallRigid.isKinematic = true;
                //blueBallRigid.isKinematic = true;
                levelManager = FindObjectOfType<LevelManager>();
            }

            obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (GameObject o in obstacles)
            {
                Rigidbody2D rigid = o.GetComponent<Rigidbody2D>();
                if (rigid != null && !rigid.isKinematic)
                {
                    listObstacleNonKinematic.Add(rigid);
                    rigid.isKinematic = true;
                }
            }

            UpdateTextAndProgress();
        }

        public void Win()
        {
            win = true;
            StopAllPhysics();
            SoundManager.Instance.PlaySound(SoundManager.Instance.win, true);

            if (mode == Mode.GameplayMode)
            {
                bool firstWin = !LevelManager.IsLevelSolved(LevelLoaded);   // solved for the first time

                if (firstWin)
                {
                    LevelManager.MarkLevelAsSolved(LevelLoaded);
                }

                StartCoroutine(CRTakeScreenshot());
                GameEnded(win, firstWin);    // fire event
            }
            setFunFact();
        }

        GameObject[] FindGameObjectsWithName(string name)
        {
            int a = GameObject.FindObjectsOfType<GameObject>().Length;
            GameObject[] arr = new GameObject[a];
            int FluentNumber = 0;
            for (int i = 0; i < a; i++)
            {
                if (GameObject.FindObjectsOfType<GameObject>()[i].name == name)
                {
                    arr[FluentNumber] = GameObject.FindObjectsOfType<GameObject>()[i];
                    FluentNumber++;
                }
            }
            System.Array.Resize(ref arr, FluentNumber);
            return arr;
        }

        public GameObject[] getLineObjects()
        {
            return FindGameObjectsWithName("Line");
        }

        public Transform[] getAllLineTransforms()
        {
            GameObject[] lines = getLineObjects();
            Transform[] transforms = new Transform[1];
            int count = 0;
            for (int i = 0; i < lines.Length; ++i)
            {
                Transform line_transform = lines[i].transform;
                System.Array.Resize(ref transforms, count + line_transform.childCount);
                //Debug.Log(line_transform.childCount + " = Number of Children of This line");
                for (int j = 0; j < line_transform.childCount; ++j)
                {
                    //Debug.Log("Getting Child " + i + parentLine.transform.GetChild(i));
                    transforms[count++] = line_transform.GetChild(j);
                }
            }
            return transforms;

        }

        public Vector2 getTargetPos()
        {
            Transform next_prey;
            if (false && isCreatingInsects)
                next_prey = GameObject.Find("AntAttractor").transform;
            else
                next_prey = GameObject.Find("Target").transform;
            return next_prey.position;
        }



        public void pushObjectTowardsAWall(GameObject obj)
        {

            Transform next_prey = GameObject.Find("Target").transform;
            Vector2 _direction = (next_prey.position - obj.transform.position).normalized;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            //create the rotation we need to be in to look at the target
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            // rotate us over time according to speed until we are in the required rotation
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, q, Time.deltaTime * 101);

            //this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            obj.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
            obj.GetComponent<Rigidbody2D>().Sleep();
            obj.GetComponent<Rigidbody2D>().AddForce(0.3f * (next_prey.position - obj.transform.position));
            obj.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;

            if (obj.GetComponent<Rigidbody2D>().isKinematic)
            {
                //obj.GetComponent<Rigidbody2D>().MovePosition(obj.transform.position + obj.transform.right * Time.deltaTime);
            }
            //obj.GetComponent<Rigidbody2D>().AddRelativeForce(1f * Vector2.up);

        }

        public void GameOver()
        {
            win = false;
            gameOver = true;
            GameState = GameState.GameOver;
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);

            if (mode == Mode.GameplayMode)
            {
                StartCoroutine(CRTakeScreenshot());
                GameEnded(win, false);
            }
            setFunFact();
        }

        public void setFunFact()
        {
            string fun_fact = fun_facts[levelLoaded % 42 ];
            funFactText.GetComponent<Text>().text = fun_fact;

        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void AddRandomMovement(Rigidbody2D rigidBody)
        {
            //if(rigidBody)
            //    rigidBody.AddRelativeForce(Random.onUnitSphere * 1);
        }

        public int pointsNeededforlevel(int level)
        {
            return (5 + 10 * (level - 1)); // Till level 10

        }

        public void ShareApp()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                Sharing.ShareURL("https://itunes.apple.com/in/app/ant-strike/id1450161184?mt=8");
            } else if (Application.platform == RuntimePlatform.Android) {
                Sharing.ShareURL("https://play.google.com/store/apps/details?id=com.fplay.antstrike&hl=en");
            }
        }
        private static Vector3 RandomPointInBox(Vector3 center, Vector3 size)
        {

            return center + new Vector3(
               (Random.value - 0.5f) * size.x,
               (Random.value - 0.5f) * size.y,
               (Random.value - 0.5f) * size.z
            );
        }

        IEnumerator RandomizeInsects()
        {
            int length = insects.Count;
            while (length >0) {
                yield return new WaitForSeconds(Random.Range(1, 5));
//                length = insects.Count;
                int next_non_random_insect_index = (length == 1) ? 0: (int)Random.Range(0, length-1);
                Debug.Log("Random index:  " + next_non_random_insect_index);
                GameObject insect = insects[next_non_random_insect_index];
                if (insect != null) //TODO if the insect has been killed
                {
                    insects[next_non_random_insect_index].GetComponent<BallController>().random = false;
                    insects.RemoveAt(next_non_random_insect_index);

                }
                length = length - 1;
            }
        }


        public Vector3 getRandomPointInGameArea()
        {
            return RandomPointInBox(newBounds.center, newBounds.size);
        }
        public void AddAnotherBall(string type)
        {
            int i = UnityEngine.Random.Range(0, 33);
            i = i % 4;

            GameObject startPos = GameObject.Find("StartPoint1"); ;
            GameObject startPos1 = GameObject.Find("StartPoint1");
            GameObject startPos2 = GameObject.Find("StartPoint2");
            GameObject startPos3 = GameObject.Find("StartPoint3");
            GameObject startPos4 = GameObject.Find("StartPoint4");

            if (i == 0)
                startPos = startPos2;
            if (i == 1)
                startPos = startPos3;
            if (i == 2)
                startPos = startPos4;
            if (i == 3)
                startPos = startPos1;


            Vector3 location_to_spawn = RandomPointInBox(newBounds.center, newBounds.size);
            if (levelLoaded == 1)
                location_to_spawn = startPos3.transform.position;
            else if (levelLoaded <= 10)
            {
                //start 
                location_to_spawn = startPos.transform.position;
            }


            switch (type)
            {
                case "badi": blueBall = Instantiate(badiAntPrefab, location_to_spawn, Quaternion.identity);
                    break;
                case "red":
                    blueBall = Instantiate(redAntPrefab, location_to_spawn, Quaternion.identity);
                    break;
                case "choti":
                    blueBall = Instantiate(blueBallPrefab, location_to_spawn, Quaternion.identity);
                    break;
                case "bonus":
                    blueBall = Instantiate(bonusAntPrefab, location_to_spawn, Quaternion.identity);
                    break;

            }

            blueBallRigid = blueBall.GetComponent<Rigidbody2D>();
            blueBallRigid.gravityScale = 0;
            blueBall.GetComponent<BallController>().ant_type = type;
            insects.Add(blueBall);
            Debug.Log("Added blueball");
            Debug.Log(insects[0]);
        }



        public void AddAnotherBall()
        {
            int i = UnityEngine.Random.Range(0, 33);
            i = i % 4;

            GameObject startPos = GameObject.Find("StartPoint1"); ;
            GameObject startPos1 = GameObject.Find("StartPoint1");
            GameObject startPos2 = GameObject.Find("StartPoint2");
            GameObject startPos3 = GameObject.Find("StartPoint3");
            GameObject startPos4 = GameObject.Find("StartPoint4");

            Debug.Log("Adding another insect");
            if (i == 0)
                startPos = startPos2;
            if (i == 1)
                startPos = startPos3;
            if (i == 2)
                startPos = startPos4;
            if (i == 3)
                startPos = startPos1;

            if (levelLoaded <= 10)
            {

                //Only load choti Ants

            }



            if (false)
            {
                pinkBall = Instantiate(pinkBallPrefab, startPos.transform.position, Quaternion.identity);
                pinkBall.AddComponent<CollisionDetection>();
                pinkBallRigid = pinkBall.GetComponent<Rigidbody2D>();
                // pinkBallRigid.isKinematic = false;
                pinkBallRigid.gravityScale = 0;
                pinkBall.GetComponent<BallController>().ant_type = "choti";

            }
            else
            {
                blueBall = Instantiate(blueBallPrefab, startPos.transform.position, Quaternion.identity);
                blueBall.AddComponent<CollisionDetection>();
                blueBallRigid = blueBall.GetComponent<Rigidbody2D>();
                // blueBallRigid.isKinematic = false;
                blueBallRigid.gravityScale = 0;
                blueBall.GetComponent<BallController>().ant_type = "choti";

            }
        }

        public void IncreaseScore()
        {
            current_score++;
            TotalScore = total_score;
            UpdateBombs();
        }

        //Create level base on level data
        void CreateLevel(LevelData levelData)
        {
            current_score = 0;
            
            //if (LevelLoaded > 20)
            //{
            //    foreach (ObstacleData o in levelData.listObstacleData)
            //    {
            //        foreach (GameObject a in obstacleManager.obstacles)
            //        {
            //            if (a.name.Equals(o.id))
            //            {
            //                GameObject obstacle = Instantiate(a, o.position, o.rotation) as GameObject;
            //                obstacle.transform.localScale = o.scale;
            //                ConveyorController cv = obstacle.GetComponent<ConveyorController>();
            //                if (cv != null)
            //                {
            //                    cv.rotateSpeed = o.rotatingSpeed;
            //                    cv.rotateDirection = o.rotateDirection;
            //                }
            //                break;
            //            }
            //        }
            //    }
            //}
         
            StartCoroutine(CreateBalls(3f));

        }
        //Create level base on level data
        void CreateLevelOld(LevelData levelData)
        {
            pinkBall = Instantiate(pinkBallPrefab, levelData.pinkBallPosition, Quaternion.identity);
            blueBall = Instantiate(blueBallPrefab, levelData.blueBallPosition, Quaternion.identity);


            //My code
            pinkBall.AddComponent<CollisionDetection>();
            blueBall.AddComponent<CollisionDetection>();


            pinkBallRigid = pinkBall.GetComponent<Rigidbody2D>();
            pinkBallRigid.isKinematic = true;

            //
            pinkBallRigid.gravityScale = 0;

            blueBallRigid = blueBall.GetComponent<Rigidbody2D>();
            blueBallRigid.isKinematic = true;
            //
            blueBallRigid.gravityScale = 0;

            foreach (ObstacleData o in levelData.listObstacleData)
            {
                foreach (GameObject a in obstacleManager.obstacles)
                {
                    if (a.name.Equals(o.id))
                    {
                        GameObject obstacle = Instantiate(a, o.position, o.rotation) as GameObject;
                        obstacle.transform.localScale = o.scale;
                        ConveyorController cv = obstacle.GetComponent<ConveyorController>();
                        if (cv != null)
                        {
                            cv.rotateSpeed = o.rotatingSpeed;
                            cv.rotateDirection = o.rotateDirection;
                        }
                        break;
                    }
                }
            }
        }
        public void UpdateTextAndProgress()
        {

            if ((totalPoints) >= TotalAvailableDots)
            {
                FinishLine();
                allowDrawing = false;
            }
            GameObject.Find("progressBarDots").GetComponent<HealthBar>().healthAmount = (float)(TotalAvailableDots - totalPoints) / TotalAvailableDots;

            if (totalPoints <= TotalAvailableDots)
                statusBar.GetComponent<Text>().text = ((total_num_of_balls) - BallsKilled) + " insects: " + (TotalAvailableDots - totalPoints) + " dots remaining";
            //statusBar.GetComponent<Text>().text = current_score.ToString();
        }

        int GetTotalAvailableDots()
        {
            int total_num_of_points = 5 * ((levelLoaded * 2) - 1); //Points required to kill small ants

            if (levelLoaded > 10)
            {
                total_num_of_points += (10 * ((levelLoaded) / 3)); // Points required to kill big ants
                total_num_of_points += (20 * ((levelLoaded) / 5)); // Points required to kill red ants
                //total_num_of_points += ((levelLoaded) / 5); // Number of Bonus Ants
            }

            int extra_dots = 5;
            if ((levelLoaded > 10) && (levelLoaded <= 20))
                extra_dots = 10;
            if ((levelLoaded > 20) && (levelLoaded <= 30))
                extra_dots = 10;
            if ((levelLoaded > 30) && (levelLoaded <= 40))
                extra_dots = 20;
            if ((levelLoaded > 40) && (levelLoaded <= 50))
                extra_dots = 20;
            if ((levelLoaded > 50) && (levelLoaded <= 60))
                extra_dots = 20;


            return total_num_of_points + extra_dots;
        }


        IEnumerator CreateBalls(float duration)
        {
            total_num_of_balls = (levelLoaded * 2) - 1;
            if(levelLoaded > 10)
            {
                total_num_of_balls += ((levelLoaded)/3 ); // Number of Badi Ants
                total_num_of_balls += ((levelLoaded) / 5 ); // Number of Red Ants
                total_num_of_balls += ((levelLoaded) / 5 ); // Number of Bonus Ants

            }
            isCreatingInsects = true;
            for (int i = 1; i <= (levelLoaded); ++i)
            {


                yield return new WaitForSeconds(duration);   //Wait
                if (levelLoaded <= 10)
                {
                    if (i == 1)
                    {
                        AddAnotherBall("choti");
                        noOfBallsAdded++;
                        continue;
                    }
                    AddAnotherBall("choti");
                    noOfBallsAdded++;
                    AddAnotherBall("choti");
                    noOfBallsAdded++;
                }
                else
                {
                    if (i == 1)
                    {
                        AddAnotherBall("choti");
                        noOfBallsAdded++;
                        continue;
                    }
                    AddAnotherBall("choti");
                    noOfBallsAdded++;
                    AddAnotherBall("choti");
                    noOfBallsAdded++;
                    if ((i % 3) == 0)
                    {
                        AddAnotherBall("badi");
                        noOfBallsAdded++;
                    }
                    if ((i % 5) == 0)
                    {
                        AddAnotherBall("bonus");
                        noOfBallsAdded++;
                        AddAnotherBall("red");
                        noOfBallsAdded++;
                    }

                }
               


            }
            isCreatingInsects = false;

          //  StartCoroutine(RandomizeInsects());
        }
        void DeformTarget(Vector3 point)
        {
            GameObject target = GameObject.Find("Target");
            MeshDeformer deformer = target.GetComponent<MeshDeformer>();
            if(deformer)
            {
                point += new Vector3(0,0,1) * 10;
                deformer.AddDeformingForce(point, 10);
            }
        }


        public void showKilledEffect(Vector2 pos)
        {
            DeformTarget(pos);
            StartCoroutine(ParticleSystemForKilledEffect(pos));
        }
        IEnumerator ParticleSystemForKilledEffect(Vector2 pos)
        {
            killedEffect.transform.position = pos;
            killedEffect.SetActive(true);
            yield return new WaitForSeconds(2);
            killedEffect.SetActive(false);

        }

        public bool isPointInGameArea(Vector3 point)
        {
            //Debug.Log(gameAreaBounds + " = " + point);
            return newBounds.Contains(point);

        }

        //public Rect getGameArea()
        //{
        //    SpriteRenderer  gameAreaSprite = GameObject.Find("GameArea").GetComponent<SpriteRenderer>();
        //    return gameAreaSprite.sprite.bounds.Contains
        //}

        bool checkForLoop(Vector3 pos)
        {
            Vector3[] points_in_current_line = new Vector3[currentLineRenderer.positionCount];
            currentLineRenderer.GetPositions(points_in_current_line);
            for(int i = 0; i < points_in_current_line.Length; ++i)
            {
                if(points_in_current_line[i] == pos)
                {
                    return true;
                }
            }
            return false;

        }

        bool checkifTargetClicked()
        {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {

                if (hit.transform.name == "Target")
                    return true;
            }
            return false;
        }

        void Update()
        {
            //UpdateTextAndProgress();
            //CheckForBombBounds();


            if (!gameOver && !win)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    
                    //Get the button on click
                    GameObject thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                    if (isPointInGameArea(mousePos2D)== false)
                    {
                        allowDrawing = false;
                    }else

                    if (checkifTargetClicked()) {
                        allowDrawing = false;
                    }else

                    if (thisButton != null)//Is click on button
                    {

                        allowDrawing = false;
                    }
                    else if (totalPoints < TotalAvailableDots)//Not click on button
                    {
                        allowDrawing = true;

                        //Is on gameplay mode
                        if (mode == Mode.GameplayMode)
                        {
                            //if (gameplayUIManager.btnHint.activeSelf)
                            //    gameplayUIManager.btnHint.SetActive(false);
                            //if (hintTemp != null)
                            //    Destroy(hintTemp);
                        }
                        stopHolding = false;
                        listPoint.Clear();
                        livePoints = 0;
                        UpdateTextAndProgress();
                        CreateLine(Input.mousePosition);
                    }
                }
                else if (Input.GetMouseButton(0) && !stopHolding && allowDrawing)
                {
                    
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (!listPoint.Contains(mousePos))
                    {
                        //Add mouse pos, set vertex and position for line renderer
                 
                        listPoint.Add(mousePos);

                        currentLineRenderer.positionCount = listPoint.Count;
                        currentLineRenderer.SetPosition(listPoint.Count - 1, listPoint[listPoint.Count - 1]);
                        //currentLineRenderer.

                        //Create collider
                        if ((listPoint.Count >= 2)) // False for dont create collider
                        {

                            Vector2 point_1 = listPoint[listPoint.Count - 2];
                            Vector2 point_2 = listPoint[listPoint.Count - 1];

                            totalPoints++;
                            UpdateTextAndProgress();
                            currentColliderObject = new GameObject("Collider");
                            currentColliderObject.AddComponent<SpriteRenderer>();
                            currentColliderObject.GetComponent<SpriteRenderer>().sprite = collider_sprite;

                            currentColliderObject.transform.position = (point_1 + point_2) / 2;
                            currentColliderObject.transform.right = (point_2 - point_1).normalized;
                            currentColliderObject.transform.SetParent(currentLine.transform);

                            currentBoxCollider2D = currentColliderObject.AddComponent<BoxCollider2D>();
                            currentBoxCollider2D.size = new Vector3((point_2 - point_1).magnitude, 0.1f, 0.1f);
                            currentBoxCollider2D.enabled = false;

                            Vector2 rayDirection = currentColliderObject.transform.TransformDirection(Vector2.right);

                            Vector2 pointDir = currentColliderObject.transform.TransformDirection(Vector2.up);

                            Vector2 rayPoint_1 = (Vector2)currentColliderObject.transform.position + (-rayDirection) * (currentBoxCollider2D.size.x);

                            Vector2 rayPoint_2 = ((Vector2)currentColliderObject.transform.position + pointDir * (currentBoxCollider2D.size.y / 2f))
                                                 + ((-rayDirection) * (currentBoxCollider2D.size.x));

                            Vector2 rayPoint_3 = ((Vector2)currentColliderObject.transform.position + (-pointDir) * (currentBoxCollider2D.size.y / 2f))
                                                 + ((-rayDirection) * (currentBoxCollider2D.size.x));

                            float rayLength = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - rayPoint_1).magnitude;

                            RaycastHit2D hit_1 = Physics2D.Raycast(rayPoint_1, rayDirection, rayLength);
                            RaycastHit2D hit_2 = Physics2D.Raycast(rayPoint_2, rayDirection, rayLength);
                            RaycastHit2D hit_3 = Physics2D.Raycast(rayPoint_3, rayDirection, rayLength);

                            if (hit_1.collider != null || hit_2.collider != null || hit_3.collider != null)
                            {
                                GameObject hit = (hit_1.collider != null) ? (hit_1.collider.gameObject) :
                                                ((hit_2.collider != null) ? (hit_2.collider.gameObject) : (hit_3.collider.gameObject));
                                if (currentColliderObject.transform.parent != hit.transform.parent)
                                {
                                    Destroy(currentBoxCollider2D.gameObject);
                                    currentLineRenderer.positionCount = (listPoint.Count - 1);
                                    listPoint.Remove(listPoint[listPoint.Count - 1]);
                                    //if (pinkBall && pinkBallRigid.isKinematic)
                                    //    pinkBallRigid.isKinematic = false;
                                    //if (blueBall && blueBallRigid.isKinematic)
                                    //    blueBallRigid.isKinematic = false;

                                    for (int i = 0; i < currentLine.transform.childCount; i++)
                                    {
                                        currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                             
                                    }

                                    if (mode == Mode.LevelEditorMode)
                                    {
                                        levelManager.listLineRendererPos = listPoint;
                                    }

                                    listLine.Add(currentLine);

                                    //currentLine.AddComponent<Rigidbody2D>().useAutoMass = true;
                                    foreach (Rigidbody2D rigid in listObstacleNonKinematic)
                                    {
                                       // rigid.isKinematic = false;
                                    }
                                    stopHolding = true;
                                }
                                else
                                {
                                
                                }
                            }
                        }
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !stopHolding && allowDrawing)
                {
                    UpdateTextAndProgress();

                    if (currentLine.transform.childCount > 0)
                    {
                        for (int i = 0; i < currentLine.transform.childCount; i++)
                        {
                            currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                        }
                        listLine.Add(currentLine);
                     
                    }
                    else
                    {
                            Destroy(currentLine);
                    }


                }
            }

           // AddRandomMovement(pinkBallRigid); AddRandomMovement(blueBallRigid);

        }

        void FinishLine()
        {
            if (currentLine && currentLine.transform.childCount > 0)
            {
                for (int i = 0; i < currentLine.transform.childCount; i++)
                {
                    if(currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>())
                        currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
                listLine.Add(currentLine);

            }
            else
            {
                Destroy(currentLine);
            }
        }

        public void setGestureText(string text)
        {
            return; //Disabling blue line for now [TODO: Think about this]
            if (currentLine == false)
                return;
            if ((text == "heart") || (text == "Circle"))
            {
                foreach (Transform child in currentLine.transform)
                {
                    if (child.gameObject.name != "Collider")
                    {
                        continue;
                    }
                    child.gameObject.name = "InvincibleCollider";
                    child.gameObject.tag = "InvincibleCollider";
                    //child.gameObject.GetComponent<SpriteRenderer>().sprite = invincibleCollider;
                    child.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                    Instantiate(invincible_animation, child.gameObject.transform.position, Quaternion.identity, child.gameObject.transform.parent);
                }
                StartCoroutine(deleteCurrentLine(currentLine,5));
            }
            else
                StartCoroutine(deleteCurrentLine(currentLine, 15));

        }

        IEnumerator deleteCurrentLine(GameObject line, float after_duration)
        {

            yield return new WaitForSeconds(after_duration);
            //foreach (Transform child in line.transform)
            //{
            //    child.gameObject.name = "Collider";
            //    child.gameObject.tag = "Untagged";
            //    child.gameObject.GetComponent<SpriteRenderer>().sprite = collider_sprite;
            //    yield return new WaitForSeconds(0.1f);   //Wait
            //}
            listLine.Remove(line);
            Destroy(line);
        }

        void CreateLine(Vector2 mousePosition)
        {
            currentLine = new GameObject("Line");
            currentLineRenderer = currentLine.AddComponent<LineRenderer>();
            currentLineRenderer.sharedMaterial = lineMaterial;
            currentLineRenderer.positionCount = 0;
            currentLineRenderer.startWidth = 0.04f;
            currentLineRenderer.endWidth = 0.04f;
            currentLineRenderer.startColor = lineColor;
            currentLineRenderer.endColor = lineColor;
            currentLineRenderer.useWorldSpace = false;

            //lines.Add(currentLine);

        }


        public void StopAllPhysics()
        {
            if (pinkBall)
            {
                pinkBallRigid.bodyType = RigidbodyType2D.Kinematic;
                pinkBallRigid.simulated = false;
            }
            if (blueBall)
            {
                blueBallRigid.bodyType = RigidbodyType2D.Kinematic;
                blueBallRigid.simulated = false;
            }
            //for (int i = 0; i < listLine.Count; i++)
            //{
            //    Rigidbody2D rigid = listLine[i].GetComponent<Rigidbody2D>();
            //    if (rigid)
            //    {
            //        rigid.bodyType = RigidbodyType2D.Kinematic;
            //        rigid.simulated = false;
            //    }
            //}
        }

        // Capture a screenshot when game ends
        private IEnumerator CRTakeScreenshot()
        {
            // Capture the screenshot that is 2x bigger than the displayed one for crisper image on retina displays
            int height = LevelManager.ssHeight * 2;
            int width = (int)(height * Screen.width / Screen.height);
            RenderTexture rt = new RenderTexture(width, height, LevelManager.bitType, RenderTextureFormat.ARGB32);
            yield return new WaitForEndOfFrame();
            Camera.main.targetTexture = rt;
            Camera.main.Render();
            yield return null;
            Camera.main.targetTexture = null;
            yield return null;

            RenderTexture.active = rt;
            Texture2D tx = new Texture2D(width, height, TextureFormat.ARGB32, false);
            tx.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tx.Apply();
            RenderTexture.active = null;
            Destroy(rt);

            byte[] bytes = tx.EncodeToPNG();

            // Store the screenshot with the level number and overwrite the old level screenshot if win, otherwise store a failed screenshot
            string filename = win ? LevelLoaded.ToString() + ".png" : failedScreenshotName;
            string imgPath = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllBytes(imgPath, bytes);
        }

        public bool ShowHint()
        {
            // Only show hint if there's enough hearts
            if (CoinManager.Instance.Coins >= heartsPerHint)
            {
                string path = LevelScroller.JSON_PATH;
                TextAsset textAsset = Resources.Load<TextAsset>(path);
                string[] data = textAsset.ToString().Split(';');
                foreach (string o in data)
                {
                    LevelData levelData = JsonUtility.FromJson<LevelData>(o);
                    if (levelData.levelNumber == LevelLoaded)
                    {
                        hintTemp = Instantiate(hintPrefab, levelData.hintData.position, levelData.hintData.rotation) as GameObject;
                        hintTemp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Hints/hint" + LevelLoaded.ToString());
                        hintTemp.gameObject.transform.localScale = levelData.hintData.scale;

                        // Remove hearts
                        CoinManager.Instance.RemoveCoins(heartsPerHint);

                        return true;
                    }
                }
            }

            return false;
        }
    }


}
