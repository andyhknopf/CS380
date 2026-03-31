#include <pch.h>
#include "Terrain/TerrainAnalysis.h"
#include "Terrain/MapMath.h"
#include "Agent/AStarAgent.h"
#include "Terrain/MapLayer.h"
#include "Projects/ProjectThree.h"

#include <iostream>
#include <cmath>

bool ProjectThree::implemented_fog_of_war() const // extra credit
{
    return false;
}

float distance_to_closest_wall(int row, int col)
{
  float smallestDistance = 10000000000.0f; // IDK?

  /*
      Check the euclidean distance from the given cell to every other wall cell,
      with cells outside the map bounds treated as walls, and return the smallest
      distance.  Make use of the is_valid_grid_position and is_wall member
      functions in the global terrain to determine if a cell is within map bounds
      and a wall, respectively.
  */

  GridNode  *map = AStarPather::gridMap[0];

  // WRITE YOUR CODE HERE

  // Loop through each cell
  for (int i = 0; i < terrain->get_map_height(); ++i)
  {
    for (int j = 0; j < terrain->get_map_width(); ++j)
    {
      // Skip non walls
      if (terrain->is_valid_grid_position(map[j].gridPos) || terrain->is_wall(map[j].gridPos))
        continue;

      // Get the distane from the nearest wall spot
      float dist = GridPos::Distance(&AStarPather::gridMap[i][j].gridPos, &GridPos(row, col));
      if (dist < smallestDistance)
        dist = smallestDistance;
    }
  }
  
  return smallestDistance; // REPLACE THIS
}

bool is_clear_path(int row0, int col0, int row1, int col1)
{
  /*
      Two cells (row0, col0) and (row1, col1) are visible to each other if a line
      between their centerpoints doesn't intersect the four boundary lines of every
      wall cell.  You should puff out the four boundary lines by a very tiny amount
      so that a diagonal line passing by the corner will intersect it.  Make use of the
      line_intersect helper function for the intersection test and the is_wall member
      function in the global terrain to determine if a cell is a wall or not.
  */

  // I guess add an epsilon to the min and max bounds of each cell bounding box?
  // If the ray intersects any walls or corners then return false, else returns true

  // Skip yourself
  if (row0 == row1 && col0 == col1)
    return false;

  // Get min and max of the rays bounding box
  int rowMax = std::max(row0, row1);
  int colMax = std::max(col0, col1);

  int rowMin = std::min(row0, row1);
  int colMin = std::min(col0, col1);
  
  GridPos one(1, 0), two(2, 0);
  
  Vec3 worldSizes = terrain->get_world_position(two) - terrain->get_world_position(one);
  float cellSize = 1.0f / worldSizes.x; 
  float halfCell = (cellSize / 2.0f) + 0.00001f; // Extend cell bounds by a little bit

  // Get the path vector in grid space
  GridPos startPos(row0, col0), endPos(row1, col1);
  Vec3 pointAWorld(terrain->get_world_position(startPos));
  Vec3 pointBWorld(terrain->get_world_position(endPos));
  Vec2 pointAGrid(pointAWorld.x, pointAWorld.z);
  Vec2 pointBGrid(pointBWorld.x, pointBWorld.z);

  // For each cell in the ray's bounding box (use code from AStarPather)
  for (int i = rowMin; i < rowMax; ++i)
  {
    for (int j = colMin; j < colMax; ++j)
    {
      // Check for errors
      assert(terrain->is_valid_grid_position(GridPos(i, j)), "Never check outside the map bounds!");

      // Only check wall cells
      if (!terrain->is_wall(GridPos(i, j)))
        continue;

      // Check left, bottom, right, top
      Vec3 cellPos = terrain->get_world_position(i, j);
      Vec2 topRight(cellPos.x + halfCell, cellPos.z + halfCell);
      Vec2 topLeft(cellPos.x + halfCell, cellPos.z - halfCell);
      Vec2 botRight(cellPos.x - halfCell, cellPos.z + halfCell);
      Vec2 botLeft(cellPos.x - halfCell, cellPos.z - halfCell);

      // If the lines intersect at any point then there is no clear path
      if (line_intersect(topLeft, botLeft, pointAGrid, pointBGrid))
        return false;
      else if (line_intersect(botLeft, botRight, pointAGrid, pointBGrid))
        return false;
      else if (line_intersect(botRight, topRight, pointAGrid, pointBGrid))
        return false;
      else if (line_intersect(topRight, topLeft, pointAGrid, pointBGrid))
        return false;
    }
  }


  return true; // REPLACE THIS
}

