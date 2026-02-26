#pragma once
#include "BehaviorNode.h"
#include "Misc/NiceTypes.h"

class L_FallForward : public BaseNode<L_FallForward>
{
protected:
    virtual void on_enter() override;
    virtual void on_update(float) override;
private:
};