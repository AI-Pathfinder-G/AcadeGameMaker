using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AcadeGameMaker.Profile.Tests
{
    // Test-only hostile NTFS state fixture authorized by C2. It neither calls
    // production code nor changes its algorithm; every path is under _base.
    internal sealed class ProfileResetMemoryJunctionFixtureV1 : IDisposable
    {
        private const uint GenericWrite = 0x40000000, OpenExisting = 3, FileFlagBackupSemantics = 0x02000000, FileFlagOpenReparsePoint = 0x00200000;
        private const uint FsctlSetReparsePoint = 0x000900A4, FsctlGetReparsePoint = 0x000900A8, IoReparseTagMountPoint = 0xA0000003;
        private readonly string _base, _root, _target; private readonly byte[] _sentinel = { 0x43, 0x32, 0x4A, 0x58 };
        internal string Root { get { return _root; } }
        internal string Target { get { return _target; } }
        internal ProfileResetMemoryJunctionFixtureV1()
        {
            _base = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C2-junction-" + Guid.NewGuid().ToString("N"));
            _root = Path.Combine(_base, "root"); _target = Path.Combine(_base, "target");
            Directory.CreateDirectory(_root); Directory.CreateDirectory(_target); File.WriteAllBytes(Path.Combine(_target, "sentinel.bin"), _sentinel);
            VerifyOwnedBase(); VerifyOwned(_root); VerifyOwned(_target);
        }
        internal void CreateJunction(string link, string target)
        {
            VerifyOwned(link); VerifyOwned(target); if (Directory.Exists(link) || File.Exists(link)) throw new InvalidOperationException("Junction link must be absent.");
            Directory.CreateDirectory(link);
            var substitute = "\\??\\" + Path.GetFullPath(target).TrimEnd(Path.DirectorySeparatorChar);
            var print = Path.GetFullPath(target).TrimEnd(Path.DirectorySeparatorChar);
            var substituteBytes = System.Text.Encoding.Unicode.GetBytes(substitute + "\0"); var printBytes = System.Text.Encoding.Unicode.GetBytes(print + "\0");
            // REPARSE_DATA_BUFFER has the 8-byte generic header plus the
            // 8-byte mount-point fixed header before PathBuffer at offset 16.
            var size = 16 + substituteBytes.Length + printBytes.Length; var data = new byte[size];
            if (substituteBytes.Length < 2 || printBytes.Length < 2 || substituteBytes.Length > ushort.MaxValue || printBytes.Length > ushort.MaxValue || size - 8 > ushort.MaxValue) throw new IOException("Junction reparse buffer exceeds USHORT bounds.");
            WriteUInt32(data, 0, IoReparseTagMountPoint); WriteUInt16(data, 4, (ushort)(size - 8));
            WriteUInt16(data, 8, 0); WriteUInt16(data, 10, (ushort)(substituteBytes.Length - 2)); WriteUInt16(data, 12, (ushort)substituteBytes.Length); WriteUInt16(data, 14, (ushort)(printBytes.Length - 2));
            Buffer.BlockCopy(substituteBytes, 0, data, 16, substituteBytes.Length); Buffer.BlockCopy(printBytes, 0, data, 16 + substituteBytes.Length, printBytes.Length);
            var handle = CreateFile(link, GenericWrite, 0, IntPtr.Zero, OpenExisting, FileFlagBackupSemantics | FileFlagOpenReparsePoint, IntPtr.Zero);
            if (handle == new IntPtr(-1)) throw new IOException("CreateFile for junction failed: " + Marshal.GetLastWin32Error());
            try { uint ignored; if (!DeviceIoControl(handle, FsctlSetReparsePoint, data, (uint)data.Length, null, 0, out ignored, IntPtr.Zero)) throw new IOException("DeviceIoControl set junction failed: " + Marshal.GetLastWin32Error()); }
            finally { CloseHandle(handle); }
            AssertJunction(link); AssertSentinel();
        }
        internal void AssertSentinel()
        {
            VerifyOwned(_target); var bytes = File.ReadAllBytes(Path.Combine(_target, "sentinel.bin"));
            if (bytes.Length != _sentinel.Length) throw new InvalidOperationException("Junction target sentinel changed.");
            for (var i = 0; i < bytes.Length; i++) if (bytes[i] != _sentinel[i]) throw new InvalidOperationException("Junction target sentinel changed.");
            if (File.Exists(Path.Combine(_target, "profile.operation.lock")) || File.Exists(Path.Combine(_target, ProfileResetBarrierV1.BarrierName))) throw new InvalidOperationException("Foreign target was mutated.");
        }
        internal void AssertJunction(string link)
        {
            VerifyOwned(link); var handle = CreateFile(link, 0, 0, IntPtr.Zero, OpenExisting, FileFlagBackupSemantics | FileFlagOpenReparsePoint, IntPtr.Zero);
            if (handle == new IntPtr(-1)) throw new IOException("CreateFile for junction verify failed: " + Marshal.GetLastWin32Error());
            try { var data = new byte[16384]; uint read; if (!DeviceIoControl(handle, FsctlGetReparsePoint, null, 0, data, (uint)data.Length, out read, IntPtr.Zero) || read < 8 || ReadUInt32(data, 0) != IoReparseTagMountPoint) throw new IOException("Junction tag verification failed."); }
            finally { CloseHandle(handle); }
        }
        public void Dispose()
        {
            // Remove known links non-recursively before considering base cleanup.
            RemoveJunctions(_base); VerifyOwned(_base);
            if (Directory.Exists(_base)) Directory.Delete(_base, true);
        }
        private void RemoveJunctions(string directory)
        {
            VerifyOwned(directory);
            foreach (var child in Directory.GetDirectories(directory))
            {
                var attributes = File.GetAttributes(child);
                if ((attributes & FileAttributes.ReparsePoint) != 0) { AssertJunction(child); if (!RemoveDirectory(child)) throw new IOException("Junction removal failed: " + Marshal.GetLastWin32Error()); }
                else RemoveJunctions(child);
            }
        }
        private void VerifyOwned(string path)
        {
            VerifyOwnedBase();
            var bareBase = Path.GetFullPath(_base).TrimEnd(Path.DirectorySeparatorChar);
            var fullBase = bareBase + Path.DirectorySeparatorChar;
            var full = Path.GetFullPath(path);
            if (!string.Equals(full.TrimEnd(Path.DirectorySeparatorChar), bareBase, StringComparison.OrdinalIgnoreCase) && !full.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Fixture path escapes its owned base.");
            if (Directory.Exists(_base) && (File.GetAttributes(_base) & FileAttributes.ReparsePoint) != 0) throw new InvalidOperationException("Fixture base must not be a reparse point.");
        }
        private void VerifyOwnedBase()
        {
            var bareBase = Path.GetFullPath(_base).TrimEnd(Path.DirectorySeparatorChar);
            var temp = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar);
            var prefix = temp + Path.DirectorySeparatorChar;
            if (!bareBase.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Fixture base is not an OS-temp descendant.");
            if (!Directory.Exists(bareBase) || (File.GetAttributes(bareBase) & FileAttributes.ReparsePoint) != 0) throw new InvalidOperationException("Fixture base is unsafe.");
            var current = new DirectoryInfo(bareBase);
            while (current != null)
            {
                var attributes = File.GetAttributes(current.FullName);
                if ((attributes & FileAttributes.Directory) == 0 || (attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidOperationException("Fixture ancestor is unsafe.");
                if (string.Equals(current.FullName.TrimEnd(Path.DirectorySeparatorChar), temp, StringComparison.OrdinalIgnoreCase)) return;
                current = current.Parent;
            }
            throw new InvalidOperationException("Fixture base escaped OS temp ancestry.");
        }
        private static void WriteUInt16(byte[] b, int i, ushort v) { b[i] = (byte)v; b[i + 1] = (byte)(v >> 8); }
        private static void WriteUInt32(byte[] b, int i, uint v) { b[i] = (byte)v; b[i + 1] = (byte)(v >> 8); b[i + 2] = (byte)(v >> 16); b[i + 3] = (byte)(v >> 24); }
        private static uint ReadUInt32(byte[] b, int i) { return (uint)(b[i] | (b[i + 1] << 8) | (b[i + 2] << 16) | (b[i + 3] << 24)); }
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] private static extern IntPtr CreateFile(string name, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool DeviceIoControl(IntPtr device, uint code, byte[] input, uint inputSize, byte[] output, uint outputSize, out uint returned, IntPtr overlapped);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] private static extern bool RemoveDirectory(string path);
    }
}
