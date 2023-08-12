using UnityEngine;

public class Coins : Collectables
{
    public override void AddToCollection(){
        //override this
        GameManager.coins++;
    }
}