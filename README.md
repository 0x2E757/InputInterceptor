# InputInterceptor

Library for keyboard and mouse input interception and simulation. Based on [interception driver](http://www.oblita.com/interception.html). Library will work only if the driver is intalled, however library contains installer and the driver can be installed by `InputInterceptor.InstallDriver()` call (`InputInterceptor.UninstallDriver()` for uninstall). Available as [NuGet package](https://www.nuget.org/packages/InputInterceptor/).

Compatible with .NET Standard 1.3, 1.4, 1.5, 1.6, 2.0 and 2.1!

## Usage

You are able to use static `InputInterceptor` class to interact with the driver in the same way as shown in the driver [examples](http://www.oblita.com/interception.html).

Library also contains `Hook` derived classes — `KeyboardHook` and `MouseHook`, which will do most of the work for you. They will intercept input and have methods to simulate input. Note, that input simulation might not work right if device wasn't captured (random keyboard/mouse device will be used). Even if hook doesn't recieve callback it will capture device with first input by this device.

### MouseHook
```C#
// Constructors
MouseHook(MouseFilter filter = MouseFilter.All, CallbackAction callback = null);
MouseHook(CallbackAction callback);
// Base methods
Boolean SetMouseState(MouseState state, Int16 rolling = 0);
Win32Point GetCursorPosition();
Boolean SetCursorPosition(Win32Point point, Boolean useWinAPI = false);
Boolean SetCursorPosition(Int32 x, Int32 y, Boolean useWinAPI = false);
Boolean MoveCursorBy(Int32 dX, Int32 dY, Boolean useWinAPI = false);
// Simulation methods
Boolean SimulateLeftButtonDown();
Boolean SimulateLeftButtonUp();
Boolean SimulateLeftButtonClick(Int32 releaseDelay = 50);
Boolean SimulateMiddleButtonDown();
Boolean SimulateMiddleButtonUp();
Boolean SimulateMiddleButtonClick(Int32 releaseDelay = 50);
Boolean SimulateRightButtonDown();
Boolean SimulateRightButtonUp();
Boolean SimulateRightButtonClick(Int32 releaseDelay = 50);
Boolean SimulateScrollDown(Int16 rolling = 120);
Boolean SimulateScrollUp(Int16 rolling = 120);
Boolean SimulateMoveTo(Win32Point point, Int32 speed = 15, Boolean useWinAPI = false);
Boolean SimulateMoveTo(Int32 x, Int32 y, Int32 speed = 15, Boolean useWinAPI = false);
Boolean SimulateMoveBy(Int32 dX, Int32 dY, Int32 speed = 15, Boolean useWinAPI = false);
```

### KeyboardHook
```C#
// Constructors
KeyboardHook(KeyboardFilter filter = KeyboardFilter.All, CallbackAction callback = null);
KeyboardHook(CallbackAction callback);
// Base methods
Boolean SetKeyState(KeyCode code, KeyState state);
// Simulation methods
Boolean SimulateKeyDown(KeyCode code);
Boolean SimulateKeyUp(KeyCode code);
Boolean SimulateKeyPress(KeyCode code, Int32 releaseDelay = 75);
Boolean SimulateInput(String text, Int32 delayBetweenKeyPresses = 50, Int32 releaseDelay = 75);
```
<sub>\* `SimulateInput` method works with ANSI compatible string with english letters only (special chars are supported)</sub>

### Hook
```C#
// Base properties
Context Context { get; private set; }
Device Device { get; private set; }
Device RandomDevice { get; private set; }
Filter FilterMode { get; private set; }
Predicate Predicate { get; private set; }
CallbackAction Callback { get; private set; }
Exception Exception { get; private set; }
Boolean Active { get; private set; }
Thread Thread { get; private set; }
// Usefull getter properties
Boolean IsInitialized => this.Context != Context.Zero && this.Device != -1;
Boolean CanSimulateInput => this.Context != Context.Zero && (this.RandomDevice != -1 || this.Device != -1);
Boolean HasException => this.Exception != null;
```

## Example Application

```C#
void Main(string[] args) {
    // Check if the driver is installed
    if (InputInterceptor.CheckDriverInstalled()) {
        Console.WriteLine("Input interceptor seems to be installed.");
        // Generate a temporary dll for communication with the driver
        if (InputInterceptor.Initialize()) {
            Console.WriteLine("Input interceptor successfully initialized.");
            // Create simple hooks with outputting all actions to console 
            MouseHook mouseHook = new MouseHook((ref MouseStroke mouseStroke) => {
                Console.WriteLine($"{mouseStroke.X} {mouseStroke.Y} {mouseStroke.Flags} {mouseStroke.State} {mouseStroke.Information}"); // Mouse XY coordinates are raw
            });
            KeyboardHook keyboardHook = new KeyboardHook((ref KeyStroke keyStroke) => {
                Console.WriteLine($"{keyStroke.Code} {keyStroke.State} {keyStroke.Information}");
            });
            Console.WriteLine("Hooks enabled. Press any key to release.");
            Console.ReadKey();
            // Dispose internal filters
            keyboardHook.Dispose();
            mouseHook.Dispose();
            // Cleanup system from temporary dll
            InputInterceptor.Dispose();
        } else {
            Console.WriteLine("Input interceptor initialization failed.");
        }
    } else {
        // Try to install the driver if it's not
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

You may lose keyboard and mouse control if your program freezes (due to an exception in main thread or something like that) when intercepting input. The hook classes are wrapped with `try catch`, however, for example, this will not save you from blocking the thread.