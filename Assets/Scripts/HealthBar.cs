using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBarPivot;
    public Image fillImage;
    public bool hideImageInFullHealth=true ;
    // Start is called before the first frame update
    void Start()
    {
      if (GetComponent<Obstacle>())
        GetComponent<Obstacle>().OnTakeDamage +=UpdateHealthBar ;
        if (GetComponent<Character >())
            GetComponent<Character >().OnTakeDamage += UpdateHealthBar;
        if (hideImageInFullHealth)
            healthBarPivot.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthBar(float ratio)
    {
        fillImage.fillAmount = ratio;
        healthBarPivot.gameObject.SetActive(true );
        // healthBarPivot.gameObject.transform.LookAt(Camera.main .transform .position );
    }
}
