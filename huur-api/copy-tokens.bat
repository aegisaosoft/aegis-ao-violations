@echo off
echo ========================================
echo HuurApi Token Copy Script
echo ========================================
echo.

echo [1/2] Creating AppData directory...
if not exist "C:\Users\Alexander\AppData\Roaming\HuurApi" (
    mkdir "C:\Users\Alexander\AppData\Roaming\HuurApi"
    echo Created directory: C:\Users\Alexander\AppData\Roaming\HuurApi
) else (
    echo Directory already exists
)

echo.
echo [2/2] Copying token file to AppData...
copy "huur-api-tokens.json" "C:\Users\Alexander\AppData\Roaming\HuurApi\huur-api-tokens.json" /Y
if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo TOKEN FILE COPIED SUCCESSFULLY!
    echo ========================================
    echo.
    echo Location: C:\Users\Alexander\AppData\Roaming\HuurApi\huur-api-tokens.json
    echo.
    echo You can now run the application!
) else (
    echo.
    echo ========================================
    echo ERROR: Failed to copy token file!
    echo ========================================
    echo.
    echo Please check if huur-api-tokens.json exists in the current directory.
)

echo.
pause
