using System;
using System.Runtime.InteropServices;

namespace Satori.Client.Input;

internal static class SdlNative
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate nint GetKeyboardStateDelegate(out int numKeys);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int SetHintDelegate([MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void StopTextInputDelegate();

	private const int ScancodeA = 4;

	private const int ScancodeD = 7;

	private const int ScancodeE = 8;

	private const int ScancodeS = 22;

	private const int ScancodeW = 26;

	private const int ScancodeRight = 79;

	private const int ScancodeLeft = 80;

	private const int ScancodeDown = 81;

	private const int ScancodeUp = 82;

	private const int ScancodeKp4 = 88;

	private const int ScancodeKp2 = 90;

	private const int ScancodeKp8 = 92;

	private const int ScancodeKp6 = 94;

	private static readonly int[] UpScancodes = new int[4] { 26, 82, 92, 8 };

	private static readonly int[] DownScancodes = new int[3] { 22, 81, 90 };

	private static readonly int[] LeftScancodes = new int[3] { 4, 80, 88 };

	private static readonly int[] RightScancodes = new int[3] { 7, 79, 94 };

	private static nint _getKeyboardState;

	private static nint _setHint;

	private static nint _stopTextInput;

	private static bool _initialized;

	private static bool _available;

	public static void ConfigureForGameplay()
	{
		EnsureLoaded();
		if (_available)
		{
			SetHint("SDL_IME_SHOW_UI", "0");
			SetHint("SDL_IME_INTERNAL", "0");
			StopTextInput();
		}
	}

	public static bool IsUpDown()
	{
		return IsAnyScancodeDown(UpScancodes);
	}

	public static bool IsDownDown()
	{
		return IsAnyScancodeDown(DownScancodes);
	}

	public static bool IsLeftDown()
	{
		return IsAnyScancodeDown(LeftScancodes);
	}

	public static bool IsRightDown()
	{
		return IsAnyScancodeDown(RightScancodes);
	}

	private static bool IsAnyScancodeDown(int[] scancodes)
	{
		EnsureLoaded();
		if (!_available || _getKeyboardState == IntPtr.Zero)
		{
			return false;
		}
		int numKeys;
		nint keyboardState = GetKeyboardState(out numKeys);
		if (keyboardState == IntPtr.Zero)
		{
			return false;
		}
		foreach (int num in scancodes)
		{
			if (num >= 0 && Marshal.ReadByte(keyboardState, num) != 0)
			{
				return true;
			}
		}
		return false;
	}

	private static void EnsureLoaded()
	{
		if (!_initialized)
		{
			_initialized = true;
			if (TryLoadLibrary(out var library))
			{
				_getKeyboardState = NativeLibrary.GetExport(library, "SDL_GetKeyboardState");
				_setHint = NativeLibrary.GetExport(library, "SDL_SetHint");
				_stopTextInput = NativeLibrary.GetExport(library, "SDL_StopTextInput");
				_available = _getKeyboardState != IntPtr.Zero;
			}
		}
	}

	private static bool TryLoadLibrary(out nint library)
	{
		ReadOnlySpan<string> readOnlySpan = ((!OperatingSystem.IsWindows()) ? ((!OperatingSystem.IsMacOS()) ? new string[3] { "libSDL2-2.0.so.0", "libSDL2.so.0", "libSDL2-2.0.so" } : new string[2] { "libSDL2.dylib", "libSDL2-2.0.0.dylib" }) : new string[1] { "SDL2.dll" });
		ReadOnlySpan<string> readOnlySpan2 = readOnlySpan;
		for (int i = 0; i < readOnlySpan2.Length; i++)
		{
			string libraryPath = readOnlySpan2[i];
			if (NativeLibrary.TryLoad(libraryPath, out library))
			{
				return true;
			}
		}
		library = IntPtr.Zero;
		return false;
	}

	private static nint GetKeyboardState(out int numKeys)
	{
		numKeys = 0;
		if (_getKeyboardState == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		GetKeyboardStateDelegate delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer<GetKeyboardStateDelegate>(_getKeyboardState);
		return delegateForFunctionPointer(out numKeys);
	}

	private static void SetHint(string name, string value)
	{
		if (_setHint != IntPtr.Zero)
		{
			SetHintDelegate delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer<SetHintDelegate>(_setHint);
			delegateForFunctionPointer(name, value);
		}
	}

	private static void StopTextInput()
	{
		if (_stopTextInput != IntPtr.Zero)
		{
			StopTextInputDelegate delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer<StopTextInputDelegate>(_stopTextInput);
			delegateForFunctionPointer();
		}
	}
}
