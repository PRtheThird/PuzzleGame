using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPutColor
{
    public void PutColor(GameManager.COLOR CircleColor);
    public GameManager.COLOR GetColor();

    public void EraseColor();

}
