using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class rotate : NetworkBehaviour {
    /*
    public static float SPEED_MAX = 60;
    public static float SPEED_MIN = 30;

    public WatchableGame game;
    public float speed = SPEED_MIN;

    private Collider2D _collider;
    public GameObject overlayLeft;
    private Bounds overlayBoundsLeft;
    public GameObject overlayRight;
    private Bounds overlayBoundsRight;

    private SpriteRenderer _renderer;

    public Score scoreLeft;
    public Score scoreRight;

	void Start () {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;

	}

    float HitFactor(Vector2 Ball_Pos, Vector2 Racket_Pos, float Racket_Height) {
        return ((Ball_Pos.y - Racket_Pos.y) / Racket_Height);
    }

    void Update() {
        

    }

    void OnCollisionEnter2D(Collision2D col) {

        if (col.gameObject.name == "racket_left")
        {
            speed = Mathf.Min(speed + 7f, SPEED_MAX);

            float y = HitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
            Vector2 dir = new Vector2(1, y).normalized;
            GetComponent<Rigidbody2D>().velocity = dir * speed;
            GetComponent<TrailRenderer>().material.SetColor("_EmissionColor", new Color(0f, 240f, 210f, 1f));
        }
        if (col.gameObject.name == "racket_right")
        {
            speed = Mathf.Min(speed + 7f, SPEED_MAX);

            float y = HitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
            Vector2 dir = new Vector2(-1, y).normalized;
            GetComponent<Rigidbody2D>().velocity = dir * speed;
            GetComponent<TrailRenderer>().material.SetColor("_EmissionColor", new Color(255f, 0f, 100f, 1f));
        }
        if (col.gameObject.name == "wall_right") {
            GetComponent<Rigidbody2D>().position = new Vector3(0, 0, 0);
            GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
            GetComponent<TrailRenderer>().Clear();

            scoreLeft.OnScoreInc(1);

            speed = SPEED_MIN;

            Camera.main.GetComponent<CamShakeSimple>().OnShakeOnCollision(col, .025f);
        }
        if (col.gameObject.name == "wall_left") {
            GetComponent<Rigidbody2D>().position = new Vector3(0, 0, 0);
            GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
            GetComponent<TrailRenderer>().Clear();

            scoreRight.OnScoreInc(1);

            speed = SPEED_MIN;

            Camera.main.GetComponent<CamShakeSimple>().OnShakeOnCollision(col, .025f);
        }
    }
    */

    enum rotation
    {
        positive,
        negative,
        stop
    };

    public float time;
    public float speed;
    rotation dir;
    bool isAlready;
    float angle;
    float sin;
    float cos;
    float velo;
    Vector3 forward;
    Vector2 last_forward;
    Vector3 direction;
    int nb_col;
    int last_nb_col;
    int nb_victim;
    bool isCol;
    System.DateTime timer;
    public Rigidbody2D rb;

    void Start()
    {
       if (isLocalPlayer)
        {
            nb_col = 0;
            last_nb_col = nb_col;
            nb_victim = 0;
            isCol = false;
            velo = 50;
            //GetComponent<Rigidbody2D>().AddForce(forward * Time.deltaTime * velo);
            rb = GetComponent<Rigidbody2D>();
            dir = rotation.positive;
            isAlready = false;
            direction = Vector2.up;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

          //SearchTarget();
        if (isCol == true && timer <= System.DateTime.Now)
        {
            isCol = false;
        }

        if (nb_col == last_nb_col && nb_col > 0)
            ++nb_victim;
        else
            nb_victim = 0;

        if (nb_victim == 10)
        {
            nb_victim = 0;
            Receive_knockback();
        }

        if (Input.GetKey(KeyCode.Space) && isCol == false)
        {
            angle = transform.eulerAngles.z * Mathf.Deg2Rad;
            sin = Mathf.Sin(angle);
            cos = Mathf.Cos(angle);

            if (isAlready == false)
                forward = new Vector3(
                    direction.x * cos - direction.y * sin,
                    direction.x * sin + direction.y * cos,
                    0f);
            if (velo < 2000)
                velo += 2f;
            GetComponent<Rigidbody2D>().velocity = forward * Time.deltaTime * velo;
            
            //transform.position += forward * Time.deltaTime * speed;
            if (isAlready == false)
            {
                if (dir == rotation.positive)
                    dir = rotation.negative;
                else if (dir == rotation.negative)
                    dir = rotation.positive;
            }
            isAlready = true;
        }
        else
        {
            last_forward = GetComponent<Rigidbody2D>().velocity;
            if (velo > 3)
                velo -= 4f;
            //GetComponent<Rigidbody2D>().velocity = forward * Time.deltaTime * speed * velo;
            //GetComponent<Rigidbody2D>().velocity
            if (dir == rotation.positive)
                transform.Rotate(Vector3.forward * Time.deltaTime * time, Space.Self); //* (pone.MassProperty/5)
            else if (dir == rotation.negative)
                transform.Rotate(Vector3.back * Time.deltaTime * time, Space.Self);
            isAlready = false;
        }
    }
    
    void SearchTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.85f);
        Collider2D target;
        int i = 0;
        int nb = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Player")
            {
                ++nb;
                target = hitColliders[i];
                if (nb >= 2)
                {
                    transform.rotation = FaceObject(transform.position, target.transform.position, 90);
                    //if (isCol == false)
                    //    timer = System.DateTime.Now.AddSeconds(1);
                    //isCol = true;
                    nb = 0;
                    return;
                }
            }
            ++i;
        }
    }

    public static Quaternion FaceObject(Vector2 startingPosition, Vector2 targetPosition, int facing)
    {
        Vector2 direction = targetPosition - startingPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= (float)facing;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /*void OnCollisionEnter2D(Collision2D coll)
    {
     //   transform.rotation = FaceObject(transform.position, coll.contacts[0].point, 90);

        if (coll.gameObject.tag == "Finish")
        {
            Debug.Log("lol");
        }
        else if (coll.gameObject.tag != "Food")
        {
            //GetComponent<Rigidbody2D>().AddForceAtPosition(GetComponent<Rigidbody2D>().velocity, coll.contacts[0].point, );
            //.gameObject.GetComponent<Rigidbody2D>().velocity += GetComponent<Rigidbody2D>().velocity;
            isCol = true;
            timer = System.DateTime.Now.AddSeconds(5);
            
            //            GetComponent<Rigidbody2D>().velocity = forward * Time.deltaTime * velo;
        }
    }*/


    void Receive_knockback()
    {
        /*other.gameObject.GetComponent<rotate>().isCol = true;
        other.gameObject.GetComponent<rotate>().nb_col = 0;
        other.gameObject.GetComponent<rotate>().timer = System.DateTime.Now.AddSeconds(5);*/
        isCol = true;
        nb_col = 0;
        timer = System.DateTime.Now.AddSeconds(5);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Finish")
        {
            Debug.Log("lol");
        }
        else if (other.gameObject.tag != "Food")
        {
            //GetComponent<Rigidbody2D>().AddForceAtPosition(GetComponent<Rigidbody2D>().velocity, coll.contacts[0].point, );
            //.gameObject.GetComponent<Rigidbody2D>().velocity += GetComponent<Rigidbody2D>().velocity;
            ++nb_col;
            last_nb_col = nb_col;
            if (nb_col >= 10)
                Receive_knockback();
        }
    }


    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<camera>().setTarget(gameObject.transform);
    }
}
