@echo off && cd /d "%~dp0"

mkdir "ref-%~1"
for /f "usebackq delims=" %%a in (
	`dir /a /b "..\..\..\..\bin\Win64_Shipping_Client\TaleWorlds*.dll" "..\..\..\..\bin\Win64_Shipping_Client\TaleWorlds*.exe" "..\..\..\..\bin\Win64_Shipping_Client\Bannerlord.exe"`
) do (
	ReferenceAssemblyGenerator -f -o "ref-%~1\%%~a" "..\..\..\..\bin\Win64_Shipping_Client\%%~a"
)

for %%m in ("Native" "SandBoxCore" "SandBox" "CustomBattle" "StoryMode") do (
	for /f "usebackq delims=" %%a in (
		`dir /a /b "..\..\..\%%~m\bin\Win64_Shipping_Client\*.dll" "..\..\..\%%~m\bin\Win64_Shipping_Client\*.exe"`
	) do (
		ReferenceAssemblyGenerator -f -o "ref-%~1\%%~a" "..\..\..\%%~m\bin\Win64_Shipping_Client\%%~a"
	)
)

pushd "ref-%~1"
..\7za a -bd -ssw -t7z -m0=lzma -mx=9 -mfb=256 -md=256m -ms=on "..\ref-%~1.7z" *
popd