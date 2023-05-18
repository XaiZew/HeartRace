using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpritePlayer : MonoBehaviour
{
    public Sprite[] sprites;
    Image currentSprite;
    int currentSpriteInt = 0;

    float timer = 0;
    public float duration;

    [SerializeField] bool repeat = true;
    public bool reverse = false;

    public bool playing;

    private void Start() {
        currentSpriteInt = 1;
        currentSprite = GetComponent<Image>();

        StartCoroutine(PlayAnim());
    }

    public IEnumerator PlayAnim() {
        playing = true;
        timer = 0;
        if (reverse)
            currentSpriteInt = sprites.Length - 1;
        else
            currentSpriteInt = 0;

        do {
            if (timer > duration) {
                timer = 0;

                // if ((currentSpriteInt >= sprites.Length && repeat && !reverse)) {
                //     StartCoroutine(PlayAnim());
                //     break;
                // }

                currentSprite.sprite = sprites[currentSpriteInt];

                // if (currentSpriteInt <= 0 && reverse && repeat) {
                //     StartCoroutine(PlayAnim());
                //     break;
                // }

                if (reverse && currentSpriteInt >= 0)
                    currentSpriteInt--;
                else if (currentSpriteInt < sprites.Length && !reverse)
                    currentSpriteInt++;
    
                yield return null;
            }
            timer += 1 * Time.deltaTime;
            yield return null;
        } while (reverse ? currentSpriteInt >= 0 : currentSpriteInt < sprites.Length);

        playing = false;

        if (repeat)
            StartCoroutine(PlayAnim());
    }
}
