using System;
using Abyss.Events;
using Abyss.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Abyss.Api.Abilities;

/// <summary>
/// The base class for all custom abilities
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public abstract class ModAbilityData<T> : ScriptableObjectModContent<AbilityData> where T : ModAbility
{
    /// <summary>
    /// The instance of your ModAbility
    /// </summary>
    public T Ability { get; set; } = null!;

    /// <summary>
    /// The file name of your texture, without the extension
    /// </summary>
    public virtual string Icon => GetType().Name + "-Icon";

    /// <summary>
    /// If you arent gonna use <see cref="Icon"/>, use this to manually set the sprite
    /// </summary>
    public virtual Sprite? IconImage => GetSprite(Icon);

    /// <summary>
    /// Should this ability be unlocked by default
    /// </summary>
    public virtual bool AutoUnlock => true;

    /// <summary>
    /// Unlocks this ability ingame, using the default implementation, <see cref="GameManager.PlayerAbilities"/> needs to not be null
    /// </summary>
    public virtual void Unlock()
    {
        GameManager.Instance.PlayerAbilities.UnlockAbility(Item.name.ToLower());
    }

    public VibrationData primaryVibration;
    public VibrationData secondaryVibration;
    public AssetReference castSFX;

    [Header("Dependent Item Configuration")]
    public ItemData[] linkedItems;

    public ItemSubtype linkedItemSubtype;
    public bool allowDamagedItems;
    public bool allowExhaustedItems;
    [Header("Timings")] public float duration;
    public float cooldown;
    public float castTime;
    [Header("Behaviour")] public bool isContinuous;
    public bool deactivateOnInputLayerChanged;
    public bool requiresAbilityFocus;
    public bool persistAbilityToggle;
    public bool canFailCast;
    public bool showsCounter;
    public bool allowExitAction;
    public ActionLayer exitActionLayer;
    public float sfxRepeatThreshold;

    /// <inheritdoc />
    public override void Register()
    {
        base.Register();
        Ability = Abyss.Abilities.AbilityManager.AddComponent<T>();
        Ability.abilityData = Item;
        Item.nameKey = LocalizationManager.CreateLocalizedString(Id, DisplayName);
        Item.descriptionKey = LocalizationManager.CreateLocalizedString(Id + "_Description", Description);
        Item.icon = IconImage;
        Item.primaryVibration = primaryVibration;
        Item.secondaryVibration = secondaryVibration;
        Item.castSFX = castSFX;
        Item.linkedItems = linkedItems;
        Item.linkedItemSubtype = linkedItemSubtype;
        Item.allowDamagedItems = allowDamagedItems;
        Item.allowExhaustedItems = allowExhaustedItems;
        Item.duration = duration;
        Item.cooldown = cooldown;
        Item.castTime = castTime;
        Item.isContinuous = isContinuous;
        Item.deactivateOnInputLayerChanged = deactivateOnInputLayerChanged;
        Item.requiresAbilityFocus = requiresAbilityFocus;
        Item.persistAbilityToggle = persistAbilityToggle;
        Item.canFailCast = canFailCast;
        Item.showsCounter = showsCounter;
        Item.allowExitAction = allowExitAction;
        Item.exitActionLayer = exitActionLayer;
        Item.sfxRepeatThreshold = sfxRepeatThreshold;


        Edit(Item);

        ApplicationEvents.Instance.OnGameLoaded += () =>
        {
            try
            {
                GameManager.Instance.PlayerAbilities.RegisterAbility(Item, Ability);
                if (AutoUnlock)
                {
                    Unlock();
                }
            }
            catch (Exception e)
            {
                AbyssLogger.Error("Error registering custom ability: \n" + e);
            }
        };
    }
}