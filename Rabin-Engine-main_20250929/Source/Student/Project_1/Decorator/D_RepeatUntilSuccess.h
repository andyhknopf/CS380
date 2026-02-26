#pragma once
#include "BehaviorNode.h"

class D_RepeatUntilSuccess : public BaseNode<D_RepeatUntilSuccess>
{
protected:
    virtual void on_update(float dt) override;
    virtual void on_exit() override;
};