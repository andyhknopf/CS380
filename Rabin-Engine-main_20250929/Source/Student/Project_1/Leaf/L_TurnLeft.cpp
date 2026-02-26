#include <pch.h>
#include "L_TurnLeft.h"
#include "Agent/BehaviorAgent.h"
#include <cmath>

void L_TurnLeft::on_enter()
{
  const auto &bb = agent->get_blackboard();

  // Stop the agent
  agent->set_movement_speed(0.0f);
  startYaw = agent->get_yaw();
	BehaviorNode::on_leaf_enter();
  display_leaf_text();
}

void L_TurnLeft::on_update(float dt)
{
  float desiredYaw = PI / 2.0f + startYaw;
  float speed = 10.0f * dt;
  float epsilon = 0.001f;

  // Show debug info
  display_leaf_text();

  // Update the agents rotation
  float yaw = std::lerp(agent->get_yaw(), desiredYaw, speed);
  agent->set_yaw(yaw);

  // Just fall through if agent is still turning left
  if (agent->get_yaw() >= desiredYaw - epsilon)
   on_success();

  // Return success
}
