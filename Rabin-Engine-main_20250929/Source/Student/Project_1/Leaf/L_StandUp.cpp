#include <pch.h>
#include "L_StandUp.h"
#include "Agent/BehaviorAgent.h"
#include <cmath>

void L_StandUp::on_enter()
{

  const auto &bb = agent->get_blackboard();

	BehaviorNode::on_leaf_enter();
}

void L_StandUp::on_update(float dt)
{
  float epsilon = 0.001f;
  float speed = 10.0f;
  float desiredPitch = 0;

  // Rotate the agent back upwards
  agent->set_pitch(std::lerp(agent->get_pitch(), desiredPitch, dt * speed));

  // Check to see when we've rotated back up
  if (agent->get_pitch() < epsilon)
  {
    // Reset the agents original propeties
    Vec3 originalPosition(agent->get_position().x, 0.0f, agent->get_position().z);
    agent->set_position(originalPosition);
    agent->set_movement_speed(10.0f);
    on_success();
  }
}