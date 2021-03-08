# Emula80
Emula80 это интерпретатор ассемлер подобного языка. Данный язык назван "EmuLang" и представляет собой упрощенную версию ассемблера Intel/AMD x86/x64. Интерпретатор позволяет писать и исполнять программы на "EmuLang", концентрируясь на логико-арифметической устройстве центрального процессора. Язык обладает полнотой по тьюрингу, с возможностью применения оператора безусловного перехода. так же Emula80 является средой разработки и выполения програм.
## Особенности
1. Упрощенный ассемблер.
2. Интегрированная среда разработки для запуска и отладки программ.
3. Система расшироений.
## Состав решения
1. Emula80 - интегрированная среда разработки.
2. Lexer - лексический анализатор на основе [DoLang](https://github.com/Grumgog/doLang).
3. Processor - модель центрального процессора, а так же виртуальная машина для выполенения "EmuLang".
4. ConsoleTest - тестовый проект
## Система расширений
Emula80 поддерживает механизм расширений. Данный механизм предоставляет возможность интерпретатору загружать в память процесса DLL. DLL должен содержать класс с необходимым кодом. Благодаря механизму расширений Emula80 может взаимодействовать с "внешним миром". Так в коде уже объявлен класс для печати символов на принтер.
## Документация 
Документацию к проекту можно найти [тут](https://grumgog.github.io/).

# Emula80
Emula80 is asm-like interpreter. It can run programm written in "EmuLang" - simplified version of Intel x86 assembler.
this programming language concentrate on arithmetic/logic unit. Language is turing completeness - has logic and "jump" operators.
Emula80 is IDE for run Emulang programm and debug it.
## Features
1. Simplified asm
2. IDE for language
3. Extension system
## For programmers
This solution contain 4 projects.
1. Emula80 - IDE for emulang.
2. Lexer - lexical token analizator.
3. Processor - model of processor and interpreterer core (virtual machine).
4. ConsoleTest - Test project (nothing intresting)
## Documentation
You may find documentation [here](https://grumgog.github.io/)
 
