#include <pch.h>
#include "L_March.h"
#include "Agent/BehaviorAgent.h"

void L_March::on_enter()
{
  // set animation, speed, etc
  agent->set_movement_speed(10.0f);
  targetPoint = agent->get_position() + agent->get_forward_vector() * 5.0f;

  // grab the target position from the blackboard
  const auto &bb = agent->get_blackboard();

	BehaviorNode::on_leaf_enter();
}

// Update
void L_March::on_update(float dt)
{
  const auto result = agent->move_toward_point(targetPoint, dt);

  if (result == true)
  {
    on_success();
  }

  display_leaf_text();
}