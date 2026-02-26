#include <pch.h>
#include "D_AtMapCorner.h"

D_AtMapCorner::D_AtMapCorner() : counter(0)
{}

void D_AtMapCorner::on_enter()
{
  display_leaf_text();
  BehaviorNode::on_enter();
}

void D_AtMapCorner::on_update(float dt)
{
  // Repeat until at map corner
  // Seems a little redundant
  display_leaf_text();
  BehaviorNode * child = children.front();
  child->tick(dt);

  if (child->succeeded())
  {
    if (AtMapCorner())
      on_success();
    else
      child->set_status(NodeStatus::READY);
  }
}

bool D_AtMapCorner::AtMapCorner()
{
  const float CLOSE_ENOUGH = 10.0f;
  Vec3 corners[4] = { 
                      Vec3(0.0f),
                      Vec3(0.0f, 0.0f, 100.0f),
                      Vec3(100.0f, 0.0f, 0.0f),
                      Vec3(100.0f, 0.0f, 100.0f) 
                    };

  for (auto corner : corners)
  {
    float distance = Vec3::Distance(agent->get_position(), corner);
    if (distance > CLOSE_ENOUGH)
      continue;

    return true;
  }

  return false;
}
