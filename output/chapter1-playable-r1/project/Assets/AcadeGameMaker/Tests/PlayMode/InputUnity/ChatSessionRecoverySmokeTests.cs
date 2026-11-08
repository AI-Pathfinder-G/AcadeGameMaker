using System;
using System.Collections;
using AcadeGameMaker.Input.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // User-authorized focused Unity smoke. Not the Luna gate or natural boss acceptance.
    [TestFixture]
    internal sealed class ChatSessionRecoverySmokeTests
    {
        private OwnedBatchInputIsolation _isolation;
        private LocalContext _context;
        private Keyboard _virtualKeyboard;
        private Mouse _virtualMouse;
        private int _originalDeviceCount;

        private sealed class LocalContext : IChapter1OwnedBatchInputContext
        {
            private bool _alive = true;
            private bool _isolated;
            private readonly Chapter1OwnedBatchInputEvidence _identity;
            public Chapter1VerificationBossBudget Budget { get; } =
                new Chapter1VerificationBossBudget(65536, true);

            public LocalContext()
            {
                using (var p = System.Diagnostics.Process.GetCurrentProcess())
                    _identity = new Chapter1OwnedBatchInputEvidence
                    {
                        Epoch = 1,
                        CaseName = "ChatSessionRecoverySmokeTests.RestoresAfterGeneralBudgetExhaustion",
                        RunId = "user-approved-chat-session-recovery",
                        Phase = "local-unity-recovery-smoke",
                        SourceSha = "local-chat-session-not-independent-review",
                        OwnerPid = p.Id,
                        OwnerBirth = p.StartTime.ToFileTimeUtc().ToString()
                    };
            }

            public void SetIsolation(bool active) { _isolated = active; }
            public bool TryObserve(out Chapter1OwnedBatchInputEvidence evidence)
            {
                evidence = _identity;
                evidence.IsolatedApi = _isolated;
                return _alive;
            }
            public bool TryReadIsolatedTime(out double value)
            {
                value = 0;
                return false;
            }
            public void Dispose() { _alive = false; }
        }

        [UnitySetUp]
        public IEnumerator Enter()
        {
            Assert.That(Application.isEditor && Application.isBatchMode, Is.True);
            yield return new EnterPlayMode();
        }

        [UnityTest]
        public IEnumerator RestoresAfterGeneralBudgetExhaustion()
        {
            Assert.That(Application.isPlaying, Is.True);
            _originalDeviceCount = InputSystem.devices.Count;
            // Test-owned virtual devices provide nonempty device/control baselines.
            // No native keyboard/mouse or persistent InputSettings asset is changed.
            _virtualKeyboard = InputSystem.AddDevice<Keyboard>();
            _virtualMouse = InputSystem.AddDevice<Mouse>();
            Assert.That(InputSystem.devices.Count, Is.EqualTo(_originalDeviceCount + 2));

            _context = new LocalContext();
            _isolation = new OwnedBatchInputIsolation(
                _context, _context.SetIsolation, _ => { });

            var added = 0;
            var denied = false;
            for (var i = 0; i < 256; ++i)
            {
                try
                {
                    _context.Budget.Add("normal-test-" + i + ":" + new string('x', 480));
                    ++added;
                }
                catch (InvalidOperationException)
                {
                    denied = true;
                    break;
                }
            }
            Assert.That(denied, Is.True, "General evidence must fail closed");
            Assert.That(added, Is.GreaterThan(0));

            // Actual InputTestFixture teardown performs settings, devices and
            // control-value comparisons before writing the two reserved records.
            _isolation.Restore();
            var confirm = _isolation.ConfirmRestored();
            try
            {
                while (confirm.MoveNext()) yield return confirm.Current;
            }
            finally
            {
                (confirm as IDisposable)?.Dispose();
            }

            Assert.That(_isolation.Restored, Is.True);
            Assert.That(_isolation.Error, Is.Null);
            Assert.That(InputSystem.GetDeviceById(_virtualKeyboard.deviceId), Is.SameAs(_virtualKeyboard));
            Assert.That(InputSystem.GetDeviceById(_virtualMouse.deviceId), Is.SameAs(_virtualMouse));
            Assert.That(_virtualKeyboard.added && _virtualMouse.added, Is.True);

            _context.Budget.AddCleanup("case-witness terminal-teardown from session test");
            Assert.Throws<InvalidOperationException>(
                () => _context.Budget.AddCleanup("fourth cleanup row must be rejected"));

            InputSystem.RemoveDevice(_virtualMouse);
            InputSystem.RemoveDevice(_virtualKeyboard);
            _virtualMouse = null;
            _virtualKeyboard = null;
            Assert.That(InputSystem.devices.Count, Is.EqualTo(_originalDeviceCount));

            Debug.Log("[CHAT_RECOVERY_SMOKE] real Unity InputTestFixture restoration PASS;"
                + " normalEvidenceRows=" + added
                + " baselineVirtualDevices=2 reservedCleanupRows=3;"
                + " fourthCleanupDenied=true independentReview=false bossCompletion=false");
        }

        [UnityTearDown]
        public IEnumerator Exit()
        {
            if (_isolation != null && !_isolation.Restored)
            {
                try { _isolation.Restore(); }
                catch (Exception error)
                {
                    Debug.LogWarning("[CHAT_RECOVERY_SMOKE] best-effort teardown failed: "
                        + error.GetType().Name);
                }
            }
            if (_virtualMouse != null && _virtualMouse.added)
                InputSystem.RemoveDevice(_virtualMouse);
            if (_virtualKeyboard != null && _virtualKeyboard.added)
                InputSystem.RemoveDevice(_virtualKeyboard);
            _context?.Dispose();
            if (Application.isPlaying) yield return new ExitPlayMode();
        }
    }
}