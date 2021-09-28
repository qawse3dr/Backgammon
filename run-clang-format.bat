@echo off
for /f %%w in ('dir  /s /b Assets\*.cs') do clang-format -i %%w -Werror -style=file