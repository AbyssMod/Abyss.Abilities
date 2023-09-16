using System;
using Abyss.Events;
using Abyss.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace Abyss.Api.Abilities;

[PublicAPI]
public abstract class ModAbilityData<T> : ScriptableObjectModContent<AbilityData> where T : ModAbility
{
    /// <summary>
    /// The instance of your <typeparam name="T"></typeparam>
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

    public virtual bool AutoUnlock => true;

    /// <summary>
    /// Unlocks this ability ingame, <see cref="GameManager.PlayerAbilities"/> needs to not be null
    /// </summary>
    public void Unlock()
    {
        GameManager.Instance.PlayerAbilities.UnlockAbility(Item.name.ToLower());
    }

    /// <inheritdoc />
    public override void Register()
    {
        base.Register();
        Ability = Abyss.Abilities.AbilityManager.AddComponent<T>();
        Ability.abilityData = Item;
        Item.nameKey = LocalizationManager.CreateLocalizedString(Id, DisplayName);
        Item.descriptionKey = LocalizationManager.CreateLocalizedString(Id + "_Description", Description);
        Item.icon = IconImage;

        Edit(Item);

        /*AbyssEvents.OnGameSceneLoaded.AddListener(() =>
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
        });*/
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