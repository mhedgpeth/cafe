.\cafe.exe service stop
if %errorlevel% neq 0 exit /b %errorlevel%

move cafe-staging/* .

.\cafe.exe service start
if %errorlevel% neq 0 exit /b %errorlevel%
