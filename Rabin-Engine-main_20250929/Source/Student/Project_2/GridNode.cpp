#include "pch.h"
#include "GridNode.h"


GridNode::GridNode(GridNode* par, GridPos pos, float fCost, float gCost, ListStatus status)
  : 
    parent(par),
    gridPos(pos),
    finalCost(fCost),
    givenCost(gCost),
    onList(status)
{

}


GridNode::GridNode(GridNode* other)
  : 
    parent(other->parent),
    gridPos(other->gridPos),
    finalCost(other->finalCost),
    givenCost(other->givenCost),
    onList(other->onList)
{

}

GridNode & GridNode::operator=(GridNode* rhs)
{
  this->gridPos = rhs->gridPos;
  this->parent = rhs->parent;
  this->givenCost = rhs->givenCost;
  this->finalCost = rhs->finalCost;
  this->onList = rhs->onList;
  return *this;
}

void GridNode::ClearData(void)
{
  parent = nullptr;
  givenCost = 999.99f;
  finalCost = 999.99f;
  // gridPos = GridPos(-999, -999);
  onList = UNVISITED;
}
