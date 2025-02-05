using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PaintButtons : MonoBehaviour
{
    public TextMeshProUGUI Field1, Field2;
    public Image Icon;

    public void UpdateButtons(string T1, string T2, Sprite I)
    {
        Field1.text = T1;
        Field2.text = T2;
        Icon.sprite = I;
    }
}
