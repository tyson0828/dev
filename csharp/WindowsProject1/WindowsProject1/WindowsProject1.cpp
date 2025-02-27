// WindowsProject1.cpp : Defines the entry point for the application.
//

#include "framework.h"
#include "WindowsProject1.h"
#include <windows.h>
#include <locale.h>
#include <chrono>
#include <thread>
#include <string>

#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE hInst;                                // current instance
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);


HDC hdc;
PAINTSTRUCT ps;
/***Serpent's function****************/
void LeftClick(void)
{
    INPUT Input = { 0 };
    Input.type = INPUT_MOUSE;
    Input.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
    ::SendInput(1, &Input, sizeof(INPUT));
    ::ZeroMemory(&Input, sizeof(INPUT));
    Input.type = INPUT_MOUSE;
    Input.mi.dwFlags = MOUSEEVENTF_LEFTUP;
    ::SendInput(1, &Input, sizeof(INPUT));
}/************************************/
HWND hwnd, hwnd_outra_win;
HWND hWndEdit;

WNDPROC oldEditProc;

MSG messages;
POINT pos_cursor;
POINT Move_mouse(int x, int y) {
    GetCursorPos(&pos_cursor);
    pos_cursor.x += (long)x; pos_cursor.y += (long)y;
    SetCursorPos(pos_cursor.x, pos_cursor.y);
    return pos_cursor;
}
const int i_velox = 10;
const int hk_f4 = 0x64;
const int hk_cima = 0x26;
const int hk_baixo = 0x28;
const int hk_dir = 0x27;
const int hk_esq = 0x25;

long long sleep_duratoin = 2178420000;
// 2178420000 구멍뚫기
// 2178420000 근거리, 2178440000 군중 시도해봄
// 2178400000 쇠못덫지속시간, 2178440000 군중 ㄴ

// 명품화
// 2178400000 --> 2/5

LRESULT CALLBACK subEditProc(HWND wnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_KEYDOWN:
        switch (wParam)
        {
        case VK_RETURN:
            //Do your stuff
            std::wstring title;
            title.reserve(GetWindowTextLength(hWndEdit) + 1);
            GetWindowText(hWndEdit, const_cast<WCHAR*>(title.c_str()), title.capacity());
            _locale_t locale = _create_locale(LC_ALL, "C");
            sleep_duratoin = std::stoll(title.c_str(), nullptr, 10);
            MessageBox(0, title.c_str(), L"Updated", 0);
            break;  //or return 0; if you don't want to pass it further to def proc
            //If not your key, skip to default:

        }
    default:
        return CallWindowProc(oldEditProc, wnd, msg, wParam, lParam);
    }
    return 0;
}


LRESULT CALLBACK WinProc_Principal(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
    case WM_PAINT:
        hdc = BeginPaint(hwnd, &ps);
        SetBkMode(hdc, TRANSPARENT);
        ExtTextOut(hdc, 10, 5, ETO_OPAQUE, 0, TEXT("by: wn_br"), 9, 0);
        ExtTextOut(hdc, 10, 30, ETO_OPAQUE, 0, TEXT("thanks to Serpent !"), 19, 0);
        EndPaint(hwnd, &ps);
        break;
    case WM_HOTKEY:
        if (wParam == (long)hk_f4)
        {
            LeftClick();
            std::this_thread::sleep_for(std::chrono::nanoseconds(sleep_duratoin));

            LeftClick();
        }
        else if (wParam == (long)hk_cima) Move_mouse(0, -i_velox);
        else if (wParam == (long)hk_baixo) Move_mouse(0, +i_velox);
        else if (wParam == (long)hk_dir) Move_mouse(+i_velox, 0);
        else if (wParam == (long)hk_esq) Move_mouse(-i_velox, 0);
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hwnd, message, wParam, lParam);
    }
    return 0;
}



int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Place code here.
    WNDCLASSEX wincl = { sizeof(WNDCLASSEX),0,WinProc_Principal,0,0,
          hInstance,LoadIcon(NULL,IDI_APPLICATION),LoadCursor(NULL,IDC_ARROW),
          CreateSolidBrush(RGB(0,0,0xFF)),0,TEXT("C_Win"),LoadIcon(NULL,IDI_APPLICATION) };
    if (!RegisterClassEx(&wincl))return 0;
    hwnd = CreateWindowEx(0, TEXT("C_Win"), TEXT("S_Clicks"), WS_OVERLAPPED | WS_SYSMENU | WS_VISIBLE, CW_USEDEFAULT, CW_USEDEFAULT, 550, 400, HWND_DESKTOP, NULL, hInstance, NULL);      
    hWndEdit = CreateWindowEx(WS_EX_CLIENTEDGE, TEXT("Edit"), TEXT("2178450000"),
        WS_CHILD | WS_VISIBLE, 100, 20, 140,
        20, hwnd, NULL, NULL, NULL);

    oldEditProc = (WNDPROC)SetWindowLongPtr(hWndEdit, GWLP_WNDPROC, (LONG_PTR)subEditProc);

    RegisterHotKey(hwnd, hk_f4, 0, VK_F4);
    RegisterHotKey(hwnd, hk_cima, 0, hk_cima);
    RegisterHotKey(hwnd, hk_baixo, 0, hk_baixo);
    RegisterHotKey(hwnd, hk_dir, 0, hk_dir);
    RegisterHotKey(hwnd, hk_esq, 0, hk_esq);
    while (GetMessage(&messages, NULL, 0, 0)) {
        TranslateMessage(&messages);
        DispatchMessage(&messages);
    };
    return messages.wParam;

    // Initialize global strings
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_WINDOWSPROJECT1, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_WINDOWSPROJECT1));

    MSG msg;

    // Main message loop:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_WINDOWSPROJECT1));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_WINDOWSPROJECT1);
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Store instance handle in our global variable

   HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE: Processes messages for the main window.
//
//  WM_COMMAND  - process the application menu
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // Parse the menu selections:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        }
        break;
    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);
            // TODO: Add any drawing code that uses hdc here...
            EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
