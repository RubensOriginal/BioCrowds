using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [HideInInspector]
    public GameObject gameObject;

    [HideInInspector]
    public bool isSelected = false;

    private ManagerScript ms;
    private Vector3 originalVector3;
    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        ms = GetComponent<MouseScript>().ms;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelMoveObject();
            } else
            {
                mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                gameObject.transform.position = new Vector3(
                    Mathf.Clamp(mousePos.x, -50, 50),
                    0.75f,
                    Mathf.Clamp(mousePos.z, -50, 50));
            }
        }
    }

    public void SelectObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        originalVector3 = gameObject.transform.position;
        isSelected = true;
    }

    public void CancelMoveObject()
    {
        isSelected = false;
        gameObject.transform.position = originalVector3;
    }
}