void analyze_openness(MapLayer<float> &layer)
{
  /*
      Mark every cell in the given layer with the value 1 / (d * d),
      where d is the distance to the closest wall or edge.  Make use of the
      distance_to_closest_wall helper function.  Walls should not be marked.
  */
  // WRITE YOUR CODE HERE

  // For each 
  for (int i = 0; i < terrain->get_map_height(); ++i)
  {
    for (int j = 0; j < terrain->get_map_width(); ++j)
    {
      GridPos pos = AStarPather::gridMap[i][j].gridPos;
      float dist = distance_to_closest_wall(pos.row, pos.col);
      float epsilon = 0.0000000001f;

      // Safe divide
      float openness = 1.0f / ((dist * dist) + epsilon);

      layer.set_value(AStarPather::gridMap[i][j].gridPos, openness);
    }
  }
}

float analyze_cell_visibility(GridNode * node)
{
  float visibility = 0.0f;

  for (int i = 0; i < terrain->get_map_height(); ++i)
  {
    for (int j = 0; j < terrain->get_map_width(); ++j)
    {
      // Increment if a cell is visible to this one
      GridPos cellPos = AStarPather::gridMap[i][j].gridPos;
      if (is_clear_path(node->gridPos.row, node->gridPos.col, cellPos.row, cellPos.col))
        ++visibility;
    }
  }

  return visibility;
}

void analyze_visibility(MapLayer<float> &layer)
{
  /*
      Mark every cell in the given layer with the number of cells that
      are visible to it, divided by 160 (a magic number that looks good).  Make sure
      to cap the value at 1.0 as well.

      Two cells are visible to each other if a line between their centerpoints doesn't
      intersect the four boundary lines of every wall cell.  Make use of the is_clear_path
      helper function.
  */


  float divisor = 160.0f;

  // For each cell
  for (int i = 0; i < terrain->get_map_height(); ++i)
  {
    for (int j = 0; j < terrain->get_map_width(); ++j)
    {
      if (terrain->is_wall(AStarPather::gridMap[i][j].gridPos))
        continue;

      // Check how visible this cell is to every other cell (max value of 1)
      float cellVisibility = analyze_cell_visibility(&AStarPather::gridMap[i][j]);
      float visibility = std::min( cellVisibility / divisor, 1.0f);
      layer.set_value(AStarPather::gridMap[i][j].gridPos, visibility);
    }
  }
}

void analyze_visible_to_cell(MapLayer<float> &layer, int row, int col)
{
  /*
      For every cell in the given layer mark it with 1.0
      if it is visible to the given cell, 0.5 if it isn't visible but is next to a visible cell,
      or 0.0 otherwise.

      Two cells are visible to each other if a line between their centerpoints doesn't
      intersect the four boundary lines of every wall cell.  Make use of the is_clear_path
      helper function.
  */


  // WRITE YOUR CODE HERE
  float visibility = 0.0f;

  for (int i = 0; i < terrain->get_map_height(); ++i)
  {
    for (int j = 0; j < terrain->get_map_width(); ++j)
    {
      GridPos cellPos = AStarPather::gridMap[i][j].gridPos;

      // If this cell is visible
      if (is_clear_path(row, col, cellPos.row, cellPos.col))
      {
        layer.set_value(cellPos, 1.0);
        continue;
      }

      // See if we are next to another cell
      // Does this include diagonal neighbors????
      for (int neighbors = 0; neighbors < 8; ++neighbors)
      {
        GridNode * neighbor = AStarPather::gridMap[i][j].neighbors[neighbors];
        if (!neighbor)
          continue;
        else if (terrain->is_wall(neighbor->gridPos))
        {
          layer.set_value(cellPos, 0.0f);
          continue;
        }

        if (is_clear_path(row, col, neighbor->gridPos.row, neighbor->gridPos.col))
        {
          layer.set_value(cellPos, 0.5f);
          continue;
        }
      }
    }
  }
}

