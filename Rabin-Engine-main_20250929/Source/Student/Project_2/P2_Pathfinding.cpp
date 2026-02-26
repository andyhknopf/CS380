#include <pch.h>
#include "Projects/ProjectTwo.h"
#include "P2_Pathfinding.h"

unsigned int MAX_LOOPS = AStarPather::MAX_MAP_SIZE * AStarPather::MAX_MAP_SIZE;

#pragma region Extra Credit 
bool ProjectTwo::implemented_floyd_warshall()
{
    return false;
}

bool ProjectTwo::implemented_goal_bounding()
{
    return false;
}
#pragma endregion


bool AStarPather::initialize()
{
  startNode = nullptr;

  // handle any one-time setup requirements you have
  for (int row = 0; row < MAX_MAP_SIZE; row++)
  {
    for (int col = 0; col < MAX_MAP_SIZE; col++)
    {
      gridMap[row][col].gridPos = GridPos(row, col);
      assert(gridMap[row][col].gridPos.row < MAX_MAP_SIZE * MAX_MAP_SIZE || 
             gridMap[row][col].gridPos.col < MAX_MAP_SIZE * MAX_MAP_SIZE);

    }
  }

  // Initialize the open list
  //bottom = openList[0];
  back = -1;
  //PrecomputeNeighbors

  /*
      If you want to do any map-preprocessing, you'll need to listen
      for the map change message.  It'll look something like this:

      Callback cb = std::bind(&AStarPather::your_function_name, this);
      Messenger::listen_for_message(Messages::MAP_CHANGE, cb);

      There are other alternatives to using std::bind, so feel free to mix it up.
      Callback is just a typedef for std::function<void(void)>, so any std::invoke'able
      object that std::function can wrap will suffice.
  */

  Callback onMapChangeCB = std::bind(&AStarPather::OnMapChange, this);
  Messenger::listen_for_message(Messages::MAP_CHANGE, onMapChangeCB);

  return true; // return false if any errors actually occur, to stop engine initialization
}

void AStarPather::shutdown()
{
    /*
        Free any dynamically allocated memory or any other general house-
        keeping you need to do during shutdown.
    */
}

float AStarPather::CalculateHeuristic(GridNode* node, PathRequest * request)
{
  float cost = -999.99f;
  GridPos startNode = node->gridPos; // terrain->get_grid_position(request.start);
  GridPos goalNode = terrain->get_grid_position(request->goal);
  float xDiff = goal.col - start.col;
  float yDiff = goal.row - start.row;

  switch (request->settings.heuristic)
  {
    case Heuristic::OCTILE:
    {
      //min(xDiff, yDiff) * sqrt(2) + max(xDiff, yDiff) – min(xDiff, yDiff)
      // min(9, 3) * 1.41 + max(9, 3) – min(9, 3)
      float minimum = std::min(xDiff, yDiff);
      float maximum = std::max(xDiff, yDiff);
      cost = (minimum * ROOT2) + (maximum - minimum);
      break;
    }
    case Heuristic::CHEBYSHEV:
    {
      cost = std::max(xDiff, yDiff);
      break;
    }
    case Heuristic::INCONSISTENT:
    {
      if ((node->gridPos.row + node->gridPos.col) % 2 > 0)
        cost = GridPos::Distance(&start, &goal);
      else
        return 0.0f;

      break;
    }
    case Heuristic::MANHATTAN:
    {
      // Row difference
      // Column difference
      cost = (xDiff) + (yDiff);
      break;
    }
    case Heuristic::EUCLIDEAN:
    {
      cost = GridPos::Distance(&start, &goal);
      break;
    }
    default : // Just default to euclidean distance
    {
      cost = GridPos::Distance(&start,&goal);
      break;
    }
  }

  return cost * request->settings.weight;
}

