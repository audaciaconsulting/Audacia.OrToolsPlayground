# Assigning Pizza
This is an example of an [Assignment Problem](https://developers.google.com/optimization/assignment/overview), using the CP-SAT solver.

## Objective
The objective of the model is, given a list of pizzas, and a list of employees, assign a pizza to each employee.

Each employee has their own preferred pizza type. The model will maximize giving everyone the pizzas they like the best.

## Rules
- Each employee gets a maximum of one pizza.
- Each pizza is distributed once.
- Pizzas are leftover if there are more pizzas than employees. 