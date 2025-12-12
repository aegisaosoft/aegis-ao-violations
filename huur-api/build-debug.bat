@echo off
echo ========================================
echo HuurApi Debug Build Script
echo ========================================
echo.

echo [1/4] Creating AppData directory...
if not exist "C:\Users\Alexander\AppData\Roaming\HuurApi" (
    mkdir "C:\Users\Alexander\AppData\Roaming\HuurApi"
    echo Created directory: C:\Users\Alexander\AppData\Roaming\HuurApi
) else (
    echo Directory already exists
)

echo.
echo [2/4] Copying token file to AppData...
copy "huur-api-tokens.json" "C:\Users\Alexander\AppData\Roaming\HuurApi\huur-api-tokens.json" /Y
if %ERRORLEVEL% EQU 0 (
    echo Token file copied successfully!
) else (
    echo ERROR: Failed to copy token file!
    pause
    exit /b 1
)

echo.
echo [3/4] Restoring NuGet packages...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to restore packages!
    pause
    exit /b 1
)

echo.
echo [4/4] Building solution in Debug mode...
dotnet build --configuration Debug
if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo DEBUG BUILD SUCCESSFUL! 
    echo ========================================
    echo.
    echo Token file location: C:\Users\Alexander\AppData\Roaming\HuurApi\huur-api-tokens.json
    echo Build output: bin\Debug\
    echo.
) else (
    echo.
    echo ========================================
    echo DEBUG BUILD FAILED!
    echo ========================================
    echo.
    pause
    exit /b 1
)

echo Debug build completed successfully!
pause
