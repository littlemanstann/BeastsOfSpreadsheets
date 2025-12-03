using Unity.VisualScripting;
using UnityEngine;

public class star : MonoBehaviour
{

    public SpriteRenderer star_base;
    public SpriteRenderer star_yellow;
    public ParticleSystem parts;

    public int level_no = -1;


    public static bool[] levels_completed = { false, false, false, false, false };

    float r; //random

    float falling = -1;

    bool done = false;
   

   Vector3 start, end;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //star_yellow.color = new Color(255,255,255,0);
        var v = star_base.transform.up;
        start = star_base.transform.position + (v * 4);
        end = star_base.transform.position;
        star_yellow.transform.position = start;
        parts.Stop();
        r = Random.Range(1.5f, 2.5f);



        if (levels_completed[level_no])
        {
            yippee();
        }
        else
        {
            star_yellow.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        if (falling > -0.5)
        {
            falling += Time.deltaTime;
            var t = Mathf.Pow(falling * r, 3);
            star_yellow.transform.position = Vector3.Lerp(start, end, t);
            if (t > 1)
            {   
                if (!done)
                {
                    done = true;
                    parts.Play();
                }
                float s = Mathf.Lerp(1.3f, 1, t - 1);
                star_base.transform.localScale = new Vector3(s, s, s);
            }
        }
        
    }

    void yippee()
    {
        falling = 0;
    }


 



}