void analyze_agent_vision(MapLayer<float> &layer, const Agent *agent)
{
  /*
      For every cell in the given layer that is visible to the given agent,
      mark it as 1.0, otherwise don't change the cell's current value.

      You must consider the direction the agent is facing.  All of the agent data is
      in three dimensions, but to simplify you should operate in two dimensions, the XZ plane.

      Take the dot product between the view vector and the vector from the agent to the cell,
      both normalized, and compare the cosines directly instead of taking the arccosine to
      avoid introducing floating-point inaccuracy (larger cosine means smaller angle).

      Give the agent a field of view slighter larger than 180 degrees.

      Two cells are visible to each other if a line between their centerpoints doesn't
      intersect the four boundary lines of every wall cell.  Make use of the is_clear_path
      helper function.
  */
  
  // Get the position of the agent
  GridPos agentGridPos = terrain->get_grid_position(agent->get_position());
  Vec2 agentPos(agent->get_position().x, agent->get_position().z);
  Vec2 agentForward(agent->get_forward_vector().x, agent->get_forward_vector().z);
  agentForward.Normalize();

  // WRITE YOUR CODE HERE
  for (int i = 0; i < terrain->get_map_height(); ++i)
  {
    for (int j = 0; j < terrain->get_map_width(); ++j)
    {
      GridPos cellPos = AStarPather::gridMap[i][j].gridPos;

      // Calculate the agent to cell vector
      Vec2 cellToAgent(terrain->get_world_position(cellPos).x, terrain->get_world_position(cellPos).z);
      cellToAgent -= agentPos;
      cellToAgent.Normalize();

      // Skip if agent isn't facing the cell
      if (agentForward.Dot(cellToAgent) < -0.00000001f)
        continue;

      if (terrain->is_wall(cellPos))
      {
        layer.set_value(cellPos, 0.0);
        continue;
      }
      // TESTING!      if (is_clear_path(agentGridPos.row, agentGridPos.col, cellPos.row, cellPos.col))

      // If this cell is visible
      if (is_clear_path(agentGridPos.row, agentGridPos.col, cellPos.row, cellPos.col))
      {
        layer.set_value(cellPos, 1.0);
      }
    }
  }
}

void propagate_solo_occupancy(MapLayer<float> &layer, float decay, float growth)
{
  /*
      For every cell in the given layer:

          1) Get the value of each neighbor and apply decay factor
          2) Keep the highest value from step 1
          3) Linearly interpolate from the cell's current value to the value from step 2
              with the growing factor as a coefficient.  Make use of the lerp helper function.
          4) Store the value from step 3 in a temporary layer.
              A float[40][40] will suffice, no need to dynamically allocate or make a new MapLayer.

      After every cell has been processed into the temporary layer, write the temporary layer into
      the given layer;
  */
    
  // WRITE YOUR CODE HERE
}

void normalize_solo_occupancy(MapLayer<float> &layer)
{
  /*
      Determine the maximum value in the given layer, and then divide the value
      for every cell in the layer by that amount.  This will keep the values in the
      range of [0, 1].  Negative values should be left unmodified.
  */

  // Determine max value in layer
  for (int i = 0; i < AStarPather::MAX_MAP_SIZE; ++i)
  {

  }

  // Divide the value for every cell in the layer by that amount
  // WRITE YOUR CODE HERE
}

void enemy_field_of_view(MapLayer<float> &layer, float fovAngle, float closeDistance, float occupancyValue, AStarAgent *enemy)
{
  /*
      First, clear out the old values in the map layer by setting any negative value to 0.
      Then, for every cell in the layer that is within the field of view cone, from the
      enemy agent, mark it with the occupancy value.  Take the dot product between the view
      vector and the vector from the agent to the cell, both normalized, and compare the
      cosines directly instead of taking the arccosine to avoid introducing floating-point
      inaccuracy (larger cosine means smaller angle).

      If the tile is close enough to the enemy (less than closeDistance),
      you only check if it's visible to enemy.  Make use of the is_clear_path
      helper function.  Otherwise, you must consider the direction the enemy is facing too.
      This creates a radius around the enemy that the player can be detected within, as well
      as a fov cone.
  */

  // WRITE YOUR CODE HERE
}

bool enemy_find_player(MapLayer<float> &layer, AStarAgent *enemy, Agent *player)
{
  /*
      Check if the player's current tile has a negative value, ie in the fov cone
      or within a detection radius.
  */

  const auto &playerWorldPos = player->get_position();

  const auto playerGridPos = terrain->get_grid_position(playerWorldPos);

  // verify a valid position was returned
  if (terrain->is_valid_grid_position(playerGridPos) == true)
  {
      if (layer.get_value(playerGridPos) < 0.0f)
      {
          return true;
      }
  }

  // player isn't in the detection radius or fov cone, OR somehow off the map
  return false;
}

bool enemy_seek_player(MapLayer<float> &layer, AStarAgent *enemy)
{
  /*
      Attempt to find a cell with the highest nonzero value (normalization may
      not produce exactly 1.0 due to floating point error), and then set it as
      the new target, using enemy->path_to.

      If there are multiple cells with the same highest value, then pick the
      cell closest to the enemy.

      Return whether a target cell was found.
  */

  // WRITE YOUR CODE HERE

  return false; // REPLACE THIS
}
