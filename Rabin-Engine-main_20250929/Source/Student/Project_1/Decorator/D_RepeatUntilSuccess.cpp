#include <pch.h>
#include "D_RepeatUntilSuccess.h"

void D_RepeatUntilSuccess::on_update(float dt)
{
  BehaviorNode *child = children.front();

  child->tick(dt);

  // Only return success if child was successful
  if (child->succeeded())
    on_success();
}

void D_RepeatUntilSuccess::on_exit()
{

}

