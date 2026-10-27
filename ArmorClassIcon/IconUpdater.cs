using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using EFT.Utilities;
using Il2CppInterop.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ArmorClassIcon;

public static class IconUpdater
{
    private enum BadgeLayout
    {
        Inventory,
        Trader,
        Flea
    }

    private const float BadgeGap = 1.5f;
    private const float BadgePadding = 3f;
    private const float TraderBadgeScale = 0.9f;
    private const float FleaBadgeScale = 0.75f;
    private const float BadgeTopReserveRatio = 1.5f;

    private static readonly ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("ArmorClassIcon");

    private static readonly Dictionary<ItemViewStats, List<Image>> CloneCache = new();
    private static readonly Dictionary<GridItemView, List<Image>> BadgeCache = new();
    private static readonly Dictionary<ItemViewStats, Vector2Box> IconBaseSizes = new();
    private static readonly Dictionary<Canvas,
        Dictionary<(EItemViewType? Context, System.Type ViewClass), float>> ReferenceIconHeights = new();

    public static void ClearCache()
    {
        CloneCache.Clear();
        BadgeCache.Clear();
        IconBaseSizes.Clear();
        ReferenceIconHeights.Clear();
    }

    public static void Apply(GridItemView view, ItemViewStats stats, Item item, bool examined)
    {
        if (item == null) return;

        Image icon = stats != null ? stats._armorClassIcon : null;

        HideClones(stats);
        HideBadges(view);

        if (icon != null) icon.gameObject.SetActive(false);

        if (!IsScreenEnabled(view)) return;

        if (!IsEnabled(item)) return;

        IconDisplayMode mode = ResolveDisplayMode(view);
        List<int> classes = CollectClasses(item, mode);

        if (classes.Count == 0) return;

        if (icon != null)
        {
            ShowBadges(view, stats, icon, classes, mode, examined);
            return;
        }

        if (view == null) return;

        ShowDynamicBadges(view, classes, examined);
    }

    private static List<int> CollectClasses(Item item, IconDisplayMode mode)
    {
        var classes = new List<int>();

        if (item.TryGetItemComponent<ArmorHolderComponent>(out var holder)
            && holder.Item is CompoundItem compound)
        {
            var plates = CollectPlateClasses(compound, mode == IconDisplayMode.Custom, out var hasPlates);
            if (plates.Count > 0) classes.AddRange(plates);
        }

        var own = item.GetItemComponent<ArmorComponent>();
        if (own != null && own.ArmorClass > 0) classes.Add(own.ArmorClass);

        foreach (var child in item.GetItemComponentsInChildren<CompositeArmorComponent>())
            if (child.ArmorClass > 0)
                classes.Add(child.ArmorClass);

        if (classes.Count == 0) return classes;

        if (mode == IconDisplayMode.AllPlates || mode == IconDisplayMode.Custom)
        {
            return classes;
        }

        var max = classes.Max();
        var min = classes.Min();

        if (mode == IconDisplayMode.MinMax && min != max) return new List<int> { max, min };

        return new List<int> { max };
    }

    private static List<int> CollectPlateClasses(CompoundItem item, bool filterByToggles, out bool hasPlates)
    {
        var plates = new List<int>();
        hasPlates = false;

        foreach (var slot in item.Slots)
        {
            if (slot is not ArmorSlot armorSlot) continue;

            foreach (var plateItem in armorSlot.Items)
            {
                var plateArmor = plateItem.GetItemComponent<ArmorComponent>();
                if (plateArmor == null || plateArmor.ArmorClass <= 0) continue;

                hasPlates = true;

                if (filterByToggles && !Settings.IsSlotShown(armorSlot.ID)) continue;

                plates.Add(plateArmor.ArmorClass);
            }
        }

        return plates;
    }

    private static IconDisplayMode ResolveDisplayMode(GridItemView view)
    {
        return IsFleaView(view) ? Settings.FleaMode : Settings.InvMode;
    }

    private static bool IsFleaView(GridItemView view)
    {
        var viewType = view?.ItemContext?.ViewType;
        return viewType == EItemViewType.Ragfair || viewType == EItemViewType.NewOffer;
    }

    private static bool IsScreenEnabled(GridItemView view)
    {
        var viewType = view?.ItemContext?.ViewType;

        switch (viewType)
        {
            case EItemViewType.Ragfair:
            case EItemViewType.NewOffer:
                return Settings.ShowOnFlea.Value;

            case EItemViewType.TradingTrader:
            case EItemViewType.TradingSell:
            case EItemViewType.TradingPlayer:
                return Settings.ShowAtTraders.Value;

            default:
                return Settings.ShowInInventory.Value;
        }
    }

