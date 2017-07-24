using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class player : NetworkBehaviour {
    public float score;
	public Text txt;
    public float scale;
    public foodspawn go;

    public float MassProperty {
		get {
			var parameter =  1/Mathf.InverseLerp( 0f , 100f , score + 10) ;
			return parameter ;
		}
	}

    void Start ()
    {

        //var TxtClone = Instantiate(, 0, 0).GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
        if (!isLocalPlayer)
            return;
        scale = 0.5f + (score / 100);
        transform.localScale = new Vector3(scale, scale, scale);
    //    txt.text = score.ToString();
    }

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Food")
        {
           // if (score < 100)
			 //   score += 1;
            //go.food -= 1;
            Destroy (other.gameObject);
        }
    }
}
