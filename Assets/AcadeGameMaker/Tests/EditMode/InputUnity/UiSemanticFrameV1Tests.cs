using System;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // REQ-M5D7PA-001/002/005/006; AC-M5D7PA-001/006/007.
    internal sealed class UiSemanticFrameV1Tests
    {
        [Test]
        public void AC006_BaselineDuplicateConsecutiveAndCleanExitAreExact()
        {
            var source = new Source(); var cursor = UiSemanticFrameCursorV1.Create(source); UiSemanticFrameV1 frame;
            Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.AwaitingBaseline));
            Assert.That(cursor.TryAdvance(source, out frame), Is.False);
            source.Publish(Frame(0, 0, 0)); Assert.That(cursor.TryAdvance(source, out frame), Is.False);
            Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Ready)); Assert.That(cursor.TryAdvance(source, out frame), Is.False);
            source.Publish(Frame(1, 1, 0)); Assert.That(cursor.TryAdvance(source, out frame), Is.True); Assert.That(frame.Receipt.FrameOrdinal, Is.EqualTo(1));
            source.PublishNonUi(new InputFrameCommitReceipt(new SimulationTick(2), 2, 1, InputMode.Transition));
            Assert.That(cursor.TryAdvance(source, out frame), Is.False); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Closed));
            Assert.Throws<InvalidOperationException>(() => cursor.TryAdvance(source, out frame)); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));
        }

        [Test]
        public void AC006_ForeignSkipAndFirstNonUiFailOrCloseExactly()
        {
            var source = new Source(); source.Publish(Frame(0, 0, 0)); var cursor = UiSemanticFrameCursorV1.Create(source); var foreign = new Source(); foreign.Publish(Frame(0, 0, 0)); UiSemanticFrameV1 ignored;
            Assert.Throws<InvalidOperationException>(() => cursor.TryAdvance(foreign, out ignored)); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));
            source = new Source(); cursor = UiSemanticFrameCursorV1.Create(source); source.PublishNonUi(new InputFrameCommitReceipt(new SimulationTick(0), 0, 0, InputMode.Transition));
            Assert.That(cursor.TryAdvance(source, out ignored), Is.False); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Closed));
            source = new Source(); source.Publish(Frame(0, 0, 0)); cursor = UiSemanticFrameCursorV1.Create(source); source.Publish(Frame(2, 2, 0));
            Assert.Throws<InvalidOperationException>(() => cursor.TryAdvance(source, out ignored)); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));
        }

        [Test]
        public void AC006_ReplayEpochJumpSourceFaultAndSuccessorOverflowFailBeforeDelivery()
        {
            UiSemanticFrameV1 ignored;

            var source = new Source(); source.Publish(Frame(1, 1, 0)); var cursor = UiSemanticFrameCursorV1.Create(source);
            source.Publish(Frame(1, 0, 0));
            Assert.Throws<InvalidOperationException>(() => cursor.TryAdvance(source, out ignored), "replayed ordinal");
            Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));

            source = new Source(); source.Publish(Frame(0, 0, 0)); cursor = UiSemanticFrameCursorV1.Create(source);
            source.Publish(Frame(1, 1, 2));
            Assert.Throws<InvalidOperationException>(() => cursor.TryAdvance(source, out ignored), "unexplained epoch jump");
            Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));

            source = new Source(); source.Publish(Frame(0, 0, 0)); cursor = UiSemanticFrameCursorV1.Create(source); source.IsFaulted = true;
            Assert.Throws<InvalidOperationException>(() => cursor.TryAdvance(source, out ignored), "faulted source");
            Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));

            source = new Source(); source.Publish(Frame(int.MaxValue, 0, 0)); cursor = UiSemanticFrameCursorV1.Create(source);
            source.Publish(Frame(int.MaxValue, 1, 0));
            Assert.Throws<OverflowException>(() => cursor.TryAdvance(source, out ignored), "tick successor overflow");
            Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));
        }

        [Test]
        public void AC006_AC007_MalformedPairsAndEveryCheckedSuccessorAxisFailBeforeDelivery()
        {
            UiSemanticFrameV1 ignored;
            var source = new Source { Receipt = new InputFrameCommitReceipt(new SimulationTick(0), 0, 0, InputMode.UIOnly) };
            Assert.Throws<InvalidOperationException>(() => UiSemanticFrameCursorV1.Create(source), "UI receipt missing frame");
            source = new Source { Receipt = new InputFrameCommitReceipt(new SimulationTick(0), 0, 0, InputMode.Transition), Frame = Frame(0, 0, 0) };
            Assert.Throws<InvalidOperationException>(() => UiSemanticFrameCursorV1.Create(source), "non-UI receipt with frame");
            source = new Source { Receipt = new InputFrameCommitReceipt(new SimulationTick(0), 0, 0, InputMode.UIOnly), Frame = Frame(1, 1, 0) };
            Assert.Throws<InvalidOperationException>(() => UiSemanticFrameCursorV1.Create(source), "UI frame receipt mismatch");
            source = new Source(); source.Publish(Frame(int.MaxValue, 0, 0)); var cursor = UiSemanticFrameCursorV1.Create(source); source.Publish(Frame(int.MaxValue, 1, 0));
            Assert.Throws<OverflowException>(() => cursor.TryAdvance(source, out ignored), "tick successor"); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));
            source = new Source(); source.Publish(Frame(0, long.MaxValue, 0)); cursor = UiSemanticFrameCursorV1.Create(source); source.Publish(Frame(1, long.MaxValue, 0));
            Assert.Throws<OverflowException>(() => cursor.TryAdvance(source, out ignored), "ordinal successor"); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));
            source = new Source(); source.Publish(Frame(0, 0, long.MaxValue)); cursor = UiSemanticFrameCursorV1.Create(source); source.PublishNonUi(new InputFrameCommitReceipt(new SimulationTick(1), 1, long.MaxValue, InputMode.Transition));
            Assert.Throws<OverflowException>(() => cursor.TryAdvance(source, out ignored), "exit epoch successor"); Assert.That(cursor.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed));
        }

        [Test]
        public void AC007_FrameAndCursorProofBackingsFailClosed()
        {
            var baseline = Frame(0, 0, 0);
            foreach (var field in typeof(UiSemanticFrameV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                object boxed = baseline; field.SetValue(boxed, Corrupt(field.FieldType, field.GetValue(boxed)));
                Assert.Throws<InvalidOperationException>(() => ((UiSemanticFrameV1)boxed).Validate(), field.Name);
            }
            var source = new Source(); source.Publish(baseline); var cursor = UiSemanticFrameCursorV1.Create(source); UiSemanticFrameV1 ignored;
            foreach (var field in typeof(UiSemanticFrameCursorV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var clone = Clone(cursor);
                var corrupt = Corrupt(field.FieldType, field.GetValue(clone));
                field.SetValue(clone, corrupt);
                Assert.Throws<InvalidOperationException>(() => clone.TryAdvance(source, out ignored), field.Name);
                Assert.That(clone.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed), field.Name);
                // REQ-M5D7PA-006: terminal observation must not overwrite the
                // exact reflected raw backing that caused the failure.
                Assert.That(field.GetValue(clone), Is.EqualTo(corrupt), field.Name + " forensic backing");
            }

            foreach (var latchField in new[] { "_owner", "_token", "_tokenProof" })
            {
                var failed = Clone(cursor);
                var sourceField = typeof(UiSemanticFrameCursorV1).GetField("_source", BindingFlags.Instance | BindingFlags.NonPublic);
                sourceField.SetValue(failed, null);
                Assert.Throws<InvalidOperationException>(() => failed.TryAdvance(source, out ignored));
                MutateFaultLatch(failed, latchField);
                // The external latch remains identity-terminal even when its
                // own individual evidence is reflected. The corrupt cursor
                // row is not normalized or made usable by a source swap.
                sourceField.SetValue(failed, new Source());
                Assert.That(sourceField.GetValue(failed), Is.Not.SameAs(source), latchField + " source replacement recorded");
                Assert.That(failed.State, Is.EqualTo(UiSemanticFrameCursorStateV1.Failed), latchField);
                Assert.Throws<InvalidOperationException>(() => failed.TryAdvance(source, out ignored), latchField + " cannot reopen cursor");
            }
        }

        private static UiSemanticFrameCursorV1 Clone(UiSemanticFrameCursorV1 value) => (UiSemanticFrameCursorV1)typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(value, null);
        private static UiSemanticFrameV1 Frame(long tick, long ordinal, long epoch) => new UiSemanticFrameV1(new InputFrameCommitReceipt(new SimulationTick(checked((int)tick)), ordinal, epoch, InputMode.UIOnly), -4096, 4096, true, true, -17, 2049, true, true, true, -17, 2049, -8192, 12288, true, true);
        private static object Corrupt(Type type, object value)
        {
            if (type == typeof(bool)) return !(bool)value; if (type == typeof(int)) return ((int)value) + 1; if (type == typeof(long)) return long.MaxValue;
            if (type == typeof(InputFrameCommitReceipt?)) return null; if (type == typeof(InputFrameCommitReceipt)) return default(InputFrameCommitReceipt);
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        private static void MutateFaultLatch(UiSemanticFrameCursorV1 cursor, string fieldName)
        {
            var table = typeof(UiSemanticFrameCursorV1).GetField("TerminalFaults", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var tryGet = table.GetType().GetMethod("TryGetValue");
            var args = new object[] { cursor, null };
            Assert.That((bool)tryGet.Invoke(table, args), Is.True);
            var latch = args[1];
            typeof(UiSemanticFrameCursorV1).GetNestedType("CursorFaultBox", BindingFlags.NonPublic)
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(latch, null);
        }
        private sealed class Source : IUiSemanticFrameSourceV1
        {
            internal InputFrameCommitReceipt? Receipt; internal UiSemanticFrameV1? Frame;
            internal void Publish(UiSemanticFrameV1 frame) { Frame = frame; Receipt = frame.Receipt; }
            internal void PublishNonUi(InputFrameCommitReceipt receipt) { Frame = null; Receipt = receipt; }
            public InputFrameCommitReceipt? CurrentReceipt => Receipt; public UiSemanticFrameV1? CurrentUiFrame => Frame; public bool IsFaulted { get; set; }
        }
    }
}
