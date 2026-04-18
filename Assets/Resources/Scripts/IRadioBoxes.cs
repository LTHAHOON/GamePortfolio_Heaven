using System.Collections.Generic;
using UnityEngine.UI;

public interface IRadioBoxes
{

   public void GetRadioBoxList(IRadioBoxes radioBoxClass)
   {
        radioBoxClass.GetRadioBoxList();
   }

    public List<Toggle> GetRadioBoxList();
    public void SetRadioBoxList();
}
