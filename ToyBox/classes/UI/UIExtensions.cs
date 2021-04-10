﻿// Copyright < 2021 > Narria(github user Cabarius) - License: MIT
using UnityEngine;
using UnityModManagerNet;
using UnityEngine.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Components;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Shields;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Quests;
using Kingmaker.Blueprints.Root;
using Kingmaker.Cheats;
using Kingmaker.Controllers.Rest;
using Kingmaker.Designers;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.GameModes;
using Kingmaker.Items;
using Kingmaker.Kingdom;
using Kingmaker.Kingdom.Blueprints;
using Kingmaker.Kingdom.Settlements;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Utility;

namespace ToyBox {
    public static class UIExtensions {
        public static Settings settings { get { return Main.settings; } }
        public static void ResetGUI() { }

        // convenience extensions for constructing UI for special types
        public static void ActionButton<T>(this NamedAction<T> namedAction, T value, Action buttonAction, float width = 0) {
            if (namedAction != null && namedAction.canPerform(value)) {
                UI.ActionButton(namedAction.name, buttonAction, width == 0 ? UI.AutoWidth() : UI.Width(width));
            }
            else {
                UI.Space(width + 3);
            }
        }
        public static void MutatorButton<U, T>(this NamedMutator<U, T> muator, U unit, T value, Action buttonAction, float width = 0) {
            if (muator != null && muator.canPerform(unit, value)) {
                UI.ActionButton(muator.name, buttonAction, width == 0 ? UI.AutoWidth() : UI.Width(width));
            }
            else {
                UI.Space(width + 3);
            }
        }
        public static void BlueprintActionButton<T>(this BlueprintAction action, UnitEntityData unit, BlueprintScriptableObject bp, Action buttonAction, float width) {
            if (action != null && action.canPerform(unit, bp)) {
                UI.ActionButton(action.name, buttonAction, UI.Width(width));
            }
            else {
                UI.Space(width + 3);
            }
        }
    }
}