using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public List<DuoButton> Buttons;
    public int CorrectCount;
    public bool SolutionFound;

    public GameObject TargetObject;

    private void Awake()
    {
        
    }

    //public void Solve()
    //{
    //    SolutionFound = true;

    //    ButtonInterface target;

    //    if (TargetObject.TryGetComponent<ButtonInterface>(out target))
    //    {
    //        target.Activate();
    //        Debug.Log("Activate" + target);
    //    }
    //    else
    //    {
    //        TargetObject.GetComponentInChildren<ButtonInterface>().Activate();
    //        Debug.Log("Activate" + TargetObject.name);
    //    }
    //}

    public void CheckSolution()
    {

        if (SolutionFound)
        {
            return;
        }
        
        IEnumerable<DuoButton> query = Buttons.Where(x => x.Activated);
        CorrectCount = query.Count();

        if (CorrectCount >= Buttons.Count)
        {
            SolutionFound = true;

            ButtonInterface target;

            if (TargetObject.TryGetComponent<ButtonInterface>(out target))
            {
                target.Activate();
                Debug.Log("Activate" + target);
            }
            else
            {
                TargetObject.GetComponentInChildren<ButtonInterface>().Activate();
                Debug.Log("Activate" + TargetObject.name);
            }

            //TargetObject.SetActive(false);

            foreach (var button in Buttons)
            {
                button.CountingDown = false;
                button.Activated = true;
                button.Interactable = false;
            }
        }
    }
}
