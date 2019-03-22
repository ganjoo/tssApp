using UnityEngine;
using System.Collections;
using EasyMobile;
using System.Collections.Generic;

namespace DrawDotGame
{
    public class BallController : MonoBehaviour
    {
        private GameManager gameManager;
        public string ant_type; // "choti", "badi", "red", "bonus ---- A choti Ant dies in 5 unit ---- A badi Ant dies in 10 units 
                                // A red Ant dies in 20 units Bonus Ant requires 1 unit to die and gives back 200 units


        public ParticleSystem part;
        public List<ParticleCollisionEvent> collisionEvents;

        private Rigidbody2D rigid2D;
        public AudioClip impact;
        public AudioClip kill;
        public AudioSource audioSource;
        private GameObject progressDots;
        private GameObject progressInsects;
        int count = 0;
        private int Hits = 0;
        private bool markKilled = false;
        private bool isEating = false;
        private bool collided = false;
        private GameObject current_line_eaten;


        public delegate void FirstLevelCompleted();
        public static FirstLevelCompleted firstLevelDone;

        private Vector2 _random_direction;
        public bool random;
       public float speed;
        void Start()
        {
            GameObject[] fake_targets = GameObject.FindGameObjectsWithTag("faketarget");
            gameManager = FindObjectOfType<GameManager>();
            rigid2D = GetComponent<Rigidbody2D>();
            rigid2D.isKinematic = false;
            audioSource = GameObject.Find("GameManager").GetComponent<AudioSource>();
            isEating = false;
            progressDots = GameObject.Find("progressBarDots");
            progressInsects = GameObject.Find("progressBarInsects");
            Hits = 0;
            markKilled = false;
            collided = false;
            count = 0;
            //  ant_type = "choti";
            _random_direction = gameManager.getRandomPointInGameArea();
            random = false;
            gameObject.GetComponent<CircleCollider2D>().isTrigger = false;

            if(0 == (Random.Range(0,100) % 4))
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1f);
            else
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);




