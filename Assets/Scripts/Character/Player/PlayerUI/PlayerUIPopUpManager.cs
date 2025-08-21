using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Moko
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("YOU DIED Pop Up")] 
        [SerializeField] private GameObject youDiedPopUpGameObject;
        [SerializeField] private TextMeshProUGUI youDiedPopUpBackGroundText;
        [SerializeField] private TextMeshProUGUI youDiedPopUpText;
        [SerializeField] private CanvasGroup youDiedPopUpCanvasGroup; // to set the alpha to fade over time

        public void SendYouDiedPopUp()
        {
            // activate post processing effects
            
            
            youDiedPopUpGameObject.SetActive(true);
            youDiedPopUpBackGroundText.characterSpacing = 0;
            
            // stretch out the pop up
            StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackGroundText, 8f, 20f));

            // fade in the pop up
            StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5f));

            // wait fade out the pop up
            StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));
        }

        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            if (duration > 0f)
            {
                text.characterSpacing = 0; // reset character spacing
                float timer = 0;
                
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
        {
            if (duration > 0f)
            {
                canvas.alpha = 0f;
                float timer = 0;
                
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1f, duration * Time.deltaTime);
                    yield return null;
                }
            }
            
            canvas.alpha = 1f;

            yield return null;
        }

        private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
        {
            if (duration > 0f)
            {
                while (delay > 0)
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }
                
                canvas.alpha = 1f;
                float timer = 0;
                
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0f, duration * Time.deltaTime);
                    yield return null;
                }
            }
            
            canvas.alpha = 0f;

            yield return null;
        }
    }
}
