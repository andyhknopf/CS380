#pragma once
#include "BehaviorNode.h"
#include "Misc/NiceTypes.h"

class L_TurnLeft : public BaseNode<L_TurnLeft>
{
protected:
    virtual void on_enter() override;
    virtual void on_update(float) override;
private:
  float startYaw;
};