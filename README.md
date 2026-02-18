# Tunnel Train Game

A one-week Game-a-Week university project where you drill a tunnel through terrain to create a safe path for a train to travel from A to B.
The core systems revolve around a scalar-field-driven tilemap, a dynamic tile update pipeline, and BFS-based tunnel validation.

## Overview

You control a drill that carves through solid terrain.
The world is represented as a 2D scalar density field which determines what tile to display at each cell. As the player drills, the scalar values change, and tiles are swapped accordingly.

Once drilling is complete, the game performs a BFS path-check to verify that the tunnel is continuous, wide enough, and reaches the goal.

This entire project was built in one week, with a strong focus on technical problem-solving and destructive-terrain gameplay.

## Technical Breakdown
### 1. Scalar Field Tilemap System

Terrain is stored as a 2D scalar field (grid of floats).
Each cell contains:
 - A tile data reference
 - A durability value (scalar)
 - A dug state
   
Scalar meaning:

1.0 — solid rock

0.0 — fully dug/open space

Values in between — partially drilled, used for tile transitions

This approach allows:
- Fast runtime terrain destruction
- Clean drills using radius or directional sampling
- Easy tile state updates based on thresholds
- A data-driven way to handle terrain durability

### 2. Drill Mechanic

The drill constantly samples nearby cells, reducing their scalar durability value on each tick.

When a cell’s durability drops below the dug threshold, the tile is swapped to the “dug” type and becomes part of the tunnel space.

This creates a responsive, real-time carving effect without needing full mesh regeneration.

### 3. BFS Tunnel Validation

When the player stops drilling, the game runs a Breadth-First Search through the dug tilemap layer.

#### Process:

Start BFS at the train’s spawn cell.

BFS expands only through cells marked as dug.

If BFS successfully reaches the exit cell, the tunnel is valid.

If not, the train fails the run (crash).

This prevents the player from making disconnected pockets, one-pixel gaps, or broken tunnels that should not allow the train to pass.

### 4. Train Simulation

After BFS finds a valid path:

The path points are dropped downward until they land on the tunnel floor
(or hit the nearest solid tile).

This forms the final track trajectory.

The train then simulates its run across the generated path.

If any part of the path is invalid or unsupported, the train crashes.

## Gameplay Loop

The player drills a tunnel from the starting zone to the exit.

BFS validates the tunnel’s shape and connectivity.

A train is simulated through the generated track.

The train either successfully reaches the end…
or absolutely dies.

## Reflection
This project pushed me to build a fully destructible terrain system in a very short timeframe. I learned how effective a scalar-field-driven tilemap can be for rapid tile updates, and I refined my understanding of BFS by integrating it directly into the gameplay loop. The biggest challenges I faced were overscoping the project and managing performance issues when scaling the scalar field to larger dimensions. These limitations forced me to adapt the prototype and make smarter scope decisions to fit the available time while keeping the core mechanics functional and responsive.
