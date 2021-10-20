using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour
{
    private int m_stage = 0;
    private Brain m_currentBrain;
    private Attributes CurrentAttributes => m_currentBrain.Attributes;

    private void Log(string s)
    {
        LevelManager.Instance.TextLog.AddLine(s);
    }

    public void OnUsed(Brain brain, Interactable interactable)
    {
        m_currentBrain = brain;

        if (m_stage == 0)
        {
            if (CurrentAttributes.CookingSkill < 50)
            {
                Log($"{brain.DisplayName} tries to turn on the stove, but can't");
            }
            else
            {
                Log($"{brain.DisplayName} turns on the stove");
                m_stage++;
            }

            interactable.Finish();
        }
        else if (m_stage == 1)
        {
            if (CurrentAttributes.CookingSkill < 50)
            {
                Log($"{brain.DisplayName} tries to cook some lobster. It skuttles out of the pot.");
            }
            else
            {
                Log($"{brain.DisplayName} cooks some lobster");
                m_stage++;
            }
            
            interactable.Finish();
        }
        else if (m_stage == 2)
        {
            Log($"{brain.DisplayName}: 'Yummy'");
            interactable.Finish();
            m_stage++;
        }
        else if (m_stage == 3)
        {
            Log($"{brain.DisplayName}: 'Itchy. Tasty.'");
            interactable.Finish();
            m_stage++;
        }
        else
        {
            Log($"{brain.DisplayName} decides they've had enough of the stove...");
            StartCoroutine(FinishMeCoroutine(interactable));
            m_stage++;
        }
    }

    public void Destroy(Brain brain, Interactable interactable)
    {
        m_currentBrain = brain;

        Log($"{m_currentBrain.DisplayName} destroys the stove in disgust. Good riddance.");
        interactable.Finish();
        Destroy(gameObject);
    }

    private IEnumerator FinishMeCoroutine(Interactable interactable)
    {
        interactable.CanCancel = false;

        while (transform.localScale.x > 0.05f)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.02f * Time.deltaTime, transform.localScale.y - 0.02f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Log("Goodbye, Stove......");

        interactable.Finish();
        Destroy(gameObject);
    }
}