PathResult AStarPather::compute_path(PathRequest &request)
{
  /*
      This is where you handle pathing requests, each request has several fields:

      start/goal - start and goal world positions
      path - where you will build the path upon completion, path should be
          start to goal, not goal to start
      heuristic - which heuristic calculation to use
      weight - the heuristic weight to be applied
      newRequest - whether this is the first request for this path, should generally
          be true, unless single step is on

      smoothing - whether to apply smoothing to the path
      rubberBanding - whether to apply rubber banding
      singleStep - whether to perform only a single A* step
      debugColoring - whether to color the grid based on the A* state:
          closed list nodes - yellow
          open list nodes - blue

          use terrain->set_color(row, col, Colors::YourColor);
          also it can be helpful to temporarily use other colors for specific states
          when you are testing your algorithms

      method - which algorithm to use: A*, Floyd-Warshall, JPS+, or goal bounding,
          will be A* generally, unless you implement extra credit features

      The return values are:
          PROCESSING - a path hasn't been found yet, should only be returned in
              single step mode until a path is found
          COMPLETE - a path to the goal was found and has been built in request.path
          IMPOSSIBLE - a path from start to goal does not exist, do not add start position to path
  */


  // Should be 
  int loopCount = 0;

  for (int row = 0; row < MAX_MAP_SIZE; ++row)
  {
    for (int col = 0; col < MAX_MAP_SIZE; ++col)
    {
      assert(gridMap[row][col].gridPos.row < MAX_MAP_SIZE * MAX_MAP_SIZE || gridMap[row][col].gridPos.col < MAX_MAP_SIZE * MAX_MAP_SIZE);
    }
  }

  // WRITE YOUR CODE HERE
  // First parent is the start always
  
  // TODO: Change heuristic based on UI button
  float heuristic = 0.0f;
  
  // If we got a new path request
  if (request.newRequest)
  {
    ClearNodes();

    start = terrain->get_grid_position(request.start);
    goal = terrain->get_grid_position(request.goal);

    // Just sample code, safe to delete
    if (request.settings.debugColoring)
    {
      terrain->set_color(start, Colors::Green);
      terrain->set_color(goal, Colors::Red);
    }

    // Push start node onto open list with cost of f(x) = g(x) + h(x)
    // Question: Will parentNode->givenCost always be zero?
    startNode = &gridMap[start.row][start.col];

    heuristic = CalculateHeuristic(startNode, &request);
    startNode->givenCost = 0.0f;
    startNode->finalCost = startNode->givenCost + heuristic;
    Push(startNode);
  }

  // While the open list isn't empty
  // Note: First time using 'Extract Function' feature in Visual Studio, wouldn't have though to do this
  bool retFlag;
  PathResult retVal = SearchOpenList(startNode, goal, request, loopCount, retFlag);
  if (retFlag) return retVal;

  // Open list empty, thus no path possible (return PathResult::IMPOSSIBLE)
  return PathResult::IMPOSSIBLE;
}

PathResult AStarPather::SearchOpenList(GridNode*& parentNode, GridPos& goal, PathRequest& request, int& loopCount, bool& retFlag)
{
  // Will we return the result of this function?
  retFlag = true;

  
  for (int i = 0; !openListEmpty; ++i)
  {
    // parent node = Pop cheapest node off of open list
    parentNode = Pop();
    if (!parentNode)
    {
      retFlag = true;
      return PathResult::IMPOSSIBLE;
    }

    // Check if parent node is uninitialized
    if (parentNode->onList == GridNode::UNVISITED)
    {
      if (parentNode->parent)
        parentNode->givenCost = parentNode->parent->givenCost + GridPos::Distance(&parentNode->parent->gridPos, &parentNode->gridPos);
      else
        parentNode->givenCost = 0;
    }

    // If parent node is goal node, path is found
    if (parentNode->gridPos == goal)
    {
      // Put final path onto the path list
      PushFinalPath(parentNode, request);

      return PathResult::COMPLETE;
    }

    // For all neighboring child nodes of parent node
    SearchNeighbors(parentNode, request);

    // Place parent node on closed list
    parentNode->onList = GridNode::CLOSED;

    if (request.settings.debugColoring)
      terrain->set_color(parentNode->gridPos, Colors::Yellow);

    // If taking too long or in one-step-per-frame mode
    ++loopCount;
    if (loopCount >= MAX_LOOPS || request.settings.singleStep)
      return PathResult::PROCESSING;
  }

  // Tell the parent function we are ignoring the return value of this child function
  retFlag = false;
  return {};
}

