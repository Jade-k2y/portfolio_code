
using UnityEngine;


namespace Studio.Game
{
    public enum GameScene
    {
        GAME, INSTANCE_UI, TITLE, HOME,
        STORY_EPISODE = 1000,
        STORY_STAGE,
        GEAR_STAGE,
        RAID_STAGE,
        GEM_STAGE,
        GOLD_STAGE,
        PVP_STAGE,
        EVENT_STAGE,
    }

    public enum GameAsset
    {
        None,
        Gem,
        Gold
    }


    
    /*
    public enum PlayContent
    {
        Story,
        GearDungeon,
        RaidDungeon,
        GemDungeon,
        GoldDungeon,
        PvP,
        Event
    }


    public enum PlayStory
    {
        Episode,
        Stage
    }
    */

    public enum WeatherType
    {
        None,
        Sunshine,
        Cloudy,
        Rainy,
    }


    public enum GearMastery
    {
        None,
        Shield,
        Sword,
        Dagger,
        Bow,
        Gun,
        Staff,
        Wand,
        Knuckle,
        Axe,
        Blunt,
        Armor,
        Accessory,
    }


    public enum AttackType
    {
        None,
        Melee1H,
        Melee2H,
        MeleePaired,
        Bow,
        Gun,
        Magic,
    }


    public static class GameConstant
    {
        public static readonly float DefaultPresentDuration = 0.25f;
        public static readonly float LayerPresentDistance = 2000f;
        public static readonly int MaxSlotCount = 4;
        public static readonly int MaxPlayerSlotCount = MaxSlotCount - 1;
        public static readonly Color EmptyTextColor = new Color32(225, 75, 75, 255);
        public static readonly Color DisableTextColor = new Color32(150, 150, 150, 255);

        public static readonly string P_Empty = "EMPTY";
        public static readonly string P_Brown = "BROWN";
        public static readonly string P_Cony = "CONY";
        public static readonly string P_Moon = "MOON";
        public static readonly string P_James = "JAMES";
        public static readonly string P_Sally = "SALLY";
        public static readonly string P_Leonard = "LEONARD";
        public static readonly string P_Jessica = "JESSICA";
        public static readonly string P_Pangyo = "PANG-YO";

        public static readonly int GearCount = 13;
    }
}