using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderDropper : MonoBehaviour {

    public int boulderAmount = 5;
    public GameObject[] boulders;
    public int lapsUntilActivation = 1;
    public Transform location;
    public float locationVariance = 1f;
    public float minSize = 1;
    public float maxSize = 2;

    private void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Player") {
            lapsUntilActivation--;
            if(lapsUntilActivation < 0) {
                DropBoulders();
                Destroy(gameObject);
            }
        }      
    }

    void DropBoulders(){
        for(int i = 0; i < boulderAmount; i++) {
            var thisBoulder = Instantiate(boulders[Random.Range(0, boulders.Length - 1)], location.position + new Vector3(Random.Range(-locationVariance, locationVariance), 0, Random.Range(-locationVariance, locationVariance)), Quaternion.identity);
            var s = Random.Range(minSize, maxSize);
            thisBoulder.transform.localScale = new Vector3(s, s, s);
        }
    }
}
