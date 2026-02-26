#pragma once

class GridNode
{
  
  // TODO: Make structure as small as possible
  public:
    enum ListStatus : int
    { 
      UNVISITED = -1,
      CLOSED = 0,
      OPEN = 1
    };
    
    // Big four
    GridNode(GridNode* par = nullptr, GridPos pos = GridPos(-999, -999), float fCost = 0.0f, float gCost = 0.0f, ListStatus status = ListStatus::UNVISITED);
    GridNode(GridNode* other);
    GridNode & operator=(GridNode* rhs);

    // Methods
    void ClearData(void);

    // Members
    GridNode* neighbors[8];
    GridNode* parent;
    GridPos gridPos;
    float finalCost;
    float givenCost;
    ListStatus onList;
};

