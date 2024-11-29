using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCloneController : MonoBehaviour
{
    public GameObject boss;
    public GameObject clone1;
    public GameObject clone2;

    private float y = 2.5f;
    private float xOffset = 3.55f;

    private bool active = false;
    private bool inPosition = false;

    void Start()
    {
        EventManager.StartListening("BossPhase2Start", this, OnActivate);
        EventManager.StartListening("BossPhaseEnd", this, OnDeactivate);
    }

    void Update()
    {
        if (!active) return;
    }

    void OnActivate()
    {
        active = true;
        clone1.SetActive(true);
        clone2.SetActive(true);
        Shuffle();
    }

    private void Shuffle()
    {
        inPosition = false;
        boss.transform.position = new Vector3(0, y, boss.transform.position.z);
        clone1.transform.position = new Vector3(0, y, clone1.transform.position.z);
        clone2.transform.position = new Vector3(0, y, clone2.transform.position.z);

        int realBossIndex = Random.Range(0, 3);
        var x = 0 - xOffset;
        int cloneIndex = 1;
        for (int i = 0; i < 3; i++)
        {
            var clone = clone2;
            if (i == realBossIndex)
            {
                clone = boss;
            }
            else if (cloneIndex == 1)
            {
                clone = clone1;
                cloneIndex = 2;
            }
            StartCoroutine(MoveToPosition(clone, new Vector3(x, y, clone.transform.position.z), 1f));
            x += xOffset;
        }
    }

    private IEnumerator MoveToPosition(GameObject obj, Vector3 target, float duration)
    {
        float time = 0;
        Vector3 startPosition = obj.transform.position;
        while (time < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = target;
        if (!inPosition)
        {
            EventManager.Trigger("BossCloneInPosition");
            inPosition = true;
        }
    }

    void OnDeactivate()
    {
        active = false;
        clone1.SetActive(false);
        clone2.SetActive(false);
    }

}
