using System;
using System.Collections.Generic;
using ArmorClassIcon.Patches;
using BepInEx.Configuration;

namespace ArmorClassIcon;

public static class Settings
{
    public static ConfigEntry<bool> ShowInInventory;
    public static ConfigEntry<bool> ShowAtTraders;
    public static ConfigEntry<bool> ShowOnFlea;

    public static ConfigEntry<bool> EnableBodyArmor;
    public static ConfigEntry<bool> EnableHeadwear;
    public static ConfigEntry<bool> EnableArmoredRigs;
    public static ConfigEntry<bool> EnableVisors;
    public static ConfigEntry<bool> EnableFaceCovers;
    public static ConfigEntry<bool> EnableAccessories;

    public static ConfigEntry<string> FleaDisplayMode;
    public static ConfigEntry<string> DisplayMode;

    public static readonly Dictionary<string, ConfigEntry<bool>> SlotToggles =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly (string Id, string NameKey)[] PlateSlots =
    {
        ("front_plate", "Slot_front_plate"),
        ("back_plate", "Slot_back_plate"),
        ("left_side_plate", "Slot_left_side_plate"),
        ("right_side_plate", "Slot_right_side_plate"),
        ("soft_armor_front", "Slot_soft_armor_front"),
        ("soft_armor_back", "Slot_soft_armor_back"),
        ("soft_armor_left", "Slot_soft_armor_left"),
        ("soft_armor_right", "Slot_soft_armor_right"),
        ("collar", "Slot_collar"),
        ("groin", "Slot_groin"),
        ("groin_back", "Slot_groin_back"),
        ("shoulder_l", "Slot_shoulder_l"),
        ("shoulder_r", "Slot_shoulder_r"),
        ("helmet_top", "Slot_helmet_top"),
        ("helmet_back", "Slot_helmet_back"),
        ("helmet_eyes", "Slot_helmet_eyes"),
        ("helmet_jaw", "Slot_helmet_jaw"),
        ("helmet_ears", "Slot_helmet_ears")
    };

    public static IconDisplayMode InvMode => ParseMode(DisplayMode.Value);
    public static IconDisplayMode FleaMode => ParseMode(FleaDisplayMode.Value);

    public static void Init(ConfigFile config)
    {
        Localization.DetectLanguage();

        string secDisplay = Localization.T("Section_Display");
        string secItemTypes = Localization.T("Section_ItemTypes");
        string secMode = Localization.T("Section_Mode");
        string secCustomPreset = Localization.T("Section_CustomPreset");

        ShowInInventory = config.Bind(secDisplay, "Show in inventory", true,
            new ConfigDescription(Localization.T("Desc_ShowInInventory"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_ShowInInventory") }));

        ShowAtTraders = config.Bind(secDisplay, "Show at traders", true,
            new ConfigDescription(Localization.T("Desc_ShowAtTraders"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_ShowAtTraders") }));

        ShowOnFlea = config.Bind(secDisplay, "Show on flea market", true,
            new ConfigDescription(Localization.T("Desc_ShowOnFlea"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_ShowOnFlea") }));

        EnableBodyArmor = config.Bind(secItemTypes, "Enable on body armor", true,
            new ConfigDescription(Localization.T("Desc_EnableBodyArmor"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_EnableBodyArmor") }));

        EnableHeadwear = config.Bind(secItemTypes, "Enable on headwear", true,
            new ConfigDescription(Localization.T("Desc_EnableHeadwear"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_EnableHeadwear") }));

        EnableArmoredRigs = config.Bind(secItemTypes, "Enable on armored rigs", true,
            new ConfigDescription(Localization.T("Desc_EnableArmoredRigs"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_EnableArmoredRigs") }));

        EnableVisors = config.Bind(secItemTypes, "Enable on visors", true,
            new ConfigDescription(Localization.T("Desc_EnableVisors"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_EnableVisors") }));

        EnableFaceCovers = config.Bind(secItemTypes, "Enable on face covers", true,
            new ConfigDescription(Localization.T("Desc_EnableFaceCovers"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_EnableFaceCovers") }));

        EnableAccessories = config.Bind(secItemTypes, "Enable on accessories", true,
            new ConfigDescription(Localization.T("Desc_EnableAccessories"), null,
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_EnableAccessories") }));

        FleaDisplayMode = config.Bind(secMode, "Flea market display mode", Localization.T("Val_Highest"),
            new ConfigDescription(
                Localization.T("Desc_FleaDisplayMode"),
                new AcceptableValueList<string>(Localization.T("Val_Highest"), Localization.T("Val_MinMax")),
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_FleaDisplayMode") }));

        DisplayMode = config.Bind(secMode, "Inventory & traders display mode", Localization.T("Val_Highest"),
            new ConfigDescription(
                Localization.T("Desc_DisplayMode"),
                new AcceptableValueList<string>(
                    Localization.T("Val_Highest"),
                    Localization.T("Val_MinMax"),
                    Localization.T("Val_AllPlates"),
                    Localization.T("Val_Custom")),
                new ConfigurationManagerAttributes { DispName = Localization.T("Name_DisplayMode") }));

        foreach (var (id, nameKey) in PlateSlots)
            SlotToggles[id] = config.Bind(secCustomPreset, nameKey, true,
                new ConfigDescription(Localization.T("Desc_SlotToggle"), null,
                    new ConfigurationManagerAttributes { DispName = Localization.T(nameKey) }));

        ShowInInventory.SettingChanged += OnSettingChanged;
        ShowAtTraders.SettingChanged += OnSettingChanged;
        ShowOnFlea.SettingChanged += OnSettingChanged;
        EnableBodyArmor.SettingChanged += OnSettingChanged;
        EnableHeadwear.SettingChanged += OnSettingChanged;
        EnableArmoredRigs.SettingChanged += OnSettingChanged;
        EnableVisors.SettingChanged += OnSettingChanged;
        EnableFaceCovers.SettingChanged += OnSettingChanged;
        EnableAccessories.SettingChanged += OnSettingChanged;
        FleaDisplayMode.SettingChanged += OnSettingChanged;
        DisplayMode.SettingChanged += OnSettingChanged;

        foreach (var toggle in SlotToggles.Values) toggle.SettingChanged += OnSettingChanged;
    }

    public static bool IsSlotShown(string slotId)
    {
        return !string.IsNullOrEmpty(slotId)
               && SlotToggles.TryGetValue(slotId, out ConfigEntry<bool> toggle)
               && toggle.Value;
    }

    private static void OnSettingChanged(object sender, EventArgs args)
    {
        ItemViewStatsPatch.RequestRefresh();
    }

    private static IconDisplayMode ParseMode(string value)
    {
        return value?.Trim() switch
        {
            "Lowest & Highest" or "Низший и высший" => IconDisplayMode.MinMax,
            "All Plates" or "Все плиты" => IconDisplayMode.AllPlates,
            "Custom" or "Пользовательский" => IconDisplayMode.Custom,
            _ => IconDisplayMode.MaxClass
        };
    }
}

public enum IconDisplayMode
{
    MaxClass,
    MinMax,
    AllPlates,
    Custom
}
public class ConfigurationManagerAttributes
{
    public string DispName;
    public int? Order;
}