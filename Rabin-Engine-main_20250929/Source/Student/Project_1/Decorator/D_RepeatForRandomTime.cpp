#include <pch.h>
#include "D_RepeatForRandomTime.h"

D_RepeatForRandomTime::D_RepeatForRandomTime() : time(0.0f)
{}

void D_RepeatForRandomTime::on_enter()
{
    time = RNG::range(1.0f, 30.0f);

    BehaviorNode::on_enter();
}

void D_RepeatForRandomTime::on_update(float dt)
{
  time -= dt;

  BehaviorNode *child = children.front();

  child->tick(dt);
  if (time < 0.0f)
  {
    on_success();
  }
}
