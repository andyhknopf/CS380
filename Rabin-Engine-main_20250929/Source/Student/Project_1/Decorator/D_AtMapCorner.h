#pragma once
#include "BehaviorNode.h"

class D_AtMapCorner : public BaseNode<D_AtMapCorner>
{
public:
    D_AtMapCorner();

protected:
    unsigned counter;

    virtual void on_enter() override;
    virtual void on_update(float dt) override;
    bool AtMapCorner();
};