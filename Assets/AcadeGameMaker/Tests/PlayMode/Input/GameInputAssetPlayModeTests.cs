using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AcadeGameMaker.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace AcadeGameMaker.Tests.PlayMode.Input
{
    // REQ-UX-001/006/007/009, REQ-PLAT-002/007; AC-M5B3-001..005.
    // This fixture verifies the device/action foundation only. It deliberately
    // does not claim InputRouter, InputMode ownership, or next-tick buffering.
    internal sealed class GameInputAssetPlayModeTests : InputTestFixture
    {
        private static readonly string[] GameplayActionNames =
        {
            "Move", "Aim", "Point", "Jump", "Dash", "Interact", "ChoiceSkill", "Attack", "Transfer", "Pause"
        };

        private static readonly string[] UiActionNames =
        {
            "Navigate", "Point", "Click", "ScrollWheel", "Submit", "Cancel"
        };

        private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;
        private static string AssetPath => Path.Combine(ProjectRoot, "Assets", "GameInput.inputactions");
        private static string WrapperPath => Path.Combine(ProjectRoot, "Assets", "AcadeGameMaker", "Runtime", "Input", "GameInputActions.cs");

        [Test]
        public void SourceAssetHasExactMapsActionsSchemesAndStableIds()
        {
            var asset = LoadSourceAsset();
            try
            {
                Assert.That(asset.actionMaps.Select(map => map.name), Is.EquivalentTo(new[] { "Gameplay", "UI" }));
                Assert.That(asset.controlSchemes.Select(scheme => scheme.name), Is.EquivalentTo(new[] { "KeyboardMouse", "Gamepad" }));

                var gameplay = asset.FindActionMap("Gameplay", true);
                var ui = asset.FindActionMap("UI", true);
                Assert.That(gameplay.actions.Select(action => action.name), Is.EquivalentTo(GameplayActionNames));
                Assert.That(ui.actions.Select(action => action.name), Is.EquivalentTo(UiActionNames));
                Assert.That(gameplay.bindings.Count, Is.EqualTo(23));
                Assert.That(ui.bindings.Count, Is.EqualTo(20));
                Assert.That(asset.bindings.Count(), Is.EqualTo(43));

            Assert.That(gameplay.FindAction("Move", true).type, Is.EqualTo(InputActionType.Value));
            Assert.That(gameplay.FindAction("Aim", true).type, Is.EqualTo(InputActionType.Value));
            Assert.That(gameplay.FindAction("Point", true).type, Is.EqualTo(InputActionType.PassThrough));
            Assert.That(ui.FindAction("Navigate", true).type, Is.EqualTo(InputActionType.Value));
            Assert.That(ui.FindAction("Point", true).type, Is.EqualTo(InputActionType.PassThrough));
            Assert.That(ui.FindAction("ScrollWheel", true).type, Is.EqualTo(InputActionType.PassThrough));

            AssertBinding(gameplay, "Move", "<Keyboard>/w", "KeyboardMouse");
            AssertBinding(gameplay, "Move", "<Keyboard>/a", "KeyboardMouse");
            AssertBinding(gameplay, "Move", "<Keyboard>/s", "KeyboardMouse");
            AssertBinding(gameplay, "Move", "<Keyboard>/d", "KeyboardMouse");
            AssertBinding(gameplay, "Move", "<Gamepad>/leftStick", "Gamepad");
            AssertBinding(gameplay, "Point", "<Mouse>/position", "KeyboardMouse");
            AssertBinding(gameplay, "Aim", "<Gamepad>/rightStick", "Gamepad");
            AssertBinding(gameplay, "Jump", "<Keyboard>/space", "KeyboardMouse");
            AssertBinding(gameplay, "Jump", "<Gamepad>/buttonSouth", "Gamepad");
            AssertBinding(gameplay, "Dash", "<Keyboard>/leftShift", "KeyboardMouse");
            AssertBinding(gameplay, "Dash", "<Keyboard>/rightShift", "KeyboardMouse");
            AssertBinding(gameplay, "Dash", "<Gamepad>/buttonEast", "Gamepad");
            AssertBinding(gameplay, "Interact", "<Keyboard>/e", "KeyboardMouse");
            AssertBinding(gameplay, "Interact", "<Gamepad>/buttonWest", "Gamepad");
            AssertBinding(gameplay, "ChoiceSkill", "<Keyboard>/q", "KeyboardMouse");
            AssertBinding(gameplay, "ChoiceSkill", "<Gamepad>/rightShoulder", "Gamepad");
            AssertBinding(gameplay, "Attack", "<Mouse>/leftButton", "KeyboardMouse");
            AssertBinding(gameplay, "Attack", "<Gamepad>/rightTrigger", "Gamepad");
            AssertBinding(gameplay, "Transfer", "<Mouse>/rightButton", "KeyboardMouse");
            AssertBinding(gameplay, "Transfer", "<Gamepad>/leftTrigger", "Gamepad");
            AssertBinding(gameplay, "Pause", "<Keyboard>/escape", "KeyboardMouse");
            AssertBinding(gameplay, "Pause", "<Gamepad>/start", "Gamepad");

            AssertBinding(ui, "Navigate", "<Keyboard>/w", "KeyboardMouse");
            AssertBinding(ui, "Navigate", "<Keyboard>/upArrow", "KeyboardMouse");
            AssertBinding(ui, "Navigate", "<Keyboard>/downArrow", "KeyboardMouse");
            AssertBinding(ui, "Navigate", "<Keyboard>/leftArrow", "KeyboardMouse");
            AssertBinding(ui, "Navigate", "<Keyboard>/rightArrow", "KeyboardMouse");
            AssertBinding(ui, "Navigate", "<Gamepad>/dpad", "Gamepad");
            AssertBinding(ui, "Navigate", "<Gamepad>/leftStick", "Gamepad");
            AssertBinding(ui, "Point", "<Mouse>/position", "KeyboardMouse");
            AssertBinding(ui, "Click", "<Mouse>/leftButton", "KeyboardMouse");
            AssertBinding(ui, "ScrollWheel", "<Mouse>/scroll", "KeyboardMouse");
            AssertBinding(ui, "Submit", "<Keyboard>/enter", "KeyboardMouse");
            AssertBinding(ui, "Submit", "<Keyboard>/space", "KeyboardMouse");
            AssertBinding(ui, "Submit", "<Gamepad>/buttonSouth", "Gamepad");
            AssertBinding(ui, "Cancel", "<Keyboard>/escape", "KeyboardMouse");
            AssertBinding(ui, "Cancel", "<Gamepad>/buttonEast", "Gamepad");

            var ids = new List<Guid>();
            ids.AddRange(asset.actionMaps.Select(map => map.id));
            ids.AddRange(asset.actionMaps.SelectMany(map => map.actions).Select(action => action.id));
            ids.AddRange(asset.bindings.Select(binding => binding.id));
            Assert.That(ids, Is.Not.Empty);
            Assert.That(ids, Is.Unique);
            foreach (var id in ids)
            {
                Assert.That(id, Is.Not.EqualTo(Guid.Empty), "Input ID is empty.");
            }

            var rawIds = Regex.Matches(File.ReadAllText(AssetPath), "\\\"id\\\"\\s*:\\s*\\\"([^\\\"]+)\\\"")
                .Cast<Match>().Select(match => match.Groups[1].Value).ToArray();
            Assert.That(rawIds.Length, Is.EqualTo(61));
            Assert.That(rawIds.Select(value => Guid.Parse(value)).Distinct().Count(), Is.EqualTo(61));

            foreach (var binding in gameplay.bindings.Concat(ui.bindings))
            {
                var path = binding.path ?? string.Empty;
                Assert.That(path, Is.Not.EqualTo("<Keyboard>/j"));
                Assert.That(path, Is.Not.EqualTo("<Keyboard>/k"));
                Assert.That(path, Is.Not.EqualTo("<Keyboard>/l"));
            }

            var movePaths = gameplay.FindAction("Move", true).bindings.Select(binding => binding.path ?? string.Empty).ToArray();
            Assert.That(movePaths.Any(path => path == "<Mouse>/leftButton" || path == "<Mouse>/rightButton"), Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(asset);
            }
        }

        [Test]
        public void GeneratedWrapperMatchesSourceAndOfficialRegenerationIsStable()
        {
            var source = LoadSourceAsset();
            try
            {
                using (var wrapper = new GameInputActions())
                {
                    Assert.That(wrapper.asset.actionMaps.Count, Is.EqualTo(source.actionMaps.Count));
                    Assert.That(wrapper.asset.ToJson(), Is.EqualTo(source.ToJson()));
                    foreach (var sourceMap in source.actionMaps)
                    {
                        var generatedMap = wrapper.asset.FindActionMap(sourceMap.id);
                        Assert.That(generatedMap, Is.Not.Null);
                        Assert.That(generatedMap.name, Is.EqualTo(sourceMap.name));
                        Assert.That(generatedMap.id, Is.EqualTo(sourceMap.id));
                        Assert.That(generatedMap.actions.Count, Is.EqualTo(sourceMap.actions.Count));
                        Assert.That(generatedMap.bindings.Count, Is.EqualTo(sourceMap.bindings.Count));
                        for (var index = 0; index < sourceMap.actions.Count; index++)
                        {
                            Assert.That(generatedMap.actions[index].id, Is.EqualTo(sourceMap.actions[index].id));
                            Assert.That(generatedMap.actions[index].name, Is.EqualTo(sourceMap.actions[index].name));
                        }
                        for (var index = 0; index < sourceMap.bindings.Count; index++)
                        {
                            var expected = sourceMap.bindings[index];
                            var actual = generatedMap.bindings[index];
                            Assert.That(actual.id, Is.EqualTo(expected.id));
                            Assert.That(actual.path, Is.EqualTo(expected.path));
                            Assert.That(actual.groups, Is.EqualTo(expected.groups));
                            Assert.That(actual.action, Is.EqualTo(expected.action));
                        }
                    }

                    Assert.That(wrapper.Gameplay.enabled, Is.False);
                    Assert.That(wrapper.UI.enabled, Is.False);
                    Assert.That(wrapper.asset.enabled, Is.False);
                    wrapper.Gameplay.Disable();
                    wrapper.UI.Disable();
                    wrapper.asset.Disable();
                }

                Assert.That(File.Exists(WrapperPath), Is.True);
                var generatorType = FindLoadedType("UnityEngine.InputSystem.Editor.InputActionCodeGenerator");
                Assert.That(generatorType, Is.Not.Null, "The installed official Input System code generator is unavailable.");
                var optionsType = generatorType.GetNestedType("Options", BindingFlags.Public | BindingFlags.NonPublic);
                Assert.That(optionsType, Is.Not.Null);
                var options = Activator.CreateInstance(optionsType);
                optionsType.GetProperty("className").SetValue(options, "GameInputActions");
                optionsType.GetProperty("namespaceName").SetValue(options, "AcadeGameMaker.Input");
                optionsType.GetProperty("sourceAssetPath").SetValue(options, "Assets/GameInput.inputactions");
                var generate = generatorType.GetMethod(
                    "GenerateWrapperCode",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(InputActionAsset), optionsType },
                    null);
                Assert.That(generate, Is.Not.Null);
                var regenerated = (string)generate.Invoke(null, new[] { source, options });
                Assert.That(NormalizeNewlines(regenerated), Is.EqualTo(NormalizeNewlines(File.ReadAllText(WrapperPath))));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(source);
            }
        }

        [Test]
        public void VirtualGameplayDevicesDriveBothApprovedSchemes()
        {
            using (var actions = new GameInputActions())
            {
                var counts = Subscribe(actions.Gameplay);
                actions.Gameplay.Enable();
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var mouse = InputSystem.AddDevice<Mouse>();
                var gamepad = InputSystem.AddDevice<Gamepad>();

                PressAndRelease(keyboard.wKey);
                Set(mouse.position, new Vector2(320, 180));
                PressAndRelease(keyboard.spaceKey);
                PressAndRelease(keyboard.leftShiftKey);
                PressAndRelease(keyboard.eKey);
                PressAndRelease(keyboard.qKey);
                PressAndRelease(mouse.leftButton);
                PressAndRelease(mouse.rightButton);
                PressAndRelease(keyboard.escapeKey);

                Set(gamepad.leftStick, new Vector2(0.75f, 0));
                Set(gamepad.leftStick, Vector2.zero);
                Set(gamepad.rightStick, new Vector2(0.75f, 0));
                Set(gamepad.rightStick, Vector2.zero);
                PressAndRelease(gamepad.buttonSouth);
                PressAndRelease(gamepad.buttonEast);
                PressAndRelease(gamepad.buttonWest);
                PressAndRelease(gamepad.rightShoulder);
                PressAndRelease(gamepad.rightTrigger);
                PressAndRelease(gamepad.leftTrigger);
                PressAndRelease(gamepad.startButton);

                foreach (var name in GameplayActionNames)
                {
                    Assert.That(counts[name], Is.GreaterThan(0), "No callback for Gameplay/" + name);
                }
                actions.Disable();
            }
        }

        [Test]
        public void VirtualUiDevicesDriveProtectedDefaultsWithoutVirtualMouse()
        {
            using (var actions = new GameInputActions())
            {
                var counts = Subscribe(actions.UI);
                actions.UI.Enable();
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var mouse = InputSystem.AddDevice<Mouse>();
                var gamepad = InputSystem.AddDevice<Gamepad>();

                PressAndRelease(keyboard.wKey);
                PressAndRelease(keyboard.upArrowKey);
                Set(mouse.position, new Vector2(320, 180));
                PressAndRelease(mouse.leftButton);
                Set(mouse.scroll, new Vector2(0, 120));
                PressAndRelease(keyboard.enterKey);
                PressAndRelease(keyboard.spaceKey);
                PressAndRelease(keyboard.escapeKey);
                PressAndRelease(gamepad.dpad.up);
                PressAndRelease(gamepad.buttonSouth);
                PressAndRelease(gamepad.buttonEast);

                foreach (var name in UiActionNames)
                {
                    Assert.That(counts[name], Is.GreaterThan(0), "No callback for UI/" + name);
                }

                Assert.That(InputSystem.devices.Any(device => device.layout.IndexOf("VirtualMouse", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
                actions.Disable();
            }
        }

        [Test]
        public void WrapperMapsStartDisabledAndExplicitMapToggleIsExclusiveInFixture()
        {
            using (var actions = new GameInputActions())
            {
                Assert.That(actions.asset.enabled, Is.False);
                Assert.That(actions.Gameplay.enabled, Is.False);
                Assert.That(actions.UI.enabled, Is.False);
                var gameplayCallbacks = Subscribe(actions.Gameplay);
                var uiCallbacks = Subscribe(actions.UI);
                var keyboard = InputSystem.AddDevice<Keyboard>();

                PressAndRelease(keyboard.wKey);
                Assert.That(gameplayCallbacks["Move"], Is.Zero);
                Assert.That(uiCallbacks["Navigate"], Is.Zero);

                actions.UI.Enable();
                Assert.That(actions.Gameplay.enabled, Is.False);
                Assert.That(actions.UI.enabled, Is.True);
                PressAndRelease(keyboard.wKey);
                Assert.That(uiCallbacks["Navigate"], Is.GreaterThan(0));
                Assert.That(gameplayCallbacks["Move"], Is.Zero);

                actions.UI.Disable();
                actions.Gameplay.Enable();
                Assert.That(actions.UI.enabled, Is.False);
                Assert.That(actions.Gameplay.enabled, Is.True);
                PressAndRelease(keyboard.wKey);
                Assert.That(gameplayCallbacks["Move"], Is.GreaterThan(0));
                actions.Disable();
            }
        }

        [Test]
        public void BootstrapStillUsesInputSystemOnlyAndRejectsLegacyTemplateAsset()
        {
            var manifest = File.ReadAllText(Path.Combine(ProjectRoot, "Packages", "manifest.json"));
            var settings = File.ReadAllText(Path.Combine(ProjectRoot, "ProjectSettings", "ProjectSettings.asset"));
            StringAssert.Contains("\"com.unity.inputsystem\": \"1.20.0\"", manifest);
            StringAssert.Contains("activeInputHandler: 1", settings);
            Assert.That(File.Exists(Path.Combine(ProjectRoot, "Assets", "InputSystem_Actions.inputactions")), Is.False);
            Assert.That(File.Exists(AssetPath), Is.True);
            Assert.That(File.Exists(WrapperPath), Is.True);
        }

        // AC-M5B3-003/004: each physical binding must work independently;
        // one working scheme cannot conceal a broken alias in the other.
        [TestCase("Gameplay", "Jump", "Keyboard", "space")]
        [TestCase("Gameplay", "Jump", "Gamepad", "buttonSouth")]
        [TestCase("Gameplay", "Dash", "Keyboard", "leftShift")]
        [TestCase("Gameplay", "Dash", "Keyboard", "rightShift")]
        [TestCase("Gameplay", "Dash", "Gamepad", "buttonEast")]
        [TestCase("Gameplay", "Interact", "Keyboard", "e")]
        [TestCase("Gameplay", "Interact", "Gamepad", "buttonWest")]
        [TestCase("Gameplay", "ChoiceSkill", "Keyboard", "q")]
        [TestCase("Gameplay", "ChoiceSkill", "Gamepad", "rightShoulder")]
        [TestCase("Gameplay", "Attack", "Mouse", "leftButton")]
        [TestCase("Gameplay", "Attack", "Gamepad", "rightTrigger")]
        [TestCase("Gameplay", "Transfer", "Mouse", "rightButton")]
        [TestCase("Gameplay", "Transfer", "Gamepad", "leftTrigger")]
        [TestCase("Gameplay", "Pause", "Keyboard", "escape")]
        [TestCase("Gameplay", "Pause", "Gamepad", "start")]
        [TestCase("UI", "Submit", "Keyboard", "enter")]
        [TestCase("UI", "Submit", "Keyboard", "space")]
        [TestCase("UI", "Submit", "Gamepad", "buttonSouth")]
        [TestCase("UI", "Cancel", "Keyboard", "escape")]
        [TestCase("UI", "Cancel", "Gamepad", "buttonEast")]
        [TestCase("UI", "Click", "Mouse", "leftButton")]
        public void EachButtonBindingPerformsExactlyOnce(string mapName, string actionName, string layout, string controlName)
        {
            using (var actions = new GameInputActions())
            {
                var device = InputSystem.AddDevice(layout);
                var action = actions.asset.FindActionMap(mapName, true).FindAction(actionName, true);
                var performed = 0;
                action.performed += _ => performed++;
                action.actionMap.Enable();
                try
                {
                    PressAndRelease(device.GetChildControl<ButtonControl>(controlName));
                    Assert.That(performed, Is.EqualTo(1));
                }
                finally { actions.Disable(); }
            }
        }

        [TestCase("Gameplay", "Move", "w", 0, 1)]
        [TestCase("Gameplay", "Move", "s", 0, -1)]
        [TestCase("Gameplay", "Move", "a", -1, 0)]
        [TestCase("Gameplay", "Move", "d", 1, 0)]
        [TestCase("UI", "Navigate", "w", 0, 1)]
        [TestCase("UI", "Navigate", "s", 0, -1)]
        [TestCase("UI", "Navigate", "a", -1, 0)]
        [TestCase("UI", "Navigate", "d", 1, 0)]
        [TestCase("UI", "Navigate", "upArrow", 0, 1)]
        [TestCase("UI", "Navigate", "downArrow", 0, -1)]
        [TestCase("UI", "Navigate", "leftArrow", -1, 0)]
        [TestCase("UI", "Navigate", "rightArrow", 1, 0)]
        public void KeyboardCompositeDirectionsMatch(string mapName, string actionName, string key, int x, int y)
        {
            using (var actions = new GameInputActions())
            {
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var action = actions.asset.FindActionMap(mapName, true).FindAction(actionName, true);
                action.actionMap.Enable();
                try
                {
                    var control = keyboard.GetChildControl<ButtonControl>(key);
                    Press(control);
                    Assert.That(action.ReadValue<Vector2>(), Is.EqualTo(new Vector2(x, y)));
                    Release(control);
                    Assert.That(action.ReadValue<Vector2>(), Is.EqualTo(Vector2.zero));
                }
                finally { actions.Disable(); }
            }
        }

        private static Dictionary<string, int> Subscribe(GameInputActions.GameplayActions map)
        {
            var counts = GameplayActionNames.ToDictionary(name => name, _ => 0, StringComparer.Ordinal);
            foreach (var action in map.Get().actions)
            {
                var captured = action;
                captured.performed += _ => counts[captured.name]++;
            }
            return counts;
        }

        private static Dictionary<string, int> Subscribe(GameInputActions.UIActions map)
        {
            var counts = UiActionNames.ToDictionary(name => name, _ => 0, StringComparer.Ordinal);
            foreach (var action in map.Get().actions)
            {
                var captured = action;
                captured.performed += _ => counts[captured.name]++;
            }
            return counts;
        }

        private static InputActionAsset LoadSourceAsset() => InputActionAsset.FromJson(File.ReadAllText(AssetPath));

        private static void AssertBinding(InputActionMap map, string actionName, string path, string group)
        {
            var action = map.FindAction(actionName, true);
            Assert.That(action.bindings.Any(binding => binding.path == path && ContainsGroup(binding.groups, group)), Is.True,
                $"Missing {map.name}/{actionName} binding {path} [{group}]");
        }

        private static bool ContainsGroup(string groups, string expected) =>
            (groups ?? string.Empty).Split(';').Any(group => string.Equals(group, expected, StringComparison.Ordinal));

        private static Type FindLoadedType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullName, false);
                if (type != null) return type;
            }

            foreach (var assemblyName in new[] { "Unity.InputSystem.Editor", "Unity.InputSystem" })
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    var type = assembly.GetType(fullName, false);
                    if (type != null) return type;
                }
                catch (FileNotFoundException) { }
                catch (FileLoadException) { }
            }

            return null;
        }

        private static string NormalizeNewlines(string value) =>
            value.Replace("\r\n", "\n").Replace("\r", "\n");
    }
}
