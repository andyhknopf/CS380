#pragma once
#include "BehaviorNode.h"

class D_RepeatForRandomTime : public BaseNode<D_RepeatForRandomTime>
{
public:
    D_RepeatForRandomTime();

protected:
    float time;

    virtual void on_enter() override;
    virtual void on_update(float dt) override;
};