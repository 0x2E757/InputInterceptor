# InputInterceptor

Library for keyboard and mouse input interception and simulation. Based on [interception driver](http://www.oblita.com/interception.html). Library will work only if the driver is intalled, however library contains installer and the driver can be installed by `InputInterceptor.Install()` call.

## Usage

You are able to use static `InputInterceptor` class to interact with the driver in the same way as shown in the driver [examples](http://www.oblita.com/interception.html).

Library also contains `Hook` classes — `KeyboardHook` and `MouseHook`, which will do most of the work for you. They will intercept input and might simulate input. Note, that input simulation will be available after any input interception, otherwise hook willn't know device id.

MouseHook:
```C#
// Constructors
public MouseHook(MouseFilter filter = MouseFilter.All, CallbackAction callback = null);
public MouseHook(CallbackAction callback);
// Base methods
public Boolean SetMouseState(MouseState state, Int16 rolling = 0);
public Win32Point GetCursorPosition(); // uses user32.dll winapi
public Boolean SetCursorPosition(Win32Point point, Boolean useWinAPI = false, Boolean useWinAPIOnly = false);
public Boolean SetCursorPosition(Int32 x, Int32 y, Boolean useWinAPI = false, Boolean useWinAPIOnly = false);
public Boolean MoveCursorBy(Int32 dX, Int32 dY, Boolean useWinAPI = false, Boolean useWinAPIOnly = false);
// Simulation methods
public Boolean SimulateLeftButtonClick(Int32 releaseDelay = 50);
public Boolean SimulateMiddleButtonClick(Int32 releaseDelay = 50);
public Boolean SimulateRightButtonClick(Int32 releaseDelay = 50);
public Boolean SimulateScrollDown(Int16 rolling = 120);
public Boolean SimulateScrollUp(Int16 rolling = 120);
public Boolean SimulateMoveTo(Win32Point point, Int32 speed = 15, Boolean useWinAPI = false, Boolean useWinAPIOnly = false);
public Boolean SimulateMoveTo(Int32 x, Int32 y, Int32 speed = 15, Boolean useWinAPI = false, Boolean useWinAPIOnly = false);
public Boolean SimulateMoveBy(Int32 dX, Int32 dY, Int32 speed = 15, Boolean useWinAPI = false, Boolean useWinAPIOnly = false);
```

KeyboardHook:
```C#
// Constructors
public KeyboardHook(KeyboardFilter filter = KeyboardFilter.All, CallbackAction callback = null);
public KeyboardHook(CallbackAction callback);
// Base methods
public Boolean SetKeyState(KeyCode code, KeyState state);
// Simulation methods
public Boolean SimulateKeyPress(KeyCode code, Int32 releaseDelay = 75);
public Boolean SimulateInput(String text, Int32 delayBetweenKeyPresses = 100, Int32 releaseDelay = 75); // works with ANSI compatible string with english letters only (special chars are supported)
```

All functions returns `true` if operation succeeded. Also `Hook` derived classes contain base class properties:
```C#
public Context Context { get; private set; }
public Device Device { get; private set; }
public Filter FilterMode { get; private set; }
public Predicate Predicate { get; private set; }
public CallbackAction Callback { get; private set; }
public Exception Exception { get; private set; }
public Boolean Active { get; private set; }
public Thread InterceptionThread { get; private set; }
// Usefull getter properties
public Boolean IsInitialized { get => this.Context != Context.Zero && this.Device != -1; }
public Boolean HasException { get => this.Exception != null; }
```

## Example Application

```C#
private static void Main(String[] args) {
    if (InputInterceptor.CheckDriverInstalled()) {
        Console.WriteLine("Input interceptor seems to be installed.");
        if (InputInterceptor.Initialize()) {
            Console.WriteLine("Input interceptor successfully initialized.");
            MouseHook mouseHook = new MouseHook((ref MouseStroke mouseStroke) => {
                Console.WriteLine($"{mouseStroke.X} {mouseStroke.Y} {mouseStroke.Flags} {mouseStroke.State} {mouseStroke.Information}"); // Mouse XY coordinates are raw
            });
            KeyboardHook keyboardHook = new KeyboardHook((ref KeyStroke keyStroke) => {
                Console.WriteLine($"{keyStroke.Code} {keyStroke.State} {keyStroke.Information}");
            });
            Console.WriteLine("Hooks enabled. Press any key to release.");
            Console.ReadKey();
            keyboardHook.Dispose();
            mouseHook.Dispose();
        } else {
            Console.WriteLine("Input interceptor initialization failed.");
        }
    } else {
        Console.WriteLine("Input interceptor not installed.");
        if (InputInterceptor.CheckAdministratorRights()) {
            Console.WriteLine("Installing...");
            if (InputInterceptor.InstallDriver()) {
                Console.WriteLine("Done! Restart your computer.");
            } else {
                Console.WriteLine("Something... gone... wrong... :(");
            }
        } else {
            Console.WriteLine("Restart program with administrator rights so it will be installed.");
        }
    }
    Console.WriteLine("End of program. Press any key.");
    Console.ReadKey();
}
```

## Warning

You may lose keyboard and mouse control if your program freezes (due to an exception or something like that) when intercepting input. The hook classes are wrapped with `try catch`, however, for example, this will not save you from blocking the thread.