using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Méthodes d'extension pour faciliter la gestion des composants UI.
/// </summary>
public static class UIHelper 
{
    private static Dictionary<Slider, Coroutine> activeCoroutines = new Dictionary<Slider, Coroutine>();
    
    /// <summary>
    /// Met à jour la valeur d'un slider avec une transition animée.
    /// </summary>
    /// <param name="slider">Le slider à animer</param>
    /// <param name="targetValue">La valeur cible</param>
    /// <param name="duration">La durée de la transition en secondes</param>
    public static void UpdateSlider(this MonoBehaviour caller, Slider slider, float targetValue, float duration = 0.3f)
    {
        if (activeCoroutines.ContainsKey(slider) && activeCoroutines[slider] != null)
        {
            caller.StopCoroutine(activeCoroutines[slider]);
        }

        Coroutine newCoroutine = caller.StartCoroutine(SmoothTransitionSlider(slider, targetValue, duration));
        activeCoroutines[slider] = newCoroutine;
    }

    /// <summary>
    /// Met à jour la valeur d'un slider avec une transition animée et retourne la coroutine.
    /// </summary>
    /// <param name="slider">Le slider à animer</param>
    /// <param name="targetValue">La valeur cible</param>
    /// <param name="duration">La durée de la transition en secondes</param>
    /// <returns>La coroutine créée</returns>
    public static Coroutine UpdateSliderCoroutine(this MonoBehaviour caller, Slider slider, float targetValue, float duration = 0.3f)
    {
        if (activeCoroutines.ContainsKey(slider) && activeCoroutines[slider] != null)
        {
            caller.StopCoroutine(activeCoroutines[slider]);
        }

        Coroutine newCoroutine = caller.StartCoroutine(SmoothTransitionSlider(slider, targetValue, duration));
        activeCoroutines[slider] = newCoroutine;
    
        return newCoroutine; 
    }
        
    private static IEnumerator SmoothTransitionSlider(Slider slider, float targetValue, float duration)
    {
        float elapsed = 0f;
        float startValue = slider.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            slider.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        slider.value = targetValue;
        if (activeCoroutines.ContainsKey(slider))
        {
            activeCoroutines.Remove(slider);
        }
    }
}