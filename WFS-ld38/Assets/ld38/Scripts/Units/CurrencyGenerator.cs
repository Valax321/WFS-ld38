using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyGenerator : UnitController
{
    protected override void ChildUnitInit()
    {
        base.ChildUnitInit();
        player.AddCurrencyPerTurn(currentTile.currency);
    }

    public override void Killed()
    {
        base.Killed();
        player.RemoveCurrencyPerTurn(currentTile.currency);
    }

    public override void StartOfTurn()
    {
        base.StartOfTurn();
        player.AddCurrency(currentTile.currency);
    }
}
