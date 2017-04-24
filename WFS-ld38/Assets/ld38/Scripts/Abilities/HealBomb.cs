using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class HealBomb : AbilityBehaviour
{
    const int BOMB_RANGE = 2;
    const int HEAL_TURNS = 2;
    List<UnitController> users = new List<UnitController>();
    int healedTurns;
    bool shouldHeal;

    public override void Activate(VoronoiTile tile)
    {
        base.Activate(tile);

        if (shouldHeal)
        {
            //Already healing, complain.
            UIController.instance.PushNotification("Unit is already healing.");
            return;
        }

        users.Clear();
        var tiles = VoronoiTile.FindTilesInRange(tile, BOMB_RANGE);
        foreach (var tl in tiles)
        {
            foreach (var t in tl)
            {
                if (t.occupyingUnit != null)
                {
                    if (!users.Contains(t.occupyingUnit))
                    {
                        users.Add(t.occupyingUnit);
                    }
                }
            }
        }

        HealSelected();
        shouldHeal = true;
    }

    public override void OnTurn()
    {
        base.OnTurn();
        if (shouldHeal)
        {
            healedTurns++;
            if (healedTurns <= HEAL_TURNS)
            {
                HealSelected();
            }
            else
            {
                shouldHeal = false;
            }
        }
    }

    void HealSelected()
    {
        List<int> markForRemove = new List<int>();
        int i = 0;
        foreach (var unit in users)
        {
            if (unit != null)
            {
                unit.Damage(-Random.Range(ourAbility.damageMin, ourAbility.damageMax));
            }
            else
            {
                markForRemove.Add(i);
            }
            i++;
        }
        foreach (var idx in markForRemove)
        {
            users.RemoveAt(idx);
        }
    }
}
