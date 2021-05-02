﻿// Copyright < 2021 > Narria (github user Cabarius) - License: MIT
using UnityEngine;
using UnityModManagerNet;
using UnityEngine.UI;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
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
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Utility;
using ModKit;

namespace ToyBox {
    public class BlueprintListUI {
        public static Settings settings { get { return Main.settings; } }

        public static int repeatCount = 1;
        public static bool hasRepeatableAction = false;
        public static int maxActions = 0;
        public static bool needsLayout = true;
        public static void OnGUI(UnitEntityData ch,
            IEnumerable<SimpleBlueprint> blueprints,
            float indent = 0, float remainingWidth = 0,
            Func<String, String> titleFormater = null,
            NamedTypeFilter typeFilter = null
        ) {
            if (titleFormater == null) titleFormater = (t) => t.orange().bold();
            int index = 0;
            if (needsLayout) {
                foreach (SimpleBlueprint blueprint in blueprints) {
                    var actions = blueprint.GetActions();
                    if (actions.Any(a => a.isRepeatable)) hasRepeatableAction = true;
                    int actionCount = actions.Sum(action => action.canPerform(blueprint, ch) ? 1 : 0);
                    maxActions = Math.Max(actionCount, maxActions);
                }
                needsLayout = false;
            }
            if (hasRepeatableAction) {
                UI.BeginHorizontal();
                UI.Space(553);
                UI.ActionIntTextField(
                    ref repeatCount,
                    "repeatCount",
                    (limit) => { },
                    () => { },
                    UI.Width(160));
                UI.Space(40);
                UI.Label("Parameter".cyan() + ": " + $"{repeatCount}".orange(), UI.ExpandWidth(false));
                repeatCount = Math.Max(1, repeatCount);
                repeatCount = Math.Min(100, repeatCount);
                UI.EndHorizontal();
            }
            UI.Div(indent);
            foreach (SimpleBlueprint blueprint in blueprints) {
                Rect rect;
                var description = blueprint.GetDescription();
                using (UI.HorizontalScope()) {
                    UI.Space(indent);
                    var actions = blueprint.GetActions().Where(action => action.canPerform(blueprint, ch)).ToArray();
                    var titles = actions.Select(a => a.name);
                    var title = blueprint.name;
                    if (titles.Contains("Remove")) {
                        title = title.cyan().bold();
                    }
                    else {
                        title = titleFormater(title);
                    }

                    UI.Label(title, UI.Width(550 - indent));
                    int actionCount = actions != null ? actions.Count() : 0;
                    for (int ii = 0; ii < maxActions; ii++) {
                        if (ii < actionCount) {
                            BlueprintAction action = actions[ii];
                            // TODO -don't show increase or decrease actions until we redo actions into a proper value editor that gives us Add/Remove and numeric item with the ability to show values.  For now users can edit ranks in the Facts Editor
                            if (action.name == "<" || action.name == ">") {
                                UI.Space(164); continue;
                            }
                            var actionName = action.name;
                            float extraSpace = 0;
                            if (action.isRepeatable) {
                                actionName += (action.isRepeatable ? $" {repeatCount}" : "");
                                extraSpace = 20 * (float)Math.Ceiling(Math.Log10((double)repeatCount));
                            }
                            UI.ActionButton(actionName, () => { action.action(blueprint, ch, repeatCount); }, UI.Width(160 + extraSpace));
                            UI.Space(10);
                        }
                        else {
                            UI.Space(174);
                        }
                    }
                    UI.Space(30);
                    rect = GUILayoutUtility.GetLastRect();
                    String typeString = blueprint.GetType().Name;
                    if (typeFilter?.collator != null) {
                        var collatorString = typeFilter.collator(blueprint);
                        if (!typeString.Contains(collatorString))
                            typeString += $" : {collatorString}".yellow();
                    }
                    if (description != null && description.Length > 0) description = $"{description}";
                    else description = "";
                    if (blueprint is BlueprintScriptableObject bpso) {
                        if (settings.showComponents && bpso.ComponentsArray?.Length > 0) {
                            String componentStr = String.Join<object>(" ", bpso.ComponentsArray).grey();
                            if (description.Length == 0) description = componentStr;
                            else description = componentStr + "\n" + description;
                        }
                        if (settings.showElements && bpso.ElementsArray?.Length > 0) {
                            String elementsStr = String.Join<object>(" ", bpso.ElementsArray).magenta();
                            if (description.Length == 0) description = elementsStr;
                            else description = elementsStr + "\n" + description;
                        }
                    }
                    using (UI.VerticalScope(UI.Width(900))) {
                        if (settings.showAssetIDs) {
                            using (UI.HorizontalScope()) {
                                UI.Label(typeString.cyan());
                                GUILayout.TextField(blueprint.AssetGuid, UI.Width(450));
                            }
                        }
                        else UI.Label(typeString.cyan());
                        if (description.Length > 0) UI.Label(description.green());
                    }
                }
#if false
                String description = blueprint.GetDescription();
                if (description.Length > 0) {
                    UI.BeginHorizontal();
                    UI.Space(684 + maxActions * 154);
                    UI.Label($"{description.green()}");
                    UI.EndHorizontal();
                }
#endif
                UI.Div(indent);
                index++;
            }
        }
    }
}