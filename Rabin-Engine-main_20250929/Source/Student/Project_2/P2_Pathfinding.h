#pragma once
#include "Misc/PathfindingDetails.hpp"
#include "GridNode.h"

class AStarPather
{
public:
    /* 
        The class should be default constructible, so you might need to define a constructor.
        If needed, you can modify the framework where the class is constructed in the
        initialize functions of ProjectTwo and ProjectThree.
    */


    /* ************************************************** */
    // DO NOT MODIFY THESE SIGNATURES
    bool initialize();
    void shutdown();
    float CalculateHeuristic(GridNode* node, PathRequest & request);
    PathResult compute_path(PathRequest &request);
    PathResult SearchOpenList(GridNode*& parentNode, GridPos& goal, PathRequest& reques);
    void SearchNeighbors(GridNode* node, PathRequest &);
    void InitializeChildNode(GridNode* childNode, float childGiven, GridNode* parentNode);
    void PushFinalPath(GridNode*& node, PathRequest& request);
    bool RubberbandNodes(GridNode * node, GridNode* parent, bool rubberbandingEnabled);
    bool PathHasIntersection(GridNode* node, GridNode* grandparent);
    void OnMapChange();
    void PrecomputeNeighbors();
    bool FindEligibleNeighbors(GridPos& offset, GridPos& nodePos);
    bool IsEligibleNode(GridPos& offsetg);
    void AddNeighbor(GridPos&, GridPos&);
    /* ************************************************** */

    /*
        You should create whatever functions, variables, or classes you need.
        It doesn't all need to be in this header and cpp, structure it whatever way
        makes sense to you.
    */
    
    void ClearNodes(bool);
    void ClearOpenList();
    void Push(GridNode *);
    GridNode* Pop(void);
    GridNode * Pop_DEPRECATED();
    void Update(GridNode *, GridNode *, float);


    // Variables
    static inline const int MAX_MAP_SIZE = 40;
    static inline GridNode gridMap[MAX_MAP_SIZE][MAX_MAP_SIZE];
    // static inline GridNode* neighBors[MAX_MAP_SIZE][MAX_MAP_SIZE][8] = { nullptr }; // Precomputed ftw, 
    // never in my life did I think I would have a 3D array of pointers of my own volition
    GridNode *openList[MAX_MAP_SIZE * MAX_MAP_SIZE];
    bool openListEmpty;
    GridPos posChecker;
    bool firstLoad;

    GridPos start;
    GridPos goal;
    GridNode* startNode;

  private:
    int back; // Back of the open list
    unsigned int openIndex; // Index of first 'available' slot
};