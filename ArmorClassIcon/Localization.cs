using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArmorClassIcon
{
    public static class Localization
    {
        private static string _currentLang = "En";

        private static readonly Dictionary<string, Dictionary<string, string>> _table =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["En"] = new Dictionary<string, string>
                {
                    ["Section_Display"] = "1. Display",
                    ["Section_ItemTypes"] = "2. Item Types",
                    ["Section_Mode"] = "3. Display Mode",
                    ["Section_CustomPreset"] = "4. Custom preset",

                    ["Name_ShowInInventory"] = "Show in inventory",
                    ["Desc_ShowInInventory"] = "Show armor class icons in inventory screens.",
                    ["Name_ShowAtTraders"] = "Show at traders",
                    ["Desc_ShowAtTraders"] = "Show armor class icons in trader screens.",
                    ["Name_ShowOnFlea"] = "Show on flea market",
                    ["Desc_ShowOnFlea"] = "Show armor class icons on flea market offers.",

                    ["Name_EnableBodyArmor"] = "Enable on body armor",
                    ["Desc_EnableBodyArmor"] = "Show armor class icon on body armor.",
                    ["Name_EnableHeadwear"] = "Enable on headwear",
                    ["Desc_EnableHeadwear"] = "Show armor class icon on armored helmets.",
                    ["Name_EnableArmoredRigs"] = "Enable on armored rigs",
                    ["Desc_EnableArmoredRigs"] = "Show armor class icon on rigs with built-in armor.",
                    ["Name_EnableVisors"] = "Enable on visors",
                    ["Desc_EnableVisors"] = "Show armor class icon on armored visors.",
                    ["Name_EnableFaceCovers"] = "Enable on face covers",
                    ["Desc_EnableFaceCovers"] = "Show armor class icon on armored face covers.",
                    ["Name_EnableAccessories"] = "Enable on accessories",
                    ["Desc_EnableAccessories"] = "Show armor class icon on equipment components with armor (ArmoredEquipment items not covered by other types).",

                    ["Name_FleaDisplayMode"] = "Flea market display mode",
                    ["Desc_FleaDisplayMode"] = "Armor class display type for the flea market.",
                    ["Name_DisplayMode"] = "Inventory & traders display mode",
                    ["Desc_DisplayMode"] = "Armor class display type for the inventory and trader screens.",
                    
                    ["Val_Highest"] = "Highest Class",
                    ["Val_MinMax"] = "Lowest & Highest",
                    ["Val_AllPlates"] = "All Plates",
                    ["Val_Custom"] = "Custom",

                    ["Desc_SlotToggle"] = "Show this plate slot in Custom display mode. Uncheck to hide the slot.",
                    ["Slot_front_plate"] = "Front plate",
                    ["Slot_back_plate"] = "Back plate",
                    ["Slot_left_side_plate"] = "Left side plate",
                    ["Slot_right_side_plate"] = "Right side plate",
                    ["Slot_soft_armor_front"] = "Soft armor front",
                    ["Slot_soft_armor_back"] = "Soft armor back",
                    ["Slot_soft_armor_left"] = "Soft armor left",
                    ["Slot_soft_armor_right"] = "Soft armor right",
                    ["Slot_collar"] = "Collar",
                    ["Slot_groin"] = "Groin",
                    ["Slot_groin_back"] = "Groin back",
                    ["Slot_shoulder_l"] = "Left shoulder",
                    ["Slot_shoulder_r"] = "Right shoulder",
                    ["Slot_helmet_top"] = "Helmet top",
                    ["Slot_helmet_back"] = "Helmet back",
                    ["Slot_helmet_eyes"] = "Helmet eyes",
                    ["Slot_helmet_jaw"] = "Helmet jaw",
                    ["Slot_helmet_ears"] = "Helmet ears"
                },
                ["Ru"] = new Dictionary<string, string>
                {
                    ["Section_Display"] = "1. Отображение",
                    ["Section_ItemTypes"] = "2. Типы предметов",
                    ["Section_Mode"] = "3. Режим отображения",
                    ["Section_CustomPreset"] = "4. Пользовательский пресет",

                    ["Name_ShowInInventory"] = "Показывать в инвентаре",
                    ["Desc_ShowInInventory"] = "Показывать иконки классов брони в меню инвентаря.",
                    ["Name_ShowAtTraders"] = "Показывать у торговцев",
                    ["Desc_ShowAtTraders"] = "Показывать иконки классов брони в меню торговцев.",
                    ["Name_ShowOnFlea"] = "Показывать на барахолке",
                    ["Desc_ShowOnFlea"] = "Показывать иконки классов брони на предложениях барахолки.",

                    ["Name_EnableBodyArmor"] = "Включить на бронежилетах",
                    ["Desc_EnableBodyArmor"] = "Показывать иконку класса брони на бронежилетах.",
                    ["Name_EnableHeadwear"] = "Включить на шлемах",
                    ["Desc_EnableHeadwear"] = "Показывать иконку класса брони на бронированных шлемах.",
                    ["Name_EnableArmoredRigs"] = "Включить на разгрузках",
                    ["Desc_EnableArmoredRigs"] = "Показывать иконку класса брони на разгрузках со встроенной броней.",
                    ["Name_EnableVisors"] = "Включить на забралах",
                    ["Desc_EnableVisors"] = "Показывать иконку класса брони на бронированных забралах.",
                    ["Name_EnableFaceCovers"] = "Включить на масках",
                    ["Desc_EnableFaceCovers"] = "Показывать иконку класса брони на бронированных масках.",
                    ["Name_EnableAccessories"] = "Включить на аксессуарах",
                    ["Desc_EnableAccessories"] = "Показывать иконку класса брони на компонентах экипировки с броней (не покрытых другими типами).",

                    ["Name_FleaDisplayMode"] = "Режим отображения на барахолке",
                    ["Desc_FleaDisplayMode"] = "Тип отображения класса брони для барахолки.",
                    ["Name_DisplayMode"] = "Режим отображения в инвентаре и у торговцев",
                    ["Desc_DisplayMode"] = "Тип отображения класса брони для инвентаря и торговцев.",
                    
                    ["Val_Highest"] = "Высший класс",
                    ["Val_MinMax"] = "Низший и высший",
                    ["Val_AllPlates"] = "Все плиты",
                    ["Val_Custom"] = "Пользовательский",

                    ["Desc_SlotToggle"] = "Показывать этот слот плиты в Пользовательском режиме. Снимите галочку, чтобы скрыть.",
                    ["Slot_front_plate"] = "Передняя плита",
                    ["Slot_back_plate"] = "Задняя плита",
                    ["Slot_left_side_plate"] = "Левая боковая плита",
                    ["Slot_right_side_plate"] = "Правая боковая плита",
                    ["Slot_soft_armor_front"] = "Мягкая броня (спереди)",
                    ["Slot_soft_armor_back"] = "Мягкая броня (сзади)",
                    ["Slot_soft_armor_left"] = "Мягкая броня (слева)",
                    ["Slot_soft_armor_right"] = "Мягкая броня (справа)",
                    ["Slot_collar"] = "Воротник",
                    ["Slot_groin"] = "Пах",
                    ["Slot_groin_back"] = "Пах (сзади)",
                    ["Slot_shoulder_l"] = "Левое плечо",
                    ["Slot_shoulder_r"] = "Правое плечо",
                    ["Slot_helmet_top"] = "Верх шлема",
                    ["Slot_helmet_back"] = "Задняя часть шлема",
                    ["Slot_helmet_eyes"] = "Забрало (глаза)",
                    ["Slot_helmet_jaw"] = "Забрало (челюсть)",
                    ["Slot_helmet_ears"] = "Уши шлема"
                }
            };

        public static string T(string key)
        {
            if (_table.TryGetValue(_currentLang, out var lang) && lang.TryGetValue(key, out var value))
                return value;

            if (_table["En"].TryGetValue(key, out var fallback))
                return fallback;

            return key;
        }

        public static void SetLanguage(string lang)
        {
            var normalized = NormalizeLanguage(lang);
            if (!string.IsNullOrEmpty(normalized))
            {
                _currentLang = normalized;
            }
        }

        public static string CurrentLanguage => _currentLang;

        public static void DetectLanguage()
        {
            var detected = TryGetSptLanguage();
            if (string.IsNullOrEmpty(detected))
                detected = TryGetUnityLanguage();

            var normalized = NormalizeLanguage(detected);

            if (!string.IsNullOrEmpty(normalized))
            {
                _currentLang = normalized;
                return;
            }

            _currentLang = "En";
        }

        private static string TryGetSptLanguage()
        {
            try
            {
                Type helperType = null;
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var t = asm.GetType("EFT.LocalizedHelper", false)
                         ?? asm.GetType("LocalizedHelper", false);
                    if (t != null) { helperType = t; break; }
                }

                if (helperType == null) return null;

                const System.Reflection.BindingFlags flags =
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static;

                var prop = helperType.GetProperty("CurrentLanguage", flags)
                        ?? helperType.GetProperty("Language", flags);
                if (prop != null)
                {
                    var v = prop.GetValue(null);
                    if (v is string s && !string.IsNullOrEmpty(s))
                        return s;
                }

                var field = helperType.GetField("CurrentLanguage", flags)
                         ?? helperType.GetField("_currentLanguage", flags)
                         ?? helperType.GetField("Language", flags);
                if (field != null)
                {
                    var v = field.GetValue(null);
                    if (v is string s && !string.IsNullOrEmpty(s))
                        return s;

                    if (v != null) return v.ToString();
                }
            }
            catch { }

            return null;
        }

        private static string TryGetUnityLanguage()
        {
            try
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Russian: return "ru";
                    case SystemLanguage.English: return "en";
                }
            }
            catch { }
            return null;
        }

        private static string NormalizeLanguage(string lang)
        {
            if (string.IsNullOrEmpty(lang)) return null;

            var main = lang.Split('-', '_')[0].Trim().ToLowerInvariant();

            switch (main)
            {
                case "ru": return "Ru";
                case "en": return "En";
                default: return null;
            }
        }
    }
}