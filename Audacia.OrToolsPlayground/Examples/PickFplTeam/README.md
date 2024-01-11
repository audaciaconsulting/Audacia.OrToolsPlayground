# Pick FPL Team
This is an example of a constraint solver problem, using the CP-SAT solver.


## Objective
The objective of the model is, given a list of players and their costs, create a fantasy FPL team who scored the most points last season.

The CSV used here is downloaded from [this repo](https://github.com/vaastav/Fantasy-Premier-League).


## Rules
- The total cost of players cannot exceed the configured amount.
- The number of players in a certain position cannot exceed the configured amount.
- The number of players for a certain team cannot exceed the configured amount.