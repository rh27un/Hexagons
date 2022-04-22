using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blood : MonoBehaviour
{
    protected Image bloodImage;
    protected float alpha;
    public float fade;
    protected Color clear = new Color(1f, 1f, 1f, 0f);
    public bool isDead;

    private void Awake()
	{
        bloodImage = GetComponent<Image>();
	}
	// Update is called once per frame
	void Update()
    {
        if (!isDead)
        {
            alpha = Mathf.Clamp(alpha - fade * Time.deltaTime, 0f, 1f);
            bloodImage.color = Color.Lerp(clear, Color.white, alpha);
        }
		else
		{
            alpha = Mathf.Clamp(alpha + fade * Time.deltaTime, 0f, 1f);
            bloodImage.color = Color.Lerp(clear, Color.black, alpha);
		}
    }

    public void AddBlood(float amt)
	{
        alpha = Mathf.Clamp(alpha + amt, 0f, 1f);
	}

}
