#pragma once
#include "BehaviorNode.h"
#include "Misc/NiceTypes.h"

class L_Fly : public BaseNode<L_Fly>
{
protected:
    virtual void on_enter() override;
    virtual void on_update(float) override;
private:
  void AvoidBounds(float);
  void UpdateRotation(float);
  float colorFactor;
};