# Make Pizza
This is an example of a [Scheduling Problem](https://developers.google.com/optimization/scheduling), using the CP Solver.

## Objective
Given a list of pizzas to make, and a number of willing chefs, schedule

The making of a pizza can be broken into stages:
- Shaping 
- Slicing
- Preparing
- Cooking 

All stages apart from cooking can be performed by a chef, which must be done in the ove.
 
The type of the pizza dictates how long each stage takes.

## Rules
- No chef can work on two pizzas at the same time.
- Only one pizza can fit in the oven.
- The stage after shaping must start no earlier than 60 minutes after it's finished.