void AStarPather::SearchNeighbors(GridNode* parentNode, PathRequest & request)
{
  GridNode* childNode = nullptr; // The child we are currently searching through
  GridNode* toPush = nullptr;    // The child node we will push onto the open list


  // For each neighbor
  for (int i = 0; i < 8; ++i)
  {
    // Skip if this neighbor is null
    if (neighBors[parentNode->gridPos.row][parentNode->gridPos.col][i] == nullptr)
      continue;


    // Check a neighbor
    childNode = neighBors[parentNode->gridPos.row][parentNode->gridPos.col][i];
    assert(childNode->gridPos.row < MAX_MAP_SIZE * MAX_MAP_SIZE || childNode->gridPos.col < MAX_MAP_SIZE * MAX_MAP_SIZE);
      
    
    // TESTING: Delete this later 
    // assert(childNode->givenCost < MAX_MAP_SIZE * MAX_MAP_SIZE);

    // Calculate the cost of the child node
    float childGiven = parentNode->givenCost + GridPos::Distance(&parentNode->gridPos, &childNode->gridPos);

    // Put it on the open list (Push() on open list)
    // If child hasn't been visited yet (isn't on open and isn't on closed list)
    if (childNode->onList == GridNode::UNVISITED)
    {
      // If adding to open list: compute cost f(x) = g(x) + h(x), set new parent pointer!)
      float heuristic = CalculateHeuristic(childNode, &request);
      childNode->givenCost = childGiven;
      childNode->finalCost = childGiven + heuristic;
      childNode->parent = parentNode;
      Push(childNode);

      // Set parent pointer
      if (request.settings.debugColoring)
        terrain->set_color(childNode->gridPos, Colors::Blue);

      // Now search for the next neighbors
      continue;
    }

    // (Only relevant for visiting previously visited node from new direction)
    bool newOneIsCheaper = (childGiven < childNode->givenCost);
    
    if (newOneIsCheaper)
    {
      // Take old expensive one off both lists & put new cheaper one on open list (Update())
      Update(childNode, parentNode, childGiven);
    }

  }
}

void AStarPather::PushFinalPath(GridNode*& node, PathRequest& request)
{
  GridNode* path = node;
  while (path)
  {
    Vec3 nodePos = terrain->get_world_position(path->gridPos);
    request.path.push_back(nodePos);
    path = path->parent;
  }

  // Idk, easiest way to fix it? Probably very slow
  std::reverse(request.path.begin(), request.path.end());
}

void AStarPather::OnMapChange()
{
  // WARNING! This might be happening twice, potential slowdown
  ClearNodes();

  //for (int i = 0; i < MAX_MAP_SIZE; ++i)
  //  for (int j = 0; j < MAX_MAP_SIZE; ++j)
  //    for (int k = 0; k < 8; ++k)
  //      neighBors[i][j][k] = nullptr;

  // Precompute neighbors
  PrecomputeNeighbors();
}

void AStarPather::PrecomputeNeighbors()
{
  int rows = terrain->get_map_height();
  int cols = terrain->get_map_width();
  GridPos top(1,0), bottom(-1, 0), left(0, -1), right(0, 1);
  GridPos nodePos(0,0);

  // I really have not written anything this bad 
  // since freshman year, but I really don't care right now
  for (int row = 0; row < rows; row++)
  {
    for (int col = 0; col < cols; col++)
    {
      // TODO: Okay, time to figure out the diagonals
      // Check top left, middle, right
      nodePos = GridPos(row, col);

      // If I am not eligible
      if (!IsEligibleNode(nodePos))
        continue;

      // CASE: Top right
      FindEligibleNeighbors(top + right, nodePos);

      // CASE: Top middle
      FindEligibleNeighbors(top, nodePos);

      // CASE: Top left
      FindEligibleNeighbors(top + left, nodePos);

      // CASE: Center left
      FindEligibleNeighbors(left, nodePos);

      // CASE: Bottom left
      FindEligibleNeighbors(bottom + left, nodePos);

      // CASE: Bottom
      FindEligibleNeighbors(bottom, nodePos);

      // CASE: Bottom right
      FindEligibleNeighbors(bottom + right, nodePos);

      // CASE: Right
      FindEligibleNeighbors(right, nodePos);
    }
  }
}

