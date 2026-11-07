using System;
using System.Runtime.CompilerServices;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7PA-001. This read-only source deliberately grants no map,
    // callback, reset, dequeue, controller, notification, or menu authority.
    internal interface IUiSemanticFrameSourceV1 : IInputFrameCommitSource
    {
        UiSemanticFrameV1? CurrentUiFrame { get; }
    }

    // REQ-M5D7PA-002/006. Immutable, receipt-bound facts captured by the one
    // InputRouter callback owner.  Output pixels are signed and unclamped.
    internal readonly struct UiSemanticFrameV1 : IEquatable<UiSemanticFrameV1>
    {
        private readonly InputFrameCommitReceipt _receipt;
        private readonly int _navigateXQ4096, _navigateYQ4096;
        private readonly bool _navigateChanged, _hasPoint, _pointChanged;
        private readonly int _pointXActualPx, _pointYActualPx;
        private readonly bool _clickPressed, _hasClickPoint;
        private readonly int _clickPointXActualPx, _clickPointYActualPx;
        private readonly long _scrollXQ4096, _scrollYQ4096;
        private readonly bool _submitPressed, _cancelPressed;
        // REQ-M5D7PA-006. A value-only proof detects reflection mutation of
        // every frame backing without consulting the router publication.
        private readonly FrameProof _proof;

        internal UiSemanticFrameV1(InputFrameCommitReceipt receipt, int navigateXQ4096, int navigateYQ4096,
            bool navigateChanged, bool hasPoint, int pointXActualPx, int pointYActualPx, bool pointChanged,
            bool clickPressed, bool hasClickPoint, int clickPointXActualPx, int clickPointYActualPx,
            long scrollXQ4096, long scrollYQ4096, bool submitPressed, bool cancelPressed)
        {
            _receipt = receipt; _navigateXQ4096 = navigateXQ4096; _navigateYQ4096 = navigateYQ4096;
            _navigateChanged = navigateChanged; _hasPoint = hasPoint; _pointXActualPx = pointXActualPx; _pointYActualPx = pointYActualPx; _pointChanged = pointChanged;
            _clickPressed = clickPressed; _hasClickPoint = hasClickPoint; _clickPointXActualPx = clickPointXActualPx; _clickPointYActualPx = clickPointYActualPx;
            _scrollXQ4096 = scrollXQ4096; _scrollYQ4096 = scrollYQ4096; _submitPressed = submitPressed; _cancelPressed = cancelPressed;
            _proof = new FrameProof(receipt, navigateXQ4096, navigateYQ4096, navigateChanged, hasPoint, pointXActualPx, pointYActualPx, pointChanged,
                clickPressed, hasClickPoint, clickPointXActualPx, clickPointYActualPx, scrollXQ4096, scrollYQ4096, submitPressed, cancelPressed);
            Validate();
        }

        internal InputFrameCommitReceipt Receipt { get { Validate(); return _receipt; } }
        internal int NavigateXQ4096 { get { Validate(); return _navigateXQ4096; } }
        internal int NavigateYQ4096 { get { Validate(); return _navigateYQ4096; } }
        internal bool NavigateChanged { get { Validate(); return _navigateChanged; } }
        internal bool HasPoint { get { Validate(); return _hasPoint; } }
        internal int PointXActualPx { get { Validate(); return _pointXActualPx; } }
        internal int PointYActualPx { get { Validate(); return _pointYActualPx; } }
        internal bool PointChanged { get { Validate(); return _pointChanged; } }
        internal bool ClickPressed { get { Validate(); return _clickPressed; } }
        internal bool HasClickPoint { get { Validate(); return _hasClickPoint; } }
        internal int ClickPointXActualPx { get { Validate(); return _clickPointXActualPx; } }
        internal int ClickPointYActualPx { get { Validate(); return _clickPointYActualPx; } }
        internal long ScrollXQ4096 { get { Validate(); return _scrollXQ4096; } }
        internal long ScrollYQ4096 { get { Validate(); return _scrollYQ4096; } }
        internal bool SubmitPressed { get { Validate(); return _submitPressed; } }
        internal bool CancelPressed { get { Validate(); return _cancelPressed; } }

        internal static UiSemanticFrameV1 Empty(InputFrameCommitReceipt receipt) =>
            new UiSemanticFrameV1(receipt, 0, 0, false, false, 0, 0, false, false, false, 0, 0, 0, 0, false, false);

        internal void Validate()
        {
            var canonical = new InputFrameCommitReceipt(_receipt.Tick, _receipt.FrameOrdinal, _receipt.ModeEpoch, _receipt.Mode);
            if (!canonical.Equals(_receipt) || _receipt.Mode != InputMode.UIOnly) throw new InvalidOperationException("UI frame receipt is malformed.");
            if (_navigateXQ4096 < -4096 || _navigateXQ4096 > 4096 || _navigateYQ4096 < -4096 || _navigateYQ4096 > 4096)
                throw new InvalidOperationException("UI navigate value is malformed.");
            if (_clickPressed != _hasClickPoint) throw new InvalidOperationException("UI click point presence is malformed.");
            if (!_hasPoint && (_pointXActualPx != 0 || _pointYActualPx != 0 || _pointChanged)) throw new InvalidOperationException("UI absent point is malformed.");
            if (!_hasClickPoint && (_clickPointXActualPx != 0 || _clickPointYActualPx != 0)) throw new InvalidOperationException("UI absent click point is malformed.");
            if (!_proof.Matches(_receipt, _navigateXQ4096, _navigateYQ4096, _navigateChanged, _hasPoint, _pointXActualPx, _pointYActualPx, _pointChanged,
                _clickPressed, _hasClickPoint, _clickPointXActualPx, _clickPointYActualPx, _scrollXQ4096, _scrollYQ4096, _submitPressed, _cancelPressed))
                throw new InvalidOperationException("UI frame proof is malformed.");
        }

        public bool Equals(UiSemanticFrameV1 other) =>
            _receipt.Equals(other._receipt) && _navigateXQ4096 == other._navigateXQ4096 && _navigateYQ4096 == other._navigateYQ4096 &&
            _navigateChanged == other._navigateChanged && _hasPoint == other._hasPoint && _pointXActualPx == other._pointXActualPx && _pointYActualPx == other._pointYActualPx &&
            _pointChanged == other._pointChanged && _clickPressed == other._clickPressed && _hasClickPoint == other._hasClickPoint &&
            _clickPointXActualPx == other._clickPointXActualPx && _clickPointYActualPx == other._clickPointYActualPx &&
            _scrollXQ4096 == other._scrollXQ4096 && _scrollYQ4096 == other._scrollYQ4096 && _submitPressed == other._submitPressed && _cancelPressed == other._cancelPressed;
        public override bool Equals(object obj) => obj is UiSemanticFrameV1 other && Equals(other);
        public override int GetHashCode() => _receipt.GetHashCode();

        private readonly struct FrameProof
        {
            private readonly InputFrameCommitReceipt _receipt; private readonly int _nx, _ny, _px, _py, _cx, _cy;
            private readonly bool _nc, _hp, _pc, _click, _hcp, _submit, _cancel;
            private readonly long _sx, _sy;
            internal FrameProof(InputFrameCommitReceipt receipt, int nx, int ny, bool nc, bool hp, int px, int py, bool pc, bool click, bool hcp, int cx, int cy, long sx, long sy, bool submit, bool cancel)
            { _receipt = receipt; _nx = nx; _ny = ny; _nc = nc; _hp = hp; _px = px; _py = py; _pc = pc; _click = click; _hcp = hcp; _cx = cx; _cy = cy; _sx = sx; _sy = sy; _submit = submit; _cancel = cancel; }
            internal bool Matches(InputFrameCommitReceipt receipt, int nx, int ny, bool nc, bool hp, int px, int py, bool pc, bool click, bool hcp, int cx, int cy, long sx, long sy, bool submit, bool cancel) =>
                _receipt.Equals(receipt) && _nx == nx && _ny == ny && _nc == nc && _hp == hp && _px == px && _py == py && _pc == pc && _click == click && _hcp == hcp && _cx == cx && _cy == cy && _sx == sx && _sy == sy && _submit == submit && _cancel == cancel;
        }
    }

    internal enum UiSemanticFrameCursorStateV1 { AwaitingBaseline = 1, Ready = 2, Closed = 3, Failed = 4 }

    // REQ-M5D7PA-005/006. An engine-free, identity-bound reader. It uses no
    // queue because a skipped fixed step must fail rather than replay input.
    internal sealed class UiSemanticFrameCursorV1
    {
        // REQ-M5D7PA-006. A terminal cursor fault is deliberately held outside
        // the reflected value/proof row.  Rewriting that row to "Failed" would
        // erase the forensic value which caused the boundary to close.
        private static readonly ConditionalWeakTable<UiSemanticFrameCursorV1, CursorFaultBox> TerminalFaults =
            new ConditionalWeakTable<UiSemanticFrameCursorV1, CursorFaultBox>();
        private readonly IUiSemanticFrameSourceV1 _source;
        private readonly IUiSemanticFrameSourceV1 _sourceProof;
        private UiSemanticFrameCursorStateV1 _state;
        private UiSemanticFrameCursorStateV1 _stateProof;
        private InputFrameCommitReceipt? _baseline;
        private InputFrameCommitReceipt? _baselineProof;

        private UiSemanticFrameCursorV1(IUiSemanticFrameSourceV1 source, UiSemanticFrameCursorStateV1 state,
            InputFrameCommitReceipt? baseline)
        {
            _source = _sourceProof = source; _state = _stateProof = state; _baseline = baseline; _baselineProof = baseline;
            ValidateSelf();
        }

        internal static UiSemanticFrameCursorV1 Create(IUiSemanticFrameSourceV1 source)
        {
            if (source == null || source.IsFaulted) throw new InvalidOperationException("UI semantic source is unavailable.");
            var pair = ReadPair(source);
            if (!pair.receipt.HasValue) return new UiSemanticFrameCursorV1(source, UiSemanticFrameCursorStateV1.AwaitingBaseline, null);
            if (pair.receipt.Value.Mode == InputMode.UIOnly) return new UiSemanticFrameCursorV1(source, UiSemanticFrameCursorStateV1.Ready, pair.receipt.Value);
            return new UiSemanticFrameCursorV1(source, UiSemanticFrameCursorStateV1.Closed, null);
        }

        internal UiSemanticFrameCursorStateV1 State
        {
            get
            {
                if (HasTerminalFault()) return UiSemanticFrameCursorStateV1.Failed;
                try { ValidateSelf(); return _state; }
                catch { LatchTerminalFault(); throw; }
            }
        }

        internal bool TryAdvance(IUiSemanticFrameSourceV1 source, out UiSemanticFrameV1 frame)
        {
            frame = default(UiSemanticFrameV1);
            try
            {
                if (HasTerminalFault()) throw new InvalidOperationException("UI semantic cursor has failed.");
                ValidateSelf();
                if (!ReferenceEquals(source, _source) || source == null || source.IsFaulted) throw new InvalidOperationException("UI semantic cursor source is invalid.");
                if (_state == UiSemanticFrameCursorStateV1.Closed) throw new InvalidOperationException("UI semantic cursor is closed.");
                if (_state == UiSemanticFrameCursorStateV1.Failed) throw new InvalidOperationException("UI semantic cursor has failed.");
                var pair = ReadPair(source);
                if (_state == UiSemanticFrameCursorStateV1.AwaitingBaseline)
                {
                    if (!pair.receipt.HasValue) return false;
                    if (pair.receipt.Value.Mode != InputMode.UIOnly) { SetState(UiSemanticFrameCursorStateV1.Closed); return false; }
                    SetBaseline(pair.receipt.Value); SetState(UiSemanticFrameCursorStateV1.Ready); return false;
                }
                var baseline = _baseline.Value;
                if (!pair.receipt.HasValue) throw new InvalidOperationException("UI semantic publication disappeared.");
                var receipt = pair.receipt.Value;
                if (receipt.Equals(baseline)) return false;
                var nextTick = checked(baseline.Tick.Value + 1);
                var nextOrdinal = checked(baseline.FrameOrdinal + 1);
                if (receipt.Tick.Value != nextTick || receipt.FrameOrdinal != nextOrdinal) throw new InvalidOperationException("UI semantic receipt is not consecutive.");
                if (receipt.Mode != InputMode.UIOnly)
                {
                    if (receipt.ModeEpoch != checked(baseline.ModeEpoch + 1) || pair.frame.HasValue) throw new InvalidOperationException("UI semantic exit is malformed.");
                    SetBaseline(receipt); SetState(UiSemanticFrameCursorStateV1.Closed); return false;
                }
                if (receipt.ModeEpoch != baseline.ModeEpoch || !pair.frame.HasValue || !pair.frame.Value.Receipt.Equals(receipt)) throw new InvalidOperationException("UI semantic successor is malformed.");
                frame = pair.frame.Value; SetBaseline(receipt); return true;
            }
            catch
            {
                LatchTerminalFault();
                throw;
            }
        }

        private static (InputFrameCommitReceipt? receipt, UiSemanticFrameV1? frame) ReadPair(IUiSemanticFrameSourceV1 source)
        {
            var receipt = source.CurrentReceipt; var frame = source.CurrentUiFrame;
            if (!receipt.HasValue)
            {
                if (frame.HasValue) throw new InvalidOperationException("UI frame exists without receipt.");
                return (null, null);
            }
            var canonical = new InputFrameCommitReceipt(receipt.Value.Tick, receipt.Value.FrameOrdinal, receipt.Value.ModeEpoch, receipt.Value.Mode);
            if (!canonical.Equals(receipt.Value)) throw new InvalidOperationException("UI source receipt is malformed.");
            if (receipt.Value.Mode == InputMode.UIOnly)
            {
                if (!frame.HasValue) throw new InvalidOperationException("UI source is missing semantic frame.");
                frame.Value.Validate(); if (!frame.Value.Receipt.Equals(receipt.Value)) throw new InvalidOperationException("UI frame receipt mismatch.");
            }
            else if (frame.HasValue) throw new InvalidOperationException("Non-UI source exposed a semantic frame.");
            return (receipt, frame);
        }

        private void SetBaseline(InputFrameCommitReceipt receipt) { _baseline = receipt; _baselineProof = receipt; }
        private void SetState(UiSemanticFrameCursorStateV1 state) { _state = _stateProof = state; }
        private bool HasTerminalFault()
        {
            if (!TerminalFaults.TryGetValue(this, out var fault)) return false;
            // A mutated latch is itself terminal.  It can never become a reset
            // path for a cursor whose identity has already failed.
            if (!fault.IsIntactFor(this)) return true;
            return true;
        }

        private void LatchTerminalFault()
        {
            TerminalFaults.GetValue(this, owner => new CursorFaultBox(owner));
        }

        private void ValidateSelf()
        {
            if (_state < UiSemanticFrameCursorStateV1.AwaitingBaseline || _state > UiSemanticFrameCursorStateV1.Closed) throw new InvalidOperationException("UI cursor is malformed.");
            if (_source == null || _sourceProof == null || !ReferenceEquals(_source, _sourceProof)) throw new InvalidOperationException("UI cursor is malformed.");
            if (_state != _stateProof) throw new InvalidOperationException("UI cursor state proof is malformed.");
            if (_baseline.HasValue != _baselineProof.HasValue) throw new InvalidOperationException("UI cursor baseline proof is malformed.");
            if (_baseline.HasValue)
            {
                var b = _baseline.Value; var p = _baselineProof.Value;
                var canonical = new InputFrameCommitReceipt(b.Tick, b.FrameOrdinal, b.ModeEpoch, b.Mode);
                if (!canonical.Equals(b) || !b.Equals(p)) throw new InvalidOperationException("UI cursor baseline is malformed.");
            }
            if (_state == UiSemanticFrameCursorStateV1.Ready && (!_baseline.HasValue || _baseline.Value.Mode != InputMode.UIOnly)) throw new InvalidOperationException("Ready cursor lacks UI baseline.");
            if (_state == UiSemanticFrameCursorStateV1.AwaitingBaseline && _baseline.HasValue) throw new InvalidOperationException("Awaiting cursor has a baseline.");
        }

        private sealed class CursorFaultBox
        {
            private readonly UiSemanticFrameCursorV1 _owner;
            private readonly object _token;
            private readonly object _tokenProof;

            internal CursorFaultBox(UiSemanticFrameCursorV1 owner)
            {
                _owner = owner;
                _token = new object();
                _tokenProof = _token;
            }

            internal bool IsIntactFor(UiSemanticFrameCursorV1 owner) =>
                ReferenceEquals(_owner, owner) && _token != null && ReferenceEquals(_token, _tokenProof);
        }
    }
}
