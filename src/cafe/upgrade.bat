echo performing requested upgrade at %date% %time% >> logs/upgrade.log
echo stopping cafe service >> logs/upgrade.log
.\cafe.exe service stop >> logs/upgrade.log
if %errorlevel% neq 0 exit /b %errorlevel%

echo copying from staging to current folder >> logs/upgrade.log
xcopy cafe-staging\*.* . /Y >> logs/upgrade.log

echo starting service >> logs/upgrade.log
.\cafe.exe service start >> logs/upgrade.log
if %errorlevel% neq 0 exit /b %errorlevel%
