clang-format -i `find -f . | grep ".cs$" | grep -v Library` -Werror -style=file
