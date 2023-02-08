using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArObjectController : MonoBehaviour
{
    public Transform objGroup;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        bool GroupCheck = false;
        while (!GroupCheck)
        {
            yield return null;

            objGroup = GameInfo.ins.GetArObjectGroup().transform;

            if (objGroup != null)
            {
                transform.parent = objGroup;
                GroupCheck = true;
            }

            yield return null;

            GameInfo.ins.LastObject = this.gameObject;
        }

        GameInfo.ins.ArObjList.Add(this.gameObject);
    }

}
