using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodspawn : MonoBehaviour
{
	public GameObject[] _foodPrefabArray;
    public Vector2 Min;
    public Vector2 Max;
    public int food;
    public int max_food;
    private Vector3 myFoodPos;

    void Start()
    {   
        InvokeRepeating("MyFoodSpawn", 0f, 0.5f);
    }

    void MyFoodSpawn()
    {
        var i = Random.Range(0, _foodPrefabArray.Length);

        myFoodPos = new Vector3(Random.Range(Min.x, Max.x), Random.Range(Min.y, Max.y), 0);

        if (food < max_food)
        {
            var randomRandomClone = Instantiate(_foodPrefabArray[i], myFoodPos, transform.rotation).GetComponent<Rigidbody2D>();
            ++food;
        }
    }
}
