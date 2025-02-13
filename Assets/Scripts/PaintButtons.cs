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

    public void UpdateButtons(Upgrade_Data UD)
    {
        Field1.text = UD.Upgrade_Name;
        Field2.text = UD.Upgrade_Description;
        Icon.sprite = UD.Upgrade_Icon;
    }
}
