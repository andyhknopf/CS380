#pragma once
#include "BehaviorNode.h"
#include "Misc/NiceTypes.h"

class L_March : public BaseNode<L_March>
{
protected:
    virtual void on_enter() override;
    virtual void on_update(float dt) override;

private:
  Vec3 targetPoint;
};