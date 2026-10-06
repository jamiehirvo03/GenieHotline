using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public GameObject LinePrefab;

    Line activeLine;

    // Update is called once per frame
    void Update()
    {
        if (InteractionManager.GetInstance().GetHeldItemData() != null)
        {

            if (InteractionManager.GetInstance().GetHeldItemData().CompareTag("Pencil"))
            {
                if (InputManager.GetInstance().GetPrimaryInteractPressed())
                {
                    GameObject newLine = Instantiate(LinePrefab);
                    activeLine = newLine.GetComponent<Line>();
                } 
            }
        }

        if (!InputManager.GetInstance().GetPrimaryInteractPressed())
        {
            activeLine = null;
        }

        if (activeLine != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(InputManager.GetInstance().GetMousePos());
            activeLine.UpdateLine(mousePos);
        }
    }
}