    private static void ShowBadges(GridItemView view, ItemViewStats stats, Image icon, List<int> classes,
        IconDisplayMode mode,
        bool examined)
    {
        if (!IconBaseSizes.TryGetValue(stats, out var box))
        {
            var size = icon.rectTransform.sizeDelta;

            if (size.x > 0f && size.y > 0f)
            {
                box = new Vector2Box(size);
                IconBaseSizes[stats] = box;
            }
        }

        IconBaseSizes.TryGetValue(stats, out box);
        var baseSize = box != null ? box.Value : icon.rectTransform.rect.size;

        StoreReferenceIconHeight(view, icon, baseSize);

        var vertical = mode == IconDisplayMode.MinMax;

        ShowBadge(icon, icon, classes[0], 0, examined, vertical, baseSize);

        if (!CloneCache.TryGetValue(stats, out var clones))
        {
            clones = new List<Image>();
            CloneCache[stats] = clones;
        }

        clones.RemoveAll(c => c == null);

        for (var i = 1; i < classes.Count; i++)
        {
            while (clones.Count < i) clones.Add(CreateClone(icon));

            ShowBadge(icon, clones[i - 1], classes[i], i, examined, vertical, baseSize);
        }
    }

    private static void ShowBadge(Image original, Image badge, int armorClass, int index, bool examined, bool vertical,
        Vector2 size)
    {
        var sprite = GetArmorClassSprite(armorClass);
        if (sprite == null) return;

        var offset = vertical
            ? new Vector2(0f, index * (size.y + BadgeGap))
            : new Vector2(index * (size.x + BadgeGap), 0f);

        badge.sprite = sprite;
        badge.rectTransform.anchoredPosition = original.rectTransform.anchoredPosition + offset;
        badge.gameObject.SetActive(examined);
    }

    private static Image CreateClone(Image original)
    {
        var clone = Object.Instantiate<Image>(original, original.transform.parent);
        clone.gameObject.SetActive(false);
        return clone;
    }

    private static void HideClones(ItemViewStats stats)
    {
        if (stats == null) return;
        if (!CloneCache.TryGetValue(stats, out var clones)) return;

        foreach (var clone in clones)
            if (clone != null)
                clone.gameObject.SetActive(false);
    }

    private static void ShowDynamicBadges(GridItemView view, List<int> classes, bool examined)
    {
        if (!BadgeCache.TryGetValue(view, out var badges))
        {
            badges = new List<Image>();
            BadgeCache[view] = badges;
        }

        badges.RemoveAll(b => b == null);

        var flea = IsFleaView(view);

        var sprites = new Sprite[classes.Count];
        var sizes = new Vector2[classes.Count];

        for (var i = 0; i < classes.Count; i++)
        {
            sprites[i] = GetArmorClassSprite(classes[i]);

            if (sprites[i] == null) continue;

            sizes[i] = flea
                ? sprites[i].rect.size * FleaBadgeScale
                : GetLocalBadgeSize(view, sprites[i]);
        }

        if (!flea) FitBadgesToView(view, sizes);

        for (var i = 0; i < classes.Count; i++)
        {
            if (sprites[i] == null) continue;

            while (badges.Count <= i) badges.Add(CreateBadge(view));

            var size = sizes[i];

            var badge = badges[i];
            badge.sprite = sprites[i];
            badge.rectTransform.sizeDelta = size;
            badge.rectTransform.anchoredPosition = new Vector2(
                BadgePadding,
                BadgePadding + (classes.Count - 1 - i) * (size.y + BadgeGap));
            badge.gameObject.SetActive(examined);
        }
    }

    private static Vector2 GetLocalBadgeSize(GridItemView view, Sprite sprite)
    {
        float viewScale = Mathf.Abs(view.transform.lossyScale.y);
        Canvas canvas = GetRootCanvas(view);

        if (canvas != null && viewScale > 0.0001f && sprite.rect.height > 0f
            && ReferenceIconHeights.TryGetValue(canvas,
                out Dictionary<(EItemViewType? Context, System.Type ViewClass), float> heights))
        {
            BadgeLayout layout = ResolveBadgeLayout(view);
            EItemViewType? context = view.ItemContext?.ViewType;
            bool exactMatch = heights.TryGetValue((context, view.GetType()), out float worldHeight);
            bool contextMatch = !exactMatch && TryGetContextHeight(heights, context, out worldHeight);
            bool inventoryFallback = false;

            if (!exactMatch && !contextMatch)
            {
                EItemViewType fallbackContext = layout == BadgeLayout.Trader
                    ? EItemViewType.TradingPlayer
                    : EItemViewType.Inventory;

                if (!TryGetContextHeight(heights, fallbackContext, out worldHeight)
                    && fallbackContext != EItemViewType.Inventory)
                    inventoryFallback = TryGetContextHeight(heights, EItemViewType.Inventory, out worldHeight);
            }

            if (worldHeight > 0f)
            {
                float layoutScale = inventoryFallback ? TraderBadgeScale : 1f;
                float targetHeight = worldHeight / viewScale * layoutScale;
                return sprite.rect.size * (targetHeight / sprite.rect.height);
            }
        }

        return sprite.rect.size * (ResolveBadgeLayout(view) == BadgeLayout.Trader ? TraderBadgeScale : 1f);
    }

