繁體中文版說明請[按此](./docs/README_zh-HK.md)。
# Popcat Client
Popcat Client is a command line tool that sends pops to [popcat.click](https://popcat.click) website every half a minute.
It also displays the leaderboard of your current location and the first on the board.

## How to install
Popcat Client does not need installing,
simply download the zip file from the [release page](https://github.com/ShingZhanho/PopcatClient/releases),
extract all files into a folder and run `PopcatClient.exe`. That's it!

## Command line options
Popcat Client supports command line options. Here are the complete doc of the available options:

### `--debug` option
Turns on Debug Mode. Currently does nothing.
Disabled by default.

### `--verbose` option
Turns on Verbose Mode. Prints more details while the application is running.
Disabled by default.

### `--max-failures <int: failures count>` option
Specifies how many failures in a row should the application exit automatically.
3 is the default value.

`<int: failure count>` parameter: Non-optional, an integer specifies the maximum sequential failures.
Must be larger than 0.

### `--init-pops <int: initial pops count>` option
Specifies how many pops should the application send to the server for testing.
1 is the default value.

`<int: inital pops count>` parameter: Non-optional, an integer specifies the number of pops
to send when starting the application. Must be larger than 0 and smaller than 801.

### `--wait-time <int: time to wait>` option
Specifies how much time (in ms) should the program wait between sending pops.
30000 is the default value.

`<int: time to wait>` parameter: Non-optional, an integer specifies the amount of time
(in ms) should the program wait between sending another bunch of pops.

### `--disable-leaderboard` option
Turns off leaderboard function. Enabled by default.
