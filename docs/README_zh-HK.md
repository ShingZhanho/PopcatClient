# Popcat Client
Popcat Client 是用於自動在 [popcat.click](https://popcat.click) 洗Pop數的命令行程式。 程式亦會自動展示排行榜。

## 如何安裝
Popcat Client 不需安裝，
只需從 [發行版本頁](https://github.com/ShingZhanho/PopcatClient/releases) 下載最新版的zip檔，
將所有檔案解壓，並執行 `PopcatClient.exe`。

## 命令行引數
Popcat Client 支援命令行引數。以下為受支援的命令行引數說明：

### `--debug` 選項
開啟除錯模式，目前尚沒有任何作用。預設關閉。

### `--verbose` 選項
開啟囉唆模式。將會在執行時展示更多資訊。預設關閉。

### `--max-failures <整數: 錯誤計數>` 選項
指示程式在連續若干次出錯後自動退出。預設為3。

`<整數: 錯誤計數>` 引數：必選，程式在連續此次數出錯後會自動退出。必須大於0。

### `--init-pops <整數: 初次Pop計數>` 計數
指示程式應在初次試探性Pop時傳送的Pop數目。預設為1。

`<整數: 初次Pop計數>` 引數：必選，程式在試探性Pop時會傳送此數目的Pop。必須大於0且小於801。

### `--wait-time <整數: 等候時長>` 計數
指示程式應在傳送Pop之間等候的時長（以毫秒 / 千分之一秒作單為）。預設值為30000。

`<整數: 等候時長>` 引數: 必選，程式在傳送Pop之間會等候此長度的時間（毫秒作單位）。必須大於0。

### `--disable-leaderboard` 選項
禁用展示排行榜功能。預設開啟。
