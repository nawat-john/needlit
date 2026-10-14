# NEEDLIT — M0 project

Unity project root: `D:\Code\needlit`. Open this folder with Unity **6000.3.26f1**. The project pins **URP 17.3.0**, **Input System 1.17.0**, and **Test Framework 1.6.0** in the package manifest and lock file. Windows Standalone support is required. First import may need package downloads. Close any Editor using this project before running the batch commands below.

The three M0 scenes were generated and the Windows build and a repeat build succeeded on 2026-10-14. EditMode passed 1/1 and PlayMode passed 3/3. The executable passed an outside-Editor startup smoke check; visual/manual gameplay review is still pending. **NEEDLIT > Create M0 Scenes** creates missing scenes only and preserves existing scene files. It registers `Assets/Needlit/Scenes/Bootstrap.unity`, `Home.unity`, and `GrindingLab.unity` in build settings. Start Play from Bootstrap. The build opens Home; walk with WASD, look with the mouse, press E on the blue doorway to enter GrindingLab, press E on the bench to enter the placeholder Grinding mode, and press Esc to return to exploration. The green doorway returns Home. The stock shown in GrindingLab is a 300 mm diameter, 1000 mm long visual reference; abrasion is M1 work.

For a repeatable command-line setup and Windows build in PowerShell:

```powershell
$unity = 'C:\Program Files\Unity\Hub\Editor\6000.3.26f1\Editor\Unity.exe'
$project = 'D:\Code\needlit'
New-Item -ItemType Directory -Force "$project\Logs" | Out-Null
$job = Start-Process -FilePath $unity -ArgumentList "-batchmode -nographics -quit -projectPath `"$project`" -executeMethod Needlit.Editor.ProjectSetup.CreateScenes -logFile `"$project\Logs\create-scenes.log`"" -WindowStyle Hidden -Wait -PassThru
if ($job.ExitCode -ne 0) { throw 'Scene creation failed; see Logs/create-scenes.log' }
$job = Start-Process -FilePath $unity -ArgumentList "-batchmode -nographics -quit -projectPath `"$project`" -executeMethod Needlit.Editor.ProjectSetup.BuildWindows -logFile `"$project\Logs\build-windows.log`"" -WindowStyle Hidden -Wait -PassThru
if ($job.ExitCode -ne 0) { throw 'Windows build failed; see Logs/build-windows.log' }
```

The output is `Builds/Windows/NEEDLIT.exe`. Run it outside the Editor to check the Home to GrindingLab path. Run tests through Unity's Test Runner, or:

```powershell
$job = Start-Process -FilePath $unity -ArgumentList "-batchmode -nographics -projectPath `"$project`" -runTests -testPlatform EditMode -testResults `"$project\Logs\editmode.xml`" -logFile `"$project\Logs\editmode.log`"" -WindowStyle Hidden -Wait -PassThru
if ($job.ExitCode -ne 0) { throw 'EditMode tests failed; see Logs/editmode.log' }
$job = Start-Process -FilePath $unity -ArgumentList "-batchmode -nographics -projectPath `"$project`" -runTests -testPlatform PlayMode -testResults `"$project\Logs\playmode.xml`" -logFile `"$project\Logs\playmode.log`"" -WindowStyle Hidden -Wait -PassThru
if ($job.ExitCode -ne 0) { throw 'PlayMode tests failed; see Logs/playmode.log' }
```

`Application.persistentDataPath` is the intended local save location when saves arrive in M2. The M0 `Paused` input mode stops player movement and unlocks the cursor; full simulation pause and UI are later work. The on-screen IMGUI text is a development HUD, not final UI. `Library`, temporary files, local logs, builds, and credentials are ignored by Git; commit `Assets` with `.meta`, `Packages`, and `ProjectSettings`.
