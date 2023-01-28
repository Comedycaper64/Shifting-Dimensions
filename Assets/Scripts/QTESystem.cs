using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTESystem : MonoBehaviour
{
    private bool Start2DQTE = false;
    private bool Start3DQTE = false;
    private bool QTEPassed = false;

    [SerializeField] private GameObject Instructions2D;
    [SerializeField] private GameObject countDown12D;
    [SerializeField] private GameObject countDown22D;
    [SerializeField] private GameObject countDown32D;
    [SerializeField] private GameObject countDown42D;
    [SerializeField] private GameObject countDown52D;
    
    [SerializeField] private GameObject Instructions3D;
    [SerializeField] private GameObject countDown13D;
    [SerializeField] private GameObject countDown23D;
    [SerializeField] private GameObject countDown33D;
    [SerializeField] private GameObject countDown43D;
    [SerializeField] private GameObject countDown53D;

    [SerializeField] private AudioClip QTECorrectInput;
    [SerializeField] private AudioClip QTEIncorrectInput;

    Coroutine countDownCoroutine;

    private int arrayIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Start2DQTE)
        {
            countDownCoroutine = StartCoroutine(Countdown());
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) && (arrayIndex == 0 || arrayIndex == 2))
                {
                    AudioSource.PlayClipAtPoint(QTECorrectInput, Camera.main.transform.position);
                    arrayIndex++;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) && (arrayIndex == 1 || arrayIndex == 3))
                {
                    AudioSource.PlayClipAtPoint(QTECorrectInput, Camera.main.transform.position);
                    arrayIndex++;
                }
                else
                {
                    AudioSource.PlayClipAtPoint(QTEIncorrectInput, Camera.main.transform.position);
                }

                if (arrayIndex == 4)
                {
                    QTEPassed = true;
                }
            }
        }
        else if (Start3DQTE)
        {
            countDownCoroutine = StartCoroutine(Countdown());
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) && (arrayIndex == 0 || arrayIndex == 1))
                {
                    AudioSource.PlayClipAtPoint(QTECorrectInput, Camera.main.transform.position);
                    arrayIndex++;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && (arrayIndex == 2 || arrayIndex == 3))
                {
                    AudioSource.PlayClipAtPoint(QTECorrectInput, Camera.main.transform.position);
                    arrayIndex++;
                }
                else
                {
                    AudioSource.PlayClipAtPoint(QTEIncorrectInput, Camera.main.transform.position);
                }

                if (arrayIndex == 4)
                {
                    QTEPassed = true;
                }
            }
        }
    }

    private IEnumerator Countdown()
    {
        if (Start2DQTE)
        {
            yield return new WaitForSeconds(1f);
            countDown12D.SetActive(false);
            yield return new WaitForSeconds(1f);
            countDown22D.SetActive(false);
            yield return new WaitForSeconds(1f);
            countDown32D.SetActive(false);
            yield return new WaitForSeconds(1f);
            countDown42D.SetActive(false);
            yield return new WaitForSeconds(0.9f);
            countDown52D.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            countDown13D.SetActive(false);
            yield return new WaitForSeconds(1f);
            countDown23D.SetActive(false);
            yield return new WaitForSeconds(1f);
            countDown33D.SetActive(false);
            yield return new WaitForSeconds(1f);
            countDown43D.SetActive(false);
            yield return new WaitForSeconds(0.9f);
            countDown53D.SetActive(false);
        }
    }

    public void StartInto2DQTE()
    {
        Start2DQTE = true;
        Instructions2D.SetActive(true);
    }
    public void StartInto3DQTE()
    {
        Start3DQTE = true;
        Instructions3D.SetActive(true);
    }

    public bool CheckQTE()
    {
        return QTEPassed;
    }

    public void ResetQTE()
    {
        QTEPassed = false;
        Start3DQTE = false;
        Start2DQTE = false;
        StopCoroutine(countDownCoroutine);
        countDown12D.SetActive(true);
        countDown22D.SetActive(true);
        countDown32D.SetActive(true); 
        countDown42D.SetActive(true);             
        countDown52D.SetActive(true);       
        countDown13D.SetActive(true);      
        countDown23D.SetActive(true);        
        countDown33D.SetActive(true);        
        countDown43D.SetActive(true);
        countDown53D.SetActive(true);

        Instructions2D.SetActive(false);
       
        Instructions3D.SetActive(false);
        
        arrayIndex = 0;
    }
}