bool AStarPather::FindEligibleNeighbors(GridPos& offset, GridPos& nodePos)
{
  // Skip if an invalid neighbor position


  if (!IsEligibleNode(nodePos + offset))
    return false;

  // CASE: Straight
  if (offset.row == 0 || offset.col == 0)
  {
    AddNeighbor(nodePos, nodePos + offset);
    return true;
  }

  // CASE: Diagonal
  GridPos vertOffset = GridPos(offset.row, 0);
  GridPos horizOffset = GridPos(0, offset.col);

  // Check horizontal & vertical positions
  if (IsEligibleNode(nodePos + horizOffset) && IsEligibleNode(nodePos + vertOffset))
  {
    AddNeighbor(nodePos, nodePos + offset); 
    return true;
  }

  return false;
}

bool AStarPather::IsEligibleNode(GridPos& node)
{
  if (!terrain->is_valid_grid_position(node) || terrain->is_wall(node))
    return false;
  
  return true;
}

void AStarPather::AddNeighbor(GridPos &node, GridPos &neighbor)
{
  // Add the corresponding neighbor to the neighbor list
  for (int i = 0; i < 8; ++i)
  {
    // Skip if this neighbor slot has been initialized already
    if (neighBors[node.row][node.col][i] == nullptr)
    {
      // Add this neighbor to the list of neighbors
      neighBors[node.row][node.col][i] = &gridMap[neighbor.row][neighbor.col];
      return;
    }
  }
}


void AStarPather::ClearNodes(void)
{

  ClearOpenList();
  startNode = nullptr;

  for (int i = 0; i < MAX_MAP_SIZE; ++i)
  {
    for (int j = 0; j < MAX_MAP_SIZE; ++j)
    {
      openList[i + (MAX_MAP_SIZE * j)] = nullptr;
      gridMap[i][j].ClearData();
    }
  }

  back = -1;
}

void AStarPather::ClearOpenList()
{

  //back = -1;
}

void AStarPather::Push(GridNode* node)
{
  // CASE AGNOSTIC:
  
  // Mark the open list as non-empty
  openListEmpty = false;
  


  // CASE: This node already allocated to array
  if (node->onList == GridNode::CLOSED || node->onList == GridNode::OPEN)
  {
    // Just mark it as 'open' and then return
    node->onList = GridNode::OPEN;
    return;
  }

  // CASE: Node not already allocated to array

  // If at maximum array bounds (should never ever happen)
  assert(back < (MAX_MAP_SIZE * MAX_MAP_SIZE) - 1);

  // Push the node onto the stack
  ++back;
  node->onList = GridNode::OPEN;
  openList[back] = node;
}


GridNode* AStarPather::Pop(void)
{
  GridNode* cheapestNode = nullptr;
  int cheapestNodeIndex = 0;

  if (back < 0)
  {
    openListEmpty = true;
    return nullptr;
  }

  // Loop through the open list and find the cheapest node
  for (int i = 0; openList[i] != nullptr; ++i)
  { 
    // Skip if node is on the closed list
    if (openList[i]->onList == GridNode::CLOSED)
      continue;

    // Switch cheapest node if 
    if (cheapestNode == nullptr || openList[i]->finalCost < cheapestNode->finalCost)
    {
      cheapestNode = openList[i];
      cheapestNodeIndex = i;
    }
  }

  // Putting onto closed list is handled elsewhere in SearchOpenList()
  openList[cheapestNodeIndex] = openList[back];
  openList[back] = nullptr;
  --back;

  if (back < 0)
    openListEmpty = true;
  return cheapestNode;
}

GridNode * AStarPather::Pop_DEPRECATED()
{
  GridNode* toPop = nullptr;

  // If the stack is empty
  if (back < 0)
  {
   
    return toPop;
  }
 
  // Save the top and decrement
  toPop = openList[back];
  --back;
  
  // Set the empty flag
  if (back < 0)
    openListEmpty = true;

  return toPop;
}

void AStarPather::Update(GridNode * node, GridNode * parentNode, float newCost)
{
  // Update the costs for this node
  float heuristic = node->finalCost - node->givenCost;
  node->givenCost = newCost;
  node->finalCost = newCost + heuristic;

  // Update the parent pointer too
  node->parent = parentNode;
  
  // Push onto open list
  Push(node);
}
