using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats {
    private static string mPlayerName;
    private static bool mUseQwerty;
    private static float mVolume = 1;

    public static string PlayerName
    {
        get { return mPlayerName; }
        set { mPlayerName = value; }
    }

    public static bool UseQwerty
    {
        get { return mUseQwerty; }
        set { mUseQwerty = value; }
    }

    public static float Volume
    {
        get { return mVolume; }
        set { mVolume = value; }
    }

}