            speed = 1f;
            if (ant_type == "choti")
                speed = 3f;
            if (ant_type == "badi")
                speed = 1f;
            if (ant_type == "red")
                speed = 0.4f;
            if (ant_type == "bonus")
                speed = 0.9f;
        }

        IEnumerator moveObjectTowardsTarget( Vector3 targetPos, float speed)
        {


            yield return new WaitForSeconds(1);

            Vector2 _direction = (targetPos - transform.position);
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            //create the rotation we need to be in to look at the target
            Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);

            // rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 101);
            rigid2D.MovePosition(rigid2D.position + _direction * Time.fixedDeltaTime * speed);
        }

        IEnumerator fixedUpdateThread()
        {
            yield return new WaitForSeconds(0.1f);
            
            Vector2 pos = transform.position;
            if ((count++ % 30) == 0)
            {                
                //Every 20th frame
                if (!isEating && rigid2D)
                {
                    //random = false;
                    if(GameManager.LevelLoaded >= 7)
                        speed = Random.Range(0.1f, 4f);

                }
            }
            else if (isEating)
            {
                MoveTowardsNextDotinCurrentLine();
            }
            else if (random)//Randomly moving
            {

                if(((count++ % 500) == 0)) //Change direction every 2000th frame
                {
                    _random_direction = gameManager.getRandomPointInGameArea();
                }
                //
                StartCoroutine(moveObjectTowardsTarget(_random_direction,speed/10));

            }
            else if (!isEating && rigid2D)
            {
                gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
                //gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                Vector2 _direction = (gameManager.getTargetPos() - pos).normalized;
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                //create the rotation we need to be in to look at the target
                Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);

                // rotate us over time according to speed until we are in the required rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 101);


                rigid2D.MovePosition(rigid2D.position + _direction * Time.fixedDeltaTime * speed);

            }

   
        }
        void Update()
        {
            StartCoroutine(fixedUpdateThread());
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle"))
            {
                return;
            }

            if (!gameManager.gameOver)
            {
                if (LayerMask.NameToLayer("Dead") == other.gameObject.layer)
                {
                    ParticleSystem particle = Instantiate(gameManager.explores, transform.position, Quaternion.identity) as ParticleSystem;
#if UNITY_5_5_OR_NEWER
                    var main = particle.main;
                    main.startColor = GetComponent<SpriteRenderer>().color;
                    particle.Play();
                    Destroy(particle.gameObject, main.startLifetimeMultiplier);
#else
                particle.startColor = GetComponent<SpriteRenderer>().color;
                particle.Play();
                Destroy(particle.gameObject, particle.startLifetime);
#endif
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    rigid2D.isKinematic = true;
                    gameManager.GameOver();
                }
            }
        }
        Transform GetClosestPrey(Transform[] enemies)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Transform potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }
        IEnumerator WaitAndDestroy(float duration, GameObject objToDestroy)
        {
            //This is a coroutine
         //   Debug.Log("Start Wait() function. The time is: " + Time.time);
           // Debug.Log("Float duration = " + duration);
            yield return new WaitForSeconds(duration);   //Wait
            //Debug.Log("End Wait() function and the time is: " + Time.time);
            Destroy(objToDestroy);
        }

        void GameWon(Collision2D col)
        {
            if (GameManager.LevelLoaded == 1)
                if(firstLevelDone != null)
                    firstLevelDone();
            gameManager.Win();

            Vector3 thisPos = this.transform.position;
            Vector3 thatPos = col.transform.position;
            Vector3 midPoint = thisPos + (thatPos - thisPos) / 2;

            ParticleSystem particle = Instantiate(gameManager.winning, midPoint, Quaternion.identity) as ParticleSystem;
            particle.Play();
#if UNITY_5_5_OR_NEWER
            Destroy(particle.gameObject, particle.main.startLifetimeMultiplier);
#else
                Destroy(particle.gameObject, particle.startLifetime);
#endif

        }

        void GameWon(Vector3 particlePos)
        {
            if (GameManager.LevelLoaded == 1)
                firstLevelDone();
            gameManager.Win();
            ParticleSystem particle = Instantiate(gameManager.winning, particlePos, Quaternion.identity) as ParticleSystem;
            particle.Play();
#if UNITY_5_5_OR_NEWER
            Destroy(particle.gameObject, particle.main.startLifetimeMultiplier);
#else
                Destroy(particle.gameObject, particle.startLifetime);
#endif
        }

        public void PlayEatAudio()
        {
            audioSource.Stop();
            audioSource.PlayOneShot(impact, 1F); 
        }
        void PlayKillAudio()
        {
            audioSource.Stop();
            audioSource.PlayOneShot(kill, 1F);
        }

        void MoveTowardsAntAttractor()
        {
            //Basically follow a path until all ants are created
        }

        void MoveTowardsNextDotinCurrentLine()
        {
            if(current_line_eaten == null)
            {
                isEating = false;
                return;
            }
            Transform line_transform = current_line_eaten.transform;
            if (line_transform.childCount > 1)
            {
                isEating = true;
                Transform[] childTransform = new Transform[line_transform.childCount];
                int i = 0;
                //Debug.Log(line_transform.childCount + " = Number of Children of This line");
                for (; i < line_transform.childCount; ++i)
                {
                   // Debug.Log("Getting Child " + i + line_transform.transform.GetChild(i));
                    childTransform[i] = line_transform.GetChild(i);
                }
                if (i == 0)
                {

                    Destroy(current_line_eaten);
                }

                Transform next_prey = GetClosestPrey(childTransform);


                //Uncomment following line to make it charge towards any dot of any line and not just this line
                //Transform next_prey = GetClosestPrey(gameManager.getAllLineTransforms());


                Vector2 _direction = (next_prey.position - transform.position).normalized;
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                //create the rotation we need to be in to look at the target
                Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);

                // rotate us over time according to speed until we are in the required rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 101);

                this.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
                this.GetComponent<Rigidbody2D>().Sleep();
                //this.GetComponent<Rigidbody2D>().AddForce(next_prey.position - transform.position);



                rigid2D.MovePosition(rigid2D.position + _direction * Time.fixedDeltaTime * 2f);

                               
            }
            else
                isEating = false;

        }
        void MoveTowardsNextDot()
        {
            if (gameManager.getAllLineTransforms().Length < 1)
                return;

            Transform next_prey = GetClosestPrey(gameManager.getAllLineTransforms());
            Vector2 _direction = (next_prey.position - transform.position).normalized;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            //create the rotation we need to be in to look at the target
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            // rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 0.1f);

            //this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
            gameObject.GetComponent<Rigidbody2D>().Sleep();
            if (!gameObject.GetComponent<Rigidbody2D>().isKinematic)
            {
                gameObject.GetComponent<Rigidbody2D>().AddForce(next_prey.position - transform.position);

            }
        }

        public int getHitsToKill()
        {
            switch (ant_type)
            {
                case "badi":
                    return 10;
                case "choti":
                    return 5;
     
                case "red":
                    return 50;
                case "bonus":
                    return 1;
            }
            return 5; //Default
        }

        public void Kill()
        {
            markKilled = true; //To prevent multiple calls after collision with many colliders
            PlayKillAudio();
            gameManager.showKilledEffect(transform.position);
            StartCoroutine(WaitAndDestroy(0.1f, gameObject));
        }

        void OnParticleCollision(GameObject other)
        {
            if (!gameManager.win && !gameManager.gameOver && !markKilled)
            {
                collided = true;
                PlayEatAudio();
                Hits++;
                gameManager.IncreaseScore();
                gameObject.transform.Find("healthbar").GetComponent<HealthBar>().healthAmount = (float)(GameManager.LevelLoaded - Hits) / GameManager.LevelLoaded;
                if (Hits == getHitsToKill()) //Kill this object
                {
                    Kill();
                    if (ant_type == "bonus")
                    {
                        gameManager.bonusPointsCaptured += gameManager.bonusAntValue;
                        gameManager.TotalAvailableDots += gameManager.bonusAntValue;
                    }

                    gameManager.PointsScored = gameManager.PointsScored + getHitsToKill();
                    gameManager.BallsKilled++;

                    gameManager.UpdateTextAndProgress();
                    if (gameManager.BallsKilled >= gameManager.total_num_of_balls)
                        GameWon(transform.position);
                    progressInsects.GetComponent<HealthBar>().healthAmount = ((float)(gameManager.total_num_of_balls) - GameObject.Find("GameManager").GetComponent<GameManager>().BallsKilled) / (gameManager.total_num_of_balls);


                }
            }
        }

        void OnCollisionEnter2D(Collision2D col)
        {

            if (!gameManager.win && !gameManager.gameOver && !markKilled)
            {
                if ((col.collider.gameObject.name == "InvincibleCollider") ||(col.collider.gameObject.name == "Collider") || ((col.collider.gameObject.tag == "InvincibleCollider")))
                {
                    //Get Parent
                
                    if (col.collider.gameObject.transform.parent == null) //Check bug TODO
                        return;
                    GameObject parentLine = col.collider.gameObject.transform.parent.gameObject;
                    collided = true;
                    PlayEatAudio();

                    //parentLine.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
                    if ((col.collider.gameObject.tag != "InvincibleCollider") && (col.collider.gameObject.name != "InvincibleCollider"))
                        Destroy(col.collider.gameObject);
                    if (!isEating)
                    {
                        GameObject gobj = GameObject.Find("GameManager");
                         Transform line_transform = parentLine.transform;

                        if (line_transform.childCount > 1)
                        {
                            isEating = true;
                            current_line_eaten = parentLine;
                            Transform[] childTransform = new Transform[line_transform.childCount];
                            int i = 0;
                            //Debug.Log(line_transform.childCount + " = Number of Children of This line");
                            for (; i < line_transform.childCount; ++i)
                            {
                                //Debug.Log("Getting Child " + i + parentLine.transform.GetChild(i));
                                childTransform[i] = line_transform.GetChild(i);
                            }
                            if (i == 0)
                            {
                                gobj.GetComponent<GameManager>().randomMovement = true;
                                Destroy(parentLine);
                            }

                            Transform next_prey = GetClosestPrey(childTransform);


                            //Uncomment following line to make it charge towards any dot of any line and not just this line
                            //Transform next_prey = GetClosestPrey(gameManager.getAllLineTransforms());


                            Vector2 _direction = (next_prey.position - transform.position).normalized;
                            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                            //create the rotation we need to be in to look at the target
                            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

                            // rotate us over time according to speed until we are in the required rotation
                            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 101);

                            //this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                            this.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
                            this.GetComponent<Rigidbody2D>().Sleep();
                            this.GetComponent<Rigidbody2D>().AddForce(next_prey.position - transform.position);
                            //rigid2D.MovePosition(next_prey.position * Time.deltaTime);
                            // transform.position = next_prey.position;
                        }
         

                    }

                    Hits++;
                    gameManager.IncreaseScore();
                    gameObject.transform.Find("healthbar").GetComponent<HealthBar>().healthAmount = (float)(GameManager.LevelLoaded - Hits)/ GameManager.LevelLoaded;
                    if (Hits == getHitsToKill()) //Kill this object
                    {
                        Kill();
                        if (ant_type == "bonus")
                        {
                            gameManager.bonusPointsCaptured += gameManager.bonusAntValue;
                            gameManager.TotalAvailableDots += gameManager.bonusAntValue;
                        }
              
                        gameManager.PointsScored = gameManager.PointsScored + getHitsToKill();
                        gameManager.BallsKilled++;

                        gameManager.UpdateTextAndProgress();
                        if(gameManager.BallsKilled >= gameManager.total_num_of_balls)
                            GameWon(col);
                        progressInsects.GetComponent<HealthBar>().healthAmount = ((float)(gameManager.total_num_of_balls) - GameObject.Find("GameManager").GetComponent<GameManager>().BallsKilled) / (gameManager.total_num_of_balls);


                    }
                   


                }
//                if (col.gameObject.CompareTag("Ball1"))
//                {
//                    gameManager.Win();

//                    Vector3 thisPos = this.transform.position;
//                    Vector3 thatPos = col.transform.position;
//                    Vector3 midPoint = thisPos + (thatPos - thisPos) / 2;

//                    ParticleSystem particle = Instantiate(gameManager.winning, midPoint, Quaternion.identity) as ParticleSystem;
//                    particle.Play();
//#if UNITY_5_5_OR_NEWER
//                    Destroy(particle.gameObject, particle.main.startLifetimeMultiplier);
//#else
//                Destroy(particle.gameObject, particle.startLifetime);
//#endif

//                }
//                else if (col.gameObject.CompareTag("Obstacle") )
//                {
//                    if (!gameManager.gameOver)
//                    {
//                        if (LayerMask.NameToLayer("Dead") == col.gameObject.layer)
//                        {
//                            ParticleSystem particle = Instantiate(gameManager.explores, transform.position, Quaternion.identity) as ParticleSystem;
//#if UNITY_5_5_OR_NEWER
//                            var main = particle.main;
//                            main.startColor = GetComponent<SpriteRenderer>().color;
//                            particle.Play();
//                            Destroy(particle.gameObject, main.startLifetimeMultiplier);
//#else
//                        particle.startColor = GetComponent<SpriteRenderer>().color;
//                        particle.Play();
//                        Destroy(particle.gameObject, particle.startLifetime);
//#endif
//                            gameObject.GetComponent<SpriteRenderer>().enabled = false;
//                            rigid2D.isKinematic = true;
//                            gameManager.GameOver();
//                        }
//                    }
//                }
                else if (col.gameObject.CompareTag("Save"))
                {

                    if (gameManager.showKilledEffectForTarget == true)
                    {
                
                        //Find contact points of this collision and draw a gameobject there
                        int points_of_contact_count = col.contactCount;
                        ContactPoint2D[] points_contact = new ContactPoint2D[points_of_contact_count];
                        col.GetContacts(points_contact);
                        gameManager.showKilledEffect(points_contact[points_of_contact_count - 1].point);
                    }

                        col.gameObject.transform.localScale /= 1.01f;
                    if ((GameManager.LevelLoaded <= 1)  || (col.gameObject.transform.localScale.x > 0.1f))
                    {
                        PlayEatAudio();
                        return;
                    }
                    Debug.Log("Gamer over");
                    if (!gameManager.gameOver)
                    {
                        if (true)
                        {
                            ParticleSystem particle = Instantiate(gameManager.explores, transform.position, Quaternion.identity) as ParticleSystem;
#if UNITY_5_5_OR_NEWER
                            var main = particle.main;
                            main.startColor = Color.blue;
                            particle.Play();
                            Destroy(particle.gameObject, main.startLifetimeMultiplier);
#else
                            particle.startColor = GetComponent<SpriteRenderer>().color;
                            particle.Play();
                            Destroy(particle.gameObject, particle.startLifetime);
#endif
                         //   gameObject.GetComponent<SpriteRenderer>().enabled = false;
                            rigid2D.isKinematic = true;
                            gameManager.GameOver();
                        }
                    }
                }
                else if (col.gameObject.CompareTag("Obstacle"))
                {

                    if ( true)
                    {

                        //Find contact points of this collision and draw a gameobject there
                        int points_of_contact_count = col.contactCount;
                        ContactPoint2D[] points_contact = new ContactPoint2D[points_of_contact_count];
                        col.GetContacts(points_contact);
                        gameManager.showKilledEffect(points_contact[points_of_contact_count - 1].point);
                    }

                    col.gameObject.transform.localScale /= 1.01f;
                    if (col.gameObject.transform.localScale.x > 0.1f)
                    {
                        PlayEatAudio();
                    }

                }
                //                else if ( col.gameObject.CompareTag("Ball"))
                //                {
                //                    if (!gameManager.gameOver)
                //                    {
                //                        if (true)
                //                        {
                //                            ParticleSystem particle = Instantiate(gameManager.explores, transform.position, Quaternion.identity) as ParticleSystem;
                //#if UNITY_5_5_OR_NEWER
                //                            var main = particle.main;
                //                            main.startColor = GetComponent<SpriteRenderer>().color;
                //                            particle.Play();
                //                            Destroy(particle.gameObject, main.startLifetimeMultiplier);
                //#else
                //                        particle.startColor = GetComponent<SpriteRenderer>().color;
                //                        particle.Play();
                //                        Destroy(particle.gameObject, particle.startLifetime);
                //#endif
                //                            gameObject.GetComponent<SpriteRenderer>().enabled = false;
                //                            rigid2D.isKinematic = true;
                //                            gameManager.GameOver();
                //                        }
                //                    }
                //                }
            }
        }
    }
}
