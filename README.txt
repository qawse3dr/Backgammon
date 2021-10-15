################################################################################

CIS 3760 - Software Engineering
Numan Mir, Lawrence Milne, Rachel Broders, Ajit Chatha

################################################################################

################################################################################
Style Guide:
################################################################################

For this project we will be using Google's C# style guide. This style was chosen
as it is simple, easy to read, and fits with existing C# code nicely.
The style guide can be found here: https://google.github.io/styleguide/csharp-style.html

We made use of Clang-Format to help ensure that our code's style was up to par.
Clang-Format is a program that automatically styles code in accordance to a
style guide(In this case, Google's C# guide).

To run clang-format run the following command on the root of the repo.
Windows: > .\run-clang-format.bat
Linux/MacOS: $ sh run-clang-format.sh

################################################################################
Unit Testing:
################################################################################

Unit testing ensures that the code does break functionality once it
comes together.

These tests will be conducted before being merged and all of the tests must
pass before merging. Unit tests can be found in the ./Asset/Unit-Tests directory.

Unit test names must follow this format:
File/ClassName: TestNameOfClassOrFunctionalilityUnderTest
TestName: Test_WhatThisTestIsFor

Use the Test Runner to run a unit test, it can be found at:
Test Runner -> Window > General > Test Runner

Once there right click test or the parent and select run.

################################################################################
Commits:
################################################################################

Commits must include an explanation on what was done and should include
a ticket number and issue title at the first commit.

Menu: Create Basic Menu #39 (Part 1)
Created a basic menu that has 2 clickable options (Play, and Quit) that are both funcitonal
Either quitting the application or bringing you to a temp Backgammon scene from the backgammon Scene
a main Menu option was added just so you weren't stuck in the temp scene

A MouseClickAndHoverText controller was added that changes the colour on hover
and adds onclick to text. This can be applied to any text object to supply an easy onclick
and should be reused where we can

TODO:
    Unit Testing
################################################################################
Rules of Backgammon:
################################################################################

Backgammon is a game played by two players. It is played on a board that is
split into 4 quadrants, with each quadrant having 6 triangles in them called
"points". The two halves of the board are split up by ridge called the "bar".
The four quadrants are split into: your home quadrant, your opponent's home quadrant,
and 2 outer board quadrat. You want to move pieces from other quadrants into your
home quadrant to start bearing them off.

Each point is number starting from one, which would be the first point in your
home quadrant, and incremented by one moving clockwise.

To start your turn, the turn player must roll two dice. The player then has two
moves to make, they must select one of their pieces and move it to a open point
(open point is a triangle that contains one or less opponent pieces). The open
point must be 'X' spaces away where 'X' is equal to one of the dice rolls.
Once the player declares a move the dice that they used to complete that move is
used up and the player must make their second move with the remaining dice.

If the dice roll consists of a 'double' (a dice roll that consists of two of the
same number), the player may now make 4 moves that move their pieces 'X' spots,
where 'X' is the number of pips on one of the dice.

If the player is not able to complete a valid move, due to no there being no open
spaces that correspond to the dice roll, the player must pass their turn.

If the player moves their piece to a open point that contains one enemy piece,
the enemy piece must be removed from the board and placed onto the bar. This is
called 'Hitting'

If the turn player has any pieces on the bar they must roll their dice and complete
their turn by first moving any pieces they have on the bar onto the board. This is
called 'Entering'. Once the dice roll is done the player must move the pieces on
the bar onto the board starting from the first point in the opponent's home board.
If there are no valid moves for the piece on the bar the player must pass their
turn.

To win the game you must move all of your pieces to you home board and then
try to get them off of the board, this is called "bearing off".

To start 'bearing off' the player must have moved all 15 of their pieces to their
home board. Then the player rolls the dice and must first move any pieces that
will perfectly bear off the board (i.e. if the player rolls a 4 they must move
the piece that will come off the board in exactly 4 spaces first). If there is no
such piece the player must then do any valid move by moving a piece that is on
a higher point (point further away from the end of the board). Finally, if there
are no pieces on a higher point the player must start moving pieces on a lower point
(closer to the end of the board) off of the board.