    private static bool TryGetContextHeight(
        Dictionary<(EItemViewType? Context, System.Type ViewClass), float> heights,
        EItemViewType? context,
        out float height)
    {
        float total = 0f;
        int count = 0;

        foreach (KeyValuePair<(EItemViewType? Context, System.Type ViewClass), float> entry in heights)
        {
            if (entry.Key.Context != context) continue;

            total += entry.Value;
            count++;
        }

        height = count > 0 ? total / count : 0f;
        return count > 0;
    }

    private static void StoreReferenceIconHeight(GridItemView view, Image icon, Vector2 baseSize)
    {
        if (view == null) return;

        Canvas canvas = GetRootCanvas(icon);
        float worldHeight = baseSize.y * Mathf.Abs(icon.transform.lossyScale.y);

        if (canvas == null || worldHeight <= 0.0001f || float.IsNaN(worldHeight) || float.IsInfinity(worldHeight)) return;

        if (!ReferenceIconHeights.TryGetValue(canvas, out var heights))
        {
            heights = new Dictionary<(EItemViewType? Context, System.Type ViewClass), float>();
            ReferenceIconHeights[canvas] = heights;
        }

        heights[(view.ItemContext?.ViewType, view.GetType())] = worldHeight;
    }

    private static Canvas GetRootCanvas(Component component)
    {
        Canvas canvas = component != null ? component.GetComponentInParent<Canvas>() : null;
        return canvas != null ? canvas.rootCanvas : null;
    }

    private static BadgeLayout ResolveBadgeLayout(GridItemView view)
    {
        EItemViewType? viewType = view?.ItemContext?.ViewType;

        return viewType switch
        {
            EItemViewType.Ragfair or EItemViewType.NewOffer => BadgeLayout.Flea,
            EItemViewType.TradingTrader or EItemViewType.TradingSell or EItemViewType.TradingPlayer =>
                BadgeLayout.Trader,
            _ => BadgeLayout.Inventory
        };
    }

    private static void FitBadgesToView(GridItemView view, Vector2[] sizes)
    {
        var rect = view.RectTransform.rect;
        if (rect.height <= 0f || rect.width <= 0f) return;

        var count = 0;
        var height = 0f;
        var width = 0f;

        foreach (var size in sizes)
        {
            if (size == Vector2.zero) continue;

            count++;
            height = size.y;
            width = Mathf.Max(width, size.x);
        }

        if (count == 0 || height <= 0f || width <= 0f) return;

        var neededHeight = count * height + (count - 1) * BadgeGap + 2f * BadgePadding + height * BadgeTopReserveRatio;
        var fitHeight = Mathf.Min(1f, (rect.height - 2f * BadgePadding) / neededHeight);
        var fitWidth = Mathf.Min(1f, (rect.width - 2f * BadgePadding) / width);
        var fit = Mathf.Max(Mathf.Min(fitHeight, fitWidth), 0.05f);

        if (fit >= 1f) return;

        for (var i = 0; i < sizes.Length; i++) sizes[i] *= fit;
    }

    private static Image CreateBadge(GridItemView view)
    {
        var go = new GameObject("ArmorClassBadge", Il2CppType.Of<RectTransform>(), Il2CppType.Of<Image>());
        var badge = go.GetComponent<Image>();
        badge.raycastTarget = false;
        badge.preserveAspect = true;
        badge.transform.SetParent(view.transform, false);

        var rect = badge.rectTransform;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);

        badge.gameObject.SetActive(false);
        return badge;
    }

    private static void HideBadges(GridItemView view)
    {
        if (view == null) return;
        if (!BadgeCache.TryGetValue(view, out var badges)) return;

        foreach (var badge in badges)
            if (badge != null)
                badge.gameObject.SetActive(false);
    }

    private static bool IsEnabled(Item item)
    {
        bool hasArmorHolder = item.TryGetItemComponent<ArmorHolderComponent>(out _);
        
        if (hasArmorHolder)
            return Settings.EnableBodyArmor.Value || Settings.EnableArmoredRigs.Value;

        bool hasComposite = false;
        foreach (var _ in item.GetItemComponentsInChildren<CompositeArmorComponent>())
        {
            hasComposite = true;
            break;
        }

        if (hasComposite)
            return Settings.EnableHeadwear.Value || Settings.EnableArmoredRigs.Value || Settings.EnableVisors.Value || Settings.EnableFaceCovers.Value;

        bool hasArmor = item.TryGetItemComponent<ArmorComponent>(out _);
        if (hasArmor)
            return Settings.EnableHeadwear.Value || Settings.EnableVisors.Value || Settings.EnableFaceCovers.Value || Settings.EnableAccessories.Value;

        return false;
    }

    private static Sprite GetArmorClassSprite(int armorClass)
    {
        var sprite = ResourcesCache.Pop<Sprite>("Mod Types/icon_type_mod_armor_plate_" + armorClass);
        if (sprite == null)
        {
            Log.LogWarning($"[ArmorClassIcon] Не удалось загрузить спрайт: Mod Types/icon_type_mod_armor_plate_{armorClass}");
        }
        return sprite;
    }

    private class Vector2Box
    {
        public Vector2 Value { get; }
        public Vector2Box(Vector2 value) { Value = value; }
    }
}