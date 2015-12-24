using UnityEngine;
using System.Collections;

public class ShipData {

    // Default ship values.
    public const float SHIP_DEFAULT_SPEED = 28.0f;
    public const float SHIP_DEFAULT_TURNSPEED = 3.0f;
    public const float SHIP_DEFAULT_FIRERATE = 0.15f;
    public const float SHIP_DEFAULT_RESPAWNRATE = 3.0f;
    public const float SHIP_DEFAULT_WARPCOOLDOWN = 2.0f;
    public const bool SHIP_DEFAULT_CANREVERSE = false;
    public const bool SHIP_DEFAULT_CANCONTROLWARP = true;

    public const float SHIP_DEFAULT_LINEARDRAG = 0.5f;
    public const float SHIP_DEFAULT_MASS = 5.0f;

    public const float SHIP_DEFAULT_BULLETPOWERUPTIME = 5.0f;
    public const float SHIP_DEFAULT_SHIPCONTROLPOWERUPTIME = 15.0f;
    public const float SHIP_DEFAULT_DOUBLESHOTPOWERUPTIME = 8.0f;


    // A speedy ship type.
    public const float SHIP_SPEEDY_SPEED = 41.0f;
    public const float SHIP_SPEEDY_TURNSPEED = 4.0f;
    public const float SHIP_SPEEDY_FIRERATE = 0.2f;
    public const float SHIP_SPEEDY_RESPAWNRATE = 3.0f;
    public const float SHIP_SPEEDY_WARPCOOLDOWN = 1.0f;
    public const bool SHIP_SPEEDY_CANREVERSE = true;
    public const bool SHIP_SPEEDY_CANCONTROLWARP = false;

    public const float SHIP_SPEEDY_LINEARDRAG = 0.4f;
    public const float SHIP_SPEEDY_MASS = 3.0f;

    public const float SHIP_SPEEDY_BULLETPOWERUPTIME = 5.0f;
    public const float SHIP_SPEEDY_SHIPCONTROLPOWERUPTIME = 15.0f;
    public const float SHIP_SPEEDY_DOUBLESHOTPOWERUPTIME = 8.0f;

    // A "guns blazin'" ship type.
    public const float SHIP_BLAZE_SPEED = 19.0f;
    public const float SHIP_BLAZE_TURNSPEED = 2.5f;
    public const float SHIP_BLAZE_FIRERATE = 0.1f;
    public const float SHIP_BLAZE_RESPAWNRATE = 3.0f;
    public const float SHIP_BLAZE_WARPCOOLDOWN = 2.0f;
    public const bool SHIP_BLAZE_CANREVERSE = false;
    public const bool SHIP_BLAZE_CANCONTROLWARP = false;

    public const float SHIP_BLAZE_LINEARDRAG = 0.65f;
    public const float SHIP_BLAZE_MASS = 7.0f;

    public const float SHIP_BLAZE_BULLETPOWERUPTIME = 9.0f;
    public const float SHIP_BLAZE_SHIPCONTROLPOWERUPTIME = 15.0f;
    public const float SHIP_BLAZE_DOUBLESHOTPOWERUPTIME = 13.0f;
}
