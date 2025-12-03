using UnityEngine;
using TMPro;

public class InstructionsTextEdit : MonoBehaviour
{

    public TMP_Text textField1;
    public TMP_Text textField2;


   public void SetText(string textP)
    {
        textField1.text = textP;
    }


}
