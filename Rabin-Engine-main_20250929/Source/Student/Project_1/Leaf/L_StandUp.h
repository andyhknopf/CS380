#pragma once
#include "BehaviorNode.h"
#include "Misc/NiceTypes.h"

class L_StandUp : public BaseNode<L_StandUp>
{
protected:
    virtual void on_enter() override;
    virtual void on_update(float) override;
private:
};