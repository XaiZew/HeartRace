using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashText : MonoBehaviour
{
    private Transform m_Target;
    [SerializeField] private float m_Speed;
    [SerializeField] float splashUpAmount;

    public float splashTextLength;

    Vector3 startPos;

    public Sprite[] splashTextSprites;
    public Image splashTextSprite;

    private void Start() {
        m_Target = GameObject.Find("Player").transform;
        Destroy(this.gameObject, splashTextLength);
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 lTargetDir = m_Target.position - startPos;
        lTargetDir.y = 0.0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * (m_Speed / 100));

        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, startPos.y + 5, transform.position.z), 1 * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(5, 5, 5), 1 * Time.deltaTime);
    }
}