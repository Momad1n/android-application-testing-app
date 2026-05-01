*** Settings ***
Library           AppiumLibrary
Suite Setup       Open App
Suite Teardown    Close App
Test Setup        Log    Starting test
Test Teardown     Log    Test finished

*** Variables ***
${REMOTE_URL}     http://localhost:4723
${PLATFORM_NAME}  Android
${DEVICE_NAME}    ${DeviceName}
${PLATFORM_VERSION}  ${PlatformVersion}
${APP}            C:/Users/Momadin/AndroidStudioProjects/MusicApplication/app/build/outputs/apk/debug/app-debug.apk

*** Keywords ***
Open App
    Open Application    ${REMOTE_URL}
    ...    platformName=${PLATFORM_NAME}
    ...    deviceName=${DEVICE_NAME}
    ...    platformVersion=${PLATFORM_VERSION}
    ...    app=${APP}
    ...    appium:automationName=UiAutomator2

Close App
    Close Application

*** Test Cases ***
Demo Test
    [Documentation]    Простейший тест
    Log    Открываем приложение
    Sleep    5s
    Log    Приложение открылось
    Capture Page Screenshot
    Should Be True    ${True}