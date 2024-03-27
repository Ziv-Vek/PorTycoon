using UnityEngine.UI;

public class SwButton : Button
{
    //This property sets the gameObject name and the text of the child text component
    public string Text
    {
        get => transform.GetChild(0).GetComponent<Text>().text;
        set
        {
            transform.GetChild(0).GetComponent<Text>().text = value;
            gameObject.name = value;
        }
    }
